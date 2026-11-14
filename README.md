# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory is an interactive desktop simulation of a LEGO brick factory, developed with C#, .NET 10, and Avalonia UI. The application models production and logistics processes involving production lines, a manually controlled robot, customer orders, transportation, quality control, and recycling.

The project was developed as part of the **Software Engineering** module at **TH Köln**. The main project consists of five consecutive laboratory assignments, with each lab building on the previous version and extending the application's functionality and architecture.

## Features

- **Production lines:** Four production lines create and move LEGO bricks with randomly assigned colors.
- **Robot control:** A manually controlled robot navigates through the factory, picks up bricks, and transports them between stations.
- **Customer orders:** Orders consist of eight randomly selected bricks.
- **Logistics:** Determines the next required brick and selects an appropriate pickup station.
- **Quality control and recycling:** Bricks undergo a probabilistic quality check. Rejected bricks are transferred to the recycling line and can be collected for another inspection.
- **Graphical simulation:** The Avalonia interface visualizes the factory, production lines, robot movements, transportation, and order progress.

## Architecture

The application follows an object-oriented architecture with clearly separated responsibilities.

| Component | Responsibility |
|---|---|
| `MainWindow` | Displays the graphical user interface and forwards user input. |
| `MainViewModel` | Connects the user interface to the application logic and manages presentation state. |
| `LegoBricksViewModel` | Provides display properties for individual LEGO bricks. |
| `Factory` | Coordinates production lines, the robot, transportation, and factory processes. |
| `Logistics` | Manages orders, delivery progress, and pickup station selection. |
| `ProductionLine` | Creates and moves LEGO bricks along production lines. |
| `Robot` | Implements robot-specific movement and positioning. |
| `LegoBrick` | Represents a brick, including its properties and state. |
| `Beweger` | Provides shared movement, rotation, collision detection, and docking functionality. |
| `Pose` | Stores an object's position and orientation. |
| `Parameters` and `ViewParameters` | Contain application settings and geometric parameters. |

## Technical Focus

The project combines several areas of software development:

- **Object-oriented programming:** Modeling factory components and their interactions through dedicated classes and responsibilities.
- **MVVM architecture:** Separating the graphical interface from application logic using ViewModels and data binding.
- **Application logic:** Implementing coordinated processes for production, transportation, order handling, quality control, and recycling.
- **UI development:** Building an interactive desktop interface with Avalonia UI, XAML, and CommunityToolkit.Mvvm.

## Additional Practical Assessment Task

After completing the five laboratory assignments, a separate practical assessment task was implemented as an independent extension. It is not part of the original five labs and is maintained in its own branch.

The task was completed independently, without AI assistance or internet access.

### Production-Line Color Shuffle

The extension introduces a function for randomly redistributing brick colors across all four production lines.

Its behavior includes:

- Random redistribution of eligible brick colors across the four production lines.
- Preservation of the total number of bricks per color.
- Preservation of the bricks' physical positions.
- No color changes for bricks outside the four production lines.
- Updated logistics instructions and pickup station selection based on the current color distribution. The selected station must contain an available brick of the required color.
- Activation of the Shuffle button after production setup.
- A status message reporting the number of shuffled bricks.

The implementation extends the existing `Factory`, `Logistics`, and `LegoBrick` components, together with the ViewModels and graphical interface.

The extension is available in the [`exam-task-shuffle` branch](https://github.com/ffeith/SWT_SmartFactoryApp/tree/exam-task-shuffle).

## Technologies

- C#
- .NET 10
- Avalonia UI
- XAML
- CommunityToolkit.Mvvm
- MVVM (Model-View-ViewModel)

## Requirements

- .NET 10 SDK
- Visual Studio, Visual Studio Code, or another compatible .NET development environment

## Build and Run

Clone the repository:

```bash
git clone https://github.com/ffeith/SWT_SmartFactoryApp.git
cd SWT_SmartFactoryApp
```

Restore dependencies:

```bash
dotnet restore
```

Build the application:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

---

**University:** TH Köln · **Module:** Software Engineering · **Project:** SmartFactory