# SmartFactory – Practical Assessment Extension

[English](README.md) | [Deutsch](README.de.md)

This branch contains an independent extension to the SmartFactory project, implemented after completion of the five original laboratory assignments at TH Köln.

The practical assessment focused on extending the existing factory simulation with a function for randomly redistributing LEGO brick colors across all four production lines.

The task was completed independently, without AI assistance or internet access.

## Task: Production-Line Color Shuffle

The objective was to introduce a Shuffle function that redistributes the existing colors among eligible bricks on all four production lines while preserving the underlying factory state.

### Implemented Requirements

- **Random color redistribution:** Brick colors are randomly reassigned across all four production lines.
- **Preservation of color counts:** The total number of bricks per color remains unchanged.
- **Fixed brick positions:** The physical positions of the bricks remain unchanged.
- **Unchanged bricks outside production lines:** Bricks outside the four production lines retain their original colors.
- **Logistics consistency:** Pickup station selection reflects the current color distribution. The selected station must contain an available brick of the required color.
- **Setup-dependent availability:** The Shuffle button becomes available after production setup.
- **Status feedback:** The interface reports the number of shuffled bricks.

## Implementation

The extension builds on the existing object-oriented architecture and involves several application components:

| Component | Responsibility in the extension |
|---|---|
| `Factory` | Identifies eligible bricks and coordinates the color redistribution. |
| `Logistics` | Determines an appropriate pickup station based on the current availability of the required brick color. |
| `LegoBrick` | Stores the brick's color and other state information. |
| `MainViewModel` | Connects the Shuffle operation to the user interface and manages its availability. |
| `LegoBrickViewModel` | Provides updated display properties for the bricks. |
| `MainWindow` | Provides the Shuffle button and displays the status feedback. |

The color redistribution operates on the existing color values. Consequently, the number of bricks per color is preserved without changing the physical arrangement of the production lines.

## Technologies

- C#
- .NET 10
- Avalonia UI
- XAML
- CommunityToolkit.Mvvm
- MVVM (Model-View-ViewModel)

## Build and Run

Requirements:
- .NET 10 SDK
- A compatible .NET development environment

Clone this branch:

```bash
git clone --branch exam-task-shuffle https://github.com/ffeith/SWT_SmartFactoryApp.git
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

## Project Context

This practical assessment extension is separate from the original five laboratory assignments and is maintained in its own Git branch.

For the complete project overview, architecture, and documentation of the semester project, see the [`main` branch](https://github.com/ffeith/SWT_SmartFactoryApp/blob/main/README.md).

---

**University:** TH Köln · **Module:** Software Engineering · **Project:** SmartFactory