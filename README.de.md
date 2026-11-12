# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory ist eine Desktopanwendung zur Simulation einer automatisierten Fabrik. Sie bildet Produktions- und Logistikprozesse ab. Dazu gehören Produktionslinien, ein manuell steuerbarer Roboter, LEGO-Steine unterschiedlicher Farben, Kundenaufträge, Qualitätskontrolle, Transport und Recycling.

Das Projekt entstand im Rahmen des Moduls **Softwaretechnik** an der **TH Köln**. Die Entwicklung erfolgte in fünf aufeinander aufbauenden Laboren. Mit jedem Labor wurde die bestehende Anwendung schrittweise erweitert.

## Funktionen

- **Produktionslinien:** Vier Produktionslinien erzeugen und bewegen LEGO-Steine mit zufällig zugewiesenen Farben.
- **Robotersteuerung:** Ein manuell steuerbarer Roboter bewegt sich durch die Fabrik und kann Steine aufnehmen und absetzen.
- **Kundenaufträge:** Ein Auftrag besteht aus acht zufällig ausgewählten Steinen. Dabei können auch gleiche Farben beziehungsweise Steine mehrfach vorkommen.
- **Logistik:** Die Logistik bestimmt, welcher Stein als Nächstes benötigt wird und an welcher Station er abgeholt werden muss.
- **Qualitätskontrolle:** Abgegebene Steine durchlaufen eine zufallsbasierte Qualitätskontrolle.
- **Recycling:** Zurückgewiesene Steine werden zur Recyclinglinie transportiert und können erneut abgeholt werden.
- **Grafische Simulation:** Die Avalonia-Benutzeroberfläche stellt die Fabrik, den Roboter, die Produktionslinien und die Bewegungen der Steine dar.

## Anwendungsfenster

Die Anwendung startet in einem maximierten Fenstermodus. Die normale Fensterdekoration einschließlich Titelleiste und Schließen-Schaltfläche bleibt sichtbar.

## Architektur

Die Anwendung ist objektorientiert aufgebaut. Die Klassen sind nach ihren jeweiligen Verantwortlichkeiten aufgeteilt.

| Komponente | Verantwortung |
|---|---|
| `MainWindow` | Darstellung der grafischen Oberfläche und Weiterleitung von Benutzereingaben. |
| `MainViewModel` | Verbindung zwischen Benutzeroberfläche und Anwendungslogik sowie Bereitstellung der angezeigten Daten. |
| `LegoBrickViewModel` | Bereitstellung der Daten zur grafischen Darstellung einzelner LEGO-Steine. |
| `Factory` | Koordination der Produktionslinien, des Roboters, des Transports und der Fabrikabläufe. |
| `Logistics` | Verwaltung von Kundenaufträgen, Lieferfortschritt und Qualitätskontrolle. |
| `ProductionLine` | Erzeugung und Bewegung der LEGO-Steine auf einer Produktionslinie. |
| `Robot` | Implementierung der spezifischen Bewegungs- und Positionierungslogik des Roboters. |
| `LegoBrick` | Repräsentation eines Steins einschließlich Farbe, Größe, Status und Transportverhalten. |
| `Beweger` | Gemeinsame Bewegungs-, Rotations-, Kollisions- und Dockingfunktionen. |
| `Pose` | Speicherung der Position und Ausrichtung eines Objekts. |
| `Parameters` und `ViewParameters` | Zentrale Anwendungswerte sowie geometrische Einstellungen. |

Die grafische Benutzeroberfläche basiert auf **Avalonia UI**. Für die ViewModel-Funktionalität wird **CommunityToolkit.Mvvm** verwendet.

## Voraussetzungen

- .NET 10 SDK
- Visual Studio oder Visual Studio Code (oder eine andere kompatible .NET-Entwicklungsumgebung)

## Projekt bauen und starten

Repository klonen:

```bash
git clone https://github.com/ffeith/SWT_SmartFactoryApp.git
cd SWT_SmartFactoryApp
```

Abhängigkeiten wiederherstellen:

```bash
dotnet restore
```

Projekt bauen:

```bash
dotnet build
```

Anwendung starten:

```bash
dotnet run
```

## Projektkontext

SmartFactory ist ein Lehrprojekt im Rahmen des Moduls Softwaretechnik an der TH Köln. Die fünf aufeinander aufbauenden Labore erweitern die ursprüngliche Fabriksimulation schrittweise um zusätzliche Funktionen und eine zunehmend umfangreiche objektorientierte Struktur.

---

**Hochschule:** TH Köln

**Modul:** Softwaretechnik

**Projekt:** SmartFactory