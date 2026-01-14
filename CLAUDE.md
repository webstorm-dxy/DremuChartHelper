# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

DremuChartHelper is a Windows desktop application built with Avalonia UI for creating/manipulating musical charts (likely for rhythm games). It uses .NET 9.0 with the MVVM architectural pattern.

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

The project is part of a solution file: `DremuChartHelper.sln`

## Architecture

### MVVM Pattern

The application follows a strict MVVM (Model-View-ViewModel) architecture:

- **Views** (`Views/`): XAML files defining UI layout
- **ViewModels** (`ViewModels/`): Business logic and state management
- **Models** (`Models/`): Data models (currently empty)

### View-ViewModel Wiring

Views and ViewModels are automatically connected via `ViewLocator.cs`:
- ViewModels inherit from `ViewModelBase` (which extends `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`)
- The ViewLocator uses reflection to match ViewModels to Views by replacing "ViewModel" with "View" in the class name
- Example: `HomePageViewModel` → `HomePageView`

### Key Dependencies

- **Avalonia UI** (11.3.10): Cross-platform UI framework
- **FluentAvaloniaUI** (2.4.1): Fluent design system components (NavigationView, Frame, etc.)
- **CommunityToolkit.Mvvm** (8.2.1): MVVM helpers with source generators

### Application Structure

- `Program.cs`: Application entry point
- `App.axaml/.cs`: Application bootstrap, DI setup, disables Avalonia's data annotation validation to avoid conflicts with CommunityToolkit.Mvvm
- `Views/MainWindow.axaml`: Main window with Fluent NavigationView sidebar
- Navigation uses FluentAvalonia's `NavigationView` with a `Frame` for content
  - Navigation is implemented in `MainWindow.axaml.cs` via `OnItemInvoked()` handler
  - Each navigation item has a `Tag` property that maps to the View type
  - Default page (ProjectManagerView) is loaded in MainWindow constructor

### UI Language

The user interface uses Chinese text (首页 = Home, 设置 = Settings).

## Important Technical Details

1. **Compiled Bindings**: Enabled by default (`AvaloniaUseCompiledBindingsByDefault=true`) for better performance
2. **Acrylic Materials**: Uses experimental acrylic blur effects (`TransparencyLevelHint="AcrylicBlur"`)
3. **Extended Client Area**: Window chrome extends into title bar area (`ExtendClientAreaToDecorationsHint="True"`)
4. **Validation Plugin**: Avalonia's DataAnnotations validation is disabled in `App.axaml.cs` to work with CommunityToolkit.Mvvm

## Adding New Pages

When creating new navigation pages:

1. Create View in `Views/` (e.g., `ChartsPage.axaml`)
2. Create ViewModel in `ViewModels/` (e.g., `ChartsPageViewModel.cs`) as a `partial class` inheriting from `ViewModelBase`
3. Add navigation item in `MainWindow.axaml`'s `<NavigationView.MenuItems>` with appropriate `Tag`
4. Add navigation case in `MainWindow.axaml.cs`'s `OnItemInvoked()` method mapping the `Tag` to the View type
5. Use CommunityToolkit.Mvvm's `[ObservableProperty]` and `[RelayCommand]` attributes in ViewModels
   - The `partial` modifier enables source generators for automatic property change notification