# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory is a desktop application that simulates an automated factory. It models production and logistics processes, including production lines, a manually controlled robot, LEGO bricks in different colors, customer orders, transportation, quality control, and recycling.

The project was developed as part of the **Software Engineering** module at **TH Köln**. Development took place over five consecutive laboratory assignments, with each lab extending the existing application.

## Project features

- **Production lines:** Four production lines produce and move LEGO bricks with randomly assigned colors.
- **Robot control:** A manually controlled robot moves through the factory and picks up and releases bricks.
- **Customer orders:** Orders consist of eight randomly selected bricks. Duplicate colors and bricks may occur.
- **Logistics:** The logistics component determines which brick is needed next and from which station it must be collected.
- **Quality control:** Delivered bricks are subject to a probabilistic quality check.
- **Recycling:** Rejected bricks are transported to the recycling line and can be collected again.
- **Graphical simulation:** The Avalonia user interface displays the factory, robot, production lines, and brick movements.

## Application window

The application starts in maximized window mode. The normal window decorations, including the title bar and close button, remain visible.

## Architecture

The application follows an object-oriented architecture. Its classes are separated according to their responsibilities.

| Component | Responsibility |
|---|---|
| `MainWindow` | Displays the graphical user interface and forwards user input. |
| `MainViewModel` | Connects the user interface to the application logic and provides display data. |
| `LegoBrickViewModel` | Provides the data required to display individual LEGO bricks. |
| `Factory` | Coordinates production lines, the robot, transportation, and factory processes. |
| `Logistics` | Manages customer orders, delivery progress, and quality control. |
| `ProductionLine` | Creates and moves LEGO bricks on a production line. |
| `Robot` | Implements the robot-specific movement and positioning behavior. |
| `LegoBrick` | Represents a brick, including its color, size, status, and transport behavior. |
| `Beweger` | Provides shared movement, rotation, collision detection, and docking functionality. |
| `Pose` | Stores an object's position and orientation. |
| `Parameters` and `ViewParameters` | Contain general application settings and geometric parameters. |

The graphical user interface is built with **Avalonia UI**. The application uses **CommunityToolkit.Mvvm** for its view-model functionality.

## Requirements

- .NET 10 SDK
- Visual Studio or Visual Studio Code (or another compatible .NET development environment)

## Build and run

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

## Project context

SmartFactory is an educational project developed as part of the Software Engineering module at TH Köln. The five laboratory assignments build on one another and progressively extend the original factory simulation.

---

**University:** TH Köln

**Module:** Software Engineering

**Project:** SmartFactory