# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory is a desktop application that simulates an automated factory. It combines production processes, manual robot control, logistics planning, customer orders, quality control, and recycling in an interactive graphical environment.

The project was developed as part of the **Software Engineering** module at **TH Köln**. The original application evolved through five consecutive laboratory assignments, each introducing new functionality and extending the existing object-oriented architecture.

## Features

- **Production lines:** Four production lines create and move LEGO bricks with randomly assigned colors.
- **Robot control:** A manually controlled robot moves through the factory, interacts with production lines, and picks up and releases bricks.
- **Customer orders:** Orders consist of eight randomly selected bricks.
- **Logistics planning:** The logistics component determines the next required brick and the appropriate pickup station.
- **Quality control:** Bricks undergo a probabilistic quality check.
- **Recycling:** Rejected bricks are transported to the recycling line and can be collected again.
- **Graphical simulation:** The Avalonia interface visualizes production lines, the robot, LEGO bricks, transportation, and order progress.

## Architecture

The application follows an object-oriented architecture with clearly separated responsibilities.

| Component | Responsibility |
|---|---|
| `MainWindow` | Displays the graphical user interface and forwards user input. |
| `MainViewModel` | Connects the user interface to the application logic and manages presentation state. |
| `LegoBrickViewModel` | Provides display properties for individual LEGO bricks. |
| `Factory` | Coordinates production lines, the robot, transportation, and factory processes. |
| `Logistics` | Manages orders, delivery progress, and pickup station selection. |
| `ProductionLine` | Creates and moves LEGO bricks on production lines. |
| `Robot` | Implements robot-specific movement and positioning. |
| `LegoBrick` | Represents a brick, including its properties, state, and transport behavior. |
| `Beweger` | Provides shared movement, rotation, collision detection, and docking functionality. |
| `Pose` | Stores an object's position and orientation. |
| `Parameters` and `ViewParameters` | Contain application settings and geometric parameters. |

## Technologies

- C#
- .NET 10
- Avalonia UI
- CommunityToolkit.Mvvm
- XAML

The graphical interface follows the MVVM (Model-View-ViewModel) pattern, separating presentation, application control, and domain logic.

## Independent Practical Assessment Extension (After Lab 5)

After completion of the five laboratory assignments, a separate practical assessment task was implemented as an independent extension. It is not part of the original five labs. The implementation was completed independently, without AI assistance or internet access.

### Shuffle Production-Line Bricks

The extension introduces a function for randomly redistributing brick colors across all four production lines.

Its behavior includes:

- Random redistribution of the existing colors across all four production lines.
- Preservation of the total number of bricks per color.
- Preservation of the bricks' physical positions.
- No color changes for bricks currently outside the four production lines.
- Updated logistics instructions and pickup station selection based on the current color distribution. The selected station must actually contain an available brick of the required color.
- Activation of the Shuffle button after production setup.
- A status message reporting the number of shuffled bricks.

The extension builds on the existing architecture, particularly the `Factory`, `Logistics`, and `LegoBrick` components, together with the ViewModels and graphical interface.

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

**University:** TH Köln<br>
**Module:** Software Engineering<br>
**Project:** SmartFactory