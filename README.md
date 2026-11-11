# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory is a desktop application that simulates an automated factory. It models key processes in a small production and logistics environment, including production lines, a controllable robot, LEGO bricks in different colors, customer orders, transportation, and recycling.

The project was developed as part of the **Software Engineering** module at **TH Köln**. Development took place over **five consecutive laboratory assignments**, completed week by week throughout the module. Each lab extended the application step by step. The result is a factory simulation in which the individual components work together as part of a larger system.

## Important: Use the application in full-screen mode

> **The application must be used in full-screen mode.**
>
> The graphical user interface was designed using fixed pixel coordinates for a full-screen layout. The standard window displayed when the application starts is smaller than the intended workspace. When the application is not running in full-screen mode, controls and factory objects may appear outside the visible area or may be positioned incorrectly.
>
> Switch to full-screen mode before using the application.

## Project features

The SmartFactory simulation covers the following areas:

- **Production lines:** Bricks are created on several production lines and move within the factory.
- **Robot control:** A robot can be controlled through the user interface. It moves around the factory and can pick up and release bricks.
- **Customer orders:** Orders specify which bricks need to be delivered to a customer.
- **Logistics planning:** The logistics component coordinates the required bricks and helps determine the next transportation task.
- **Delivery and recycling:** Bricks are transported to the customer or sent to recycling. The factory coordinates the related processes.
- **Graphical simulation:** The Avalonia user interface displays the positions, movements, and states of factory objects.

## Architecture

The application follows an object-oriented design. Classes are organized by responsibility and are based on the software architecture provided for the laboratory assignments.

| Component | Responsibility |
|---|---|
| `MainWindow` | Displays the graphical user interface and forwards user input. |
| `MainViewModel` | Connects the user interface to the application logic and provides data for display. |
| `Factory` | Coordinates production lines, the robot, and factory processes. |
| `Logistics` | Manages customer orders, delivery progress, and logistics planning. |
| `ProductionLine` | Manages and moves the bricks on a production line. |
| `Robot` | Controls the robot within the factory. |
| `LegoBrick` | Represents a brick, including its state, properties, and movement behavior. |
| `Beweger` | Provides shared movement, rotation, collision detection, and docking functionality. |
| `Pose` | Stores an object's position and orientation. |
| `Parameters` and `ViewParameters` | Provide application-wide settings as well as geometric and visual parameters. |

The user interface is built with **Avalonia UI**. The **CommunityToolkit.Mvvm** library is used to connect view models and views.

## Requirements

To build and run the project, you need:

- .NET 10 SDK
- A development environment that supports C# and .NET, such as Visual Studio or Visual Studio Code

## Run the project

Clone the repository:

```bash
git clone https://github.com/ffeith/SWT_SmartFactoryApp.git
cd SWT_SmartFactoryApp
```

Restore dependencies and start the application:

```bash
dotnet restore
dotnet run
```

After startup, switch the application to **full-screen mode**. See [Important: Use the application in full-screen mode](#important-use-the-application-in-full-screen-mode) for details.

## Project context

SmartFactory is an educational project developed as part of a university module. The five labs build on one another. The application was expanded incrementally, with new requirements integrated into the existing object-oriented structure throughout the module.

---

**University:** TH Köln  
**Module:** Software Engineering  
**Project:** SmartFactory
