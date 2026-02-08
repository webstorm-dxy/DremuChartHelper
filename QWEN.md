# DremuChartHelper

DremuChartHelper is a Windows desktop application built with Avalonia UI for creating and manipulating musical rhythm game charts. It uses .NET 9.0 with the MVVM architectural pattern and integrates with GorgeStudio (a separate server application) for chart data.

## Solution Structure

The solution contains two projects:
- **DremuChartHelper** - Main Avalonia UI application
- **GorgeStudio** - Service library providing JSON-RPC client for GorgeStudio integration

## Build and Run Commands

```bash
# Build the entire solution
dotnet build

# Run the application
dotnet run --project DremuChartHelper/DremuChartHelper.csproj

# Build release version
dotnet build -c Release

# Run with specific configuration
dotnet run --project DremuChartHelper/DremuChartHelper.csproj --configuration Release
```

**Important**: The application requires GorgeStudio server running on `http://localhost:14324` to function properly.

## Architecture

### MVVM Pattern

The application follows strict MVVM architecture:
- **Views** (`Views/`): XAML files defining UI layout
- **ViewModels** (`ViewModels/`): Business logic and state management
- **Models** (`Models/`): Data models and domain logic

### View-ViewModel Wiring

Views and ViewModels are automatically connected via `ViewLocator.cs`:
- ViewModels inherit from `ViewModelBase` (extends `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`)
- ViewLocator uses reflection to match ViewModels to Views by replacing "ViewModel" with "View"
- Example: `ProjectManagerViewModel` → `ProjectManagerView`
- Use `[ObservableProperty]` and `[RelayCommand]` attributes with `partial class` for source generation

### Key Dependencies

- **Avalonia UI** (11.3.10): Cross-platform UI framework
- **FluentAvaloniaUI** (2.4.1): Fluent design system components
- **CommunityToolkit.Mvvm** (8.2.1): MVVM helpers with source generators
- **System.Text.Json**: JSON serialization for GorgeStudio communication

## GorgeStudio Integration

The application communicates with GorgeStudio server via JSON-RPC protocol:

### Remote Function Client
- **Location**: `GorgeStudio/GorgeStudioServer/RemoteFunction.cs`
- **Endpoint**: `http://localhost:14324`
- **Protocol**: JSON-RPC 2.0
- **Pattern**: Singleton via `RemoteFunction.Instance.Value`

### Data Models
- **Location**: `GorgeStudio/GorgeStudioServer/DataModel.cs`
- Defines contract for:
  - `ScoreInformation` - Contains chart metadata and `Staves` array
  - `StaffInformation` - Represents individual chart sections
  - `ElementInformation` - Represents chart elements (notes, judgement lines, etc.)
  - `PeriodInformation` - Time periods within staves

### Chart Data Flow
```
GorgeStudio Server (localhost:14324)
  ↓ JSON-RPC
RemoteFunction.GetScoreInformationAsync()
  ↓
ChartInformation.InitializeAsync()
  ↓
ChartInformation.Staves populated
  ↓
Filters process data via SyncChartsAction
```

## Singleton Pattern Usage

Several singletons are used throughout the application:

### ChartInformation
- **Location**: `Models/GorgeLinker/ChartInformation.cs`
- **Access**: `ChartInformation.Instance.Value`
- **Purpose**: Global chart data cache with async initialization
- **Key Method**: `EnsureInitializedAsync()` - Ensures data is loaded before access

### RemoteFunction
- **Access**: `RemoteFunction.Instance.Value`
- **Purpose**: JSON-RPC client for GorgeStudio communication

## Filter Pattern

Chart data processing uses a filter-based architecture in `Models/GorgeLinker/ChartInformationFilter/`:

### ElementFilter (Base Class)
- **Purpose**: Base filter for element processing
- **Key Members**:
  - `Elements` - Protected array storing loaded elements
  - `LoadPeriodInformationsAsync()` - Loads elements from GorgeStudio
  - `OnElementsLoaded()` - Virtual method called after loading (override in subclasses)
- **Event Registration**: Uses static lock to ensure `SyncChartsAction` is only registered once
- **Thread Safety**: Static event registration prevents duplicate handlers

### JudgementLineFilter (Example Implementation)
- **Purpose**: Filters judgement line elements from loaded data
- **Inherits**: `ElementFilter`
- **Override**: `OnElementsLoaded()` to filter `Dremu.DremuMainLane` elements

### Creating New Filters
```csharp
public class MyFilter : ElementFilter
{
    protected override void OnElementsLoaded()
    {
        // Elements is now populated and safe to access
        var filtered = Elements.Where(e => e.ClassName == "SomeType").ToArray();
    }
}
```

## Important: Async Initialization and Timing

### Race Condition Prevention
When working with asynchronously loaded data:

1. **Always use `EnsureInitializedAsync()`** before accessing `ChartInformation.Staves`
2. **Never access `Elements` in constructors** of filter classes - use `OnElementsLoaded()` instead
3. **Never use `.Result` or `.Wait()`** on async methods in UI code - causes deadlocks

### Example Correct Pattern
```csharp
// In ViewModels or other code needing chart data
public async Task LoadData()
{
    var chartInfo = ChartInformation.Instance.Value;
    await chartInfo.EnsureInitializedAsync();

    if (chartInfo.Staves == null)
    {
        // Handle error
        return;
    }

    // Safe to use Staves
}
```

## Application Structure

### Main Navigation
- **MainWindow.axaml**: Main window with Fluent `NavigationView` sidebar
- **Navigation Pages**:
  - Project Manager (项目管理器)
  - Resource Manager (谱面资源管理器)
  - Curve Editor (曲线编辑器)
- Navigation uses `Frame` with `Tag` mapping in `MainWindow.axaml.cs`

### Key Directories
```
DremuChartHelper/
├── Models/
│   ├── DataManager/           # Project and recent file management
│   ├── GorgeLinker/          # GorgeStudio integration
│   │   ├── ChartInformation.cs      # Main data model
│   │   └── ChartInformationFilter/  # Data processing filters
│   └── Track/                # Timeline and track system
├── ViewModels/               # MVVM ViewModels
├── Views/                    # Avalonia XAML views
└── Controls/                 # Custom UI controls
```

## UI Language

The user interface uses Chinese text throughout. When adding new UI elements, use Chinese labels.

## Important Technical Details

1. **Compiled Bindings**: Enabled by default for performance
2. **Acrylic Materials**: Uses experimental acrylic blur effects
3. **Extended Client Area**: Window chrome extends into title bar
4. **Validation**: Avalonia's DataAnnotations validation is disabled in `App.axaml.cs` to work with CommunityToolkit.Mvvm
5. **Async/Await**: Extensive use of async patterns - always use `await` instead of `.Result` to avoid deadlocks

## Adding New Pages

1. Create View in `Views/` (e.g., `NewPage.axaml`)
2. Create ViewModel in `ViewModels/` (e.g., `NewPageViewModel.cs`) as `partial class` inheriting from `ViewModelBase`
3. Add navigation item in `MainWindow.axaml`'s `<NavigationView.MenuItems>` with appropriate `Tag`
4. Add navigation case in `MainWindow.axaml.cs`'s `OnItemInvoked()` method
5. Use `[ObservableProperty]` and `[RelayCommand]` attributes in ViewModels