# DremuChartHelper

DremuChartHelper is a Windows desktop application built with Avalonia UI for creating and manipulating musical rhythm game charts. It uses .NET 9.0 with the MVVM architectural pattern and integrates with a GorgeStudio server via the built-in GorgeLinker JSON-RPC client.

## Solution Structure

The solution contains two projects:
- **DremuChartHelper** - Main Avalonia UI application
- **DremuChartHelper.Tests** - xUnit test project

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
- **System.Text.Json**: JSON serialization for GorgeStudio server communication

## GorgeStudio Integration

The application communicates with the GorgeStudio server via JSON-RPC protocol:

### Remote Function Client
- **Location**: `DremuChartHelper/Models/GorgeLinker/GorgeStudioServer.cs`
- **Endpoint**: `http://localhost:14324`
- **Protocol**: JSON-RPC 2.0
- **Pattern**: Singleton via `RemoteFunction.Instance.Value`

### Data Models
- **Location**: `DremuChartHelper/Models/GorgeLinker/GorgeStudioServer.cs`
- Defines contract for:
  - `ScoreInformation` - Contains chart metadata and `Staves` array
  - `StaffInformation` - Represents individual chart sections
  - `ElementInformation` - Represents chart elements (notes, judgement lines, etc.)
  - `PeriodInformation` - Time periods within staves

### Chart Data Flow
```
GorgeStudio Server (localhost:14324)
  ↓ JSON-RPC
GorgeStudioChartRepository.GetScoreInformationAsync()
  ↓
ChartDataService.EnsureInitializedAsync()
  ↓
ChartDataService.Staves populated
  ↓
FilterManager processes data via ChartDataUpdated event
```

## Singleton Pattern Usage

RemoteFunction is available as a singleton via `RemoteFunction.Instance.Value` for direct JSON-RPC usage.

## Filter Pattern

Chart data processing uses a filter-based architecture in `Models/GorgeLinker/Filters/` and `Models/GorgeLinker/ChartInformationFilter/`:

### ElementFilterBase
- **Purpose**: Base filter contract for element processing
- **Key Members**:
  - `ShouldProcess()` - Filter strategy
  - `ProcessElementsAsync()` - Element handling entry point

### JudgementLineFilter (Example Implementation)
- **Purpose**: Filters judgement line elements from loaded data
- **Inherits**: `ElementFilterBase`
- **Override**: `ShouldProcess()` to filter `Dremu.DremuMainLane` elements

### Creating New Filters
```csharp
public class MyFilter : ElementFilterBase
{
    public override string Name => "MyFilter";

    public override bool ShouldProcess(ElementInformation element)
    {
        return element.ClassName == "SomeType";
    }

    public override Task ProcessElementsAsync(ElementInformation[] elements)
    {
        return Task.CompletedTask;
    }
}
```

## Important: Async Initialization and Timing

### Race Condition Prevention
When working with asynchronously loaded data:

1. **Always use `EnsureInitializedAsync()`** before accessing `IChartDataService.GetStaves()`
2. **Never do work in filter constructors** - use `ProcessElementsAsync()` after data arrives
3. **Never use `.Result` or `.Wait()`** on async methods in UI code - causes deadlocks

### Example Correct Pattern
```csharp
// In ViewModels or other code needing chart data
public async Task LoadData()
{
    await _chartDataService.EnsureInitializedAsync();
    var staves = _chartDataService.GetStaves();

    if (staves == null)
    {
        // Handle error
        return;
    }
    // Safe to use staves
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
│   ├── GorgeLinker/          # GorgeLinker integration
│   │   ├── Filters/          # Filter pipeline
│   │   ├── Repositories/     # JSON-RPC repository
│   │   ├── Services/         # Chart data services
│   │   └── GorgeStudioServer.cs # JSON-RPC contracts and client
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
