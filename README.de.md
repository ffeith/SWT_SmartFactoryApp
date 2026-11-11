# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory ist eine Desktopanwendung zur Simulation einer automatisierten Fabrik. Die Anwendung bildet zentrale Abläufe einer kleinen Produktions- und Logistikumgebung ab. Dazu gehören Produktionslinien, ein steuerbarer Roboter, LEGO-Steine unterschiedlicher Farben, Kundenaufträge sowie Transport- und Recyclingprozesse.

Das Projekt entstand im Rahmen des Moduls **Softwaretechnik** an der **TH Köln**. Die Entwicklung erfolgte in **fünf aufeinander aufbauenden Laboren**, die im Verlauf des Moduls Woche für Woche bearbeitet wurden. Mit jedem Labor wurde die Anwendung schrittweise erweitert. So entwickelte sich aus einer grundlegenden Fabriksimulation eine zunehmend umfangreiche Anwendung, in der die einzelnen Komponenten zusammenarbeiten.

## Wichtiger Hinweis: Anwendung im Vollbildmodus verwenden

> **Die Anwendung muss im Vollbildmodus verwendet werden.**
>
> Die grafische Oberfläche wurde mit festen Pixelkoordinaten für die Darstellung im Vollbild entworfen. Das beim Programmstart geöffnete Standardfenster ist kleiner als die dafür vorgesehene Fläche. Wird die Anwendung nicht im Vollbild ausgeführt, können Bedienelemente und Fabrikobjekte außerhalb des sichtbaren Bereichs liegen oder falsch positioniert erscheinen.
>
> Aktiviere deshalb vor der Bedienung den Vollbildmodus.

## Projektumfang

Die SmartFactory-Simulation umfasst die folgenden Bereiche:

- **Produktionslinien:** Steine werden auf mehreren Produktionslinien erzeugt und innerhalb der Fabrik bewegt.
- **Robotersteuerung:** Ein Roboter kann über die Benutzeroberfläche gesteuert werden. Er bewegt sich durch die Fabrik und kann Steine aufnehmen und absetzen.
- **Kundenaufträge:** Aufträge beschreiben, welche Steine an einen Kunden geliefert werden sollen.
- **Logistikplanung:** Die Logistik koordiniert die benötigten Steine und unterstützt die Auswahl der nächsten Transportaufgabe.
- **Lieferung und Recycling:** Steine werden zum Kunden transportiert oder dem Recycling zugeführt. Die Fabrik verwaltet die zugehörigen Abläufe.
- **Grafische Simulation:** Positionen, Bewegungen und Zustände der Fabrikobjekte werden in einer Avalonia-Benutzeroberfläche dargestellt.

## Architektur

Die Anwendung ist objektorientiert aufgebaut. Die Klassen sind nach fachlichen Aufgaben aufgeteilt und orientieren sich an der im Labor vorgegebenen Softwarearchitektur.

| Komponente | Verantwortung |
|---|---|
| `MainWindow` | Darstellung der grafischen Oberfläche und Weiterleitung von Benutzereingaben. |
| `MainViewModel` | Verbindung zwischen Oberfläche und Anwendungslogik sowie Bereitstellung der angezeigten Daten. |
| `Factory` | Koordination der Produktionslinien, des Roboters und der Fabrikabläufe. |
| `Logistics` | Verwaltung von Kundenaufträgen, Lieferfortschritt und Logistikplanung. |
| `ProductionLine` | Verwaltung und Bewegung der Steine einer Produktionslinie. |
| `Robot` | Steuerung des Roboters innerhalb der Fabrik. |
| `LegoBrick` | Zustand, Eigenschaften und Bewegungsverhalten eines Steins. |
| `Beweger` | Gemeinsame Bewegungs-, Rotations-, Kollisions- und Dockingfunktionen. |
| `Pose` | Speicherung von Position und Ausrichtung eines Objekts. |
| `Parameters` und `ViewParameters` | Zentrale Anwendungswerte sowie geometrische und grafische Einstellungen. |

Die Benutzeroberfläche basiert auf **Avalonia UI**. Für die Anbindung der Ansichtsmodelle wird das **CommunityToolkit.Mvvm** verwendet.

## Voraussetzungen

Für das Bauen und Ausführen des Projekts werden benötigt:

- .NET 10 SDK
- Eine Entwicklungsumgebung mit Unterstützung für C# und .NET, zum Beispiel Visual Studio oder Visual Studio Code

## Projekt ausführen

Repository klonen:

```bash
git clone https://github.com/ffeith/SWT_SmartFactoryApp.git
cd SWT_SmartFactoryApp
```

Abhängigkeiten wiederherstellen und Anwendung starten:

```bash
dotnet restore
dotnet run
```

Anschließend die Anwendung in den **Vollbildmodus** versetzen. Beachte dazu unbedingt den [wichtigen Hinweis zur Verwendung im Vollbildmodus](#wichtiger-hinweis-anwendung-im-vollbildmodus-verwenden).

## Projektkontext

SmartFactory ist ein im Rahmen eines Hochschulmoduls entwickeltes Lehrprojekt. Die fünf Labore bauen inhaltlich aufeinander auf. Die Anwendung wurde dabei schrittweise erweitert, wobei neue Anforderungen in die bestehende objektorientierte Struktur integriert wurden.

---

**Hochschule:** TH Köln  
**Modul:** Softwaretechnik  
**Projekt:** SmartFactory
