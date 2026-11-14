# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory ist eine interaktive Desktop-Simulation einer LEGO-Stein-Fabrik, entwickelt mit C#, .NET 10 und Avalonia UI. Die Anwendung bildet Produktions- und Logistikprozesse ab, darunter Produktionslinien, einen manuell gesteuerten Roboter, Kundenaufträge, Transportvorgänge, Qualitätskontrollen und Recycling.

Das Projekt entstand im Rahmen des Moduls **Softwaretechnik** an der **TH Köln**. Das ursprüngliche Semesterprojekt besteht aus fünf aufeinander aufbauenden Laborübungen, in denen die Funktionalität und Architektur der Anwendung schrittweise erweitert wurden.

## Funktionen

- **Produktionslinien:** Vier Produktionslinien fertigen LEGO-Steine mit zufällig zugewiesenen Farben und bewegen diese entlang der Förderbänder.
- **Robotersteuerung:** Ein manuell gesteuerter Roboter bewegt sich durch die Fabrik, nimmt Steine auf und transportiert sie zwischen den Stationen.
- **Kundenaufträge:** Aufträge bestehen aus acht zufällig ausgewählten LEGO-Steinen.
- **Logistik:** Ermittelt den als Nächstes benötigten Stein und wählt eine geeignete Abholstation aus.
- **Qualitätskontrolle und Recycling:** Die Steine durchlaufen eine probabilistische Qualitätsprüfung. Abgelehnte Steine werden zur Recyclingstation transportiert und können für eine erneute Prüfung abgeholt werden.
- **Grafische Simulation:** Die Avalonia-Oberfläche visualisiert die Fabrik, Produktionslinien, Roboterbewegungen, Transportvorgänge und den Fortschritt der Auftragsbearbeitung.

## Architektur

Die Anwendung folgt einer objektorientierten Architektur mit klar voneinander abgegrenzten Verantwortlichkeiten.

| Komponente | Verantwortung |
|---|---|
| `MainWindow` | Stellt die grafische Benutzeroberfläche dar und leitet Benutzereingaben weiter. |
| `MainViewModel` | Verbindet die Benutzeroberfläche mit der Anwendungslogik und verwaltet den Darstellungszustand. |
| `LegoBricksViewModel` | Stellt die Darstellungsinformationen für einzelne LEGO-Steine bereit. |
| `Factory` | Koordiniert Produktionslinien, Roboter, Transportvorgänge und Fabrikprozesse. |
| `Logistics` | Verwaltet Aufträge, den Lieferfortschritt und die Auswahl der Abholstationen. |
| `ProductionLine` | Erstellt und bewegt LEGO-Steine entlang der Produktionslinien. |
| `Robot` | Implementiert die spezifischen Bewegungs- und Positionierungsfunktionen des Roboters. |
| `LegoBrick` | Repräsentiert einen LEGO-Stein einschließlich seiner Eigenschaften und seines Zustands. |
| `Beweger` | Stellt gemeinsame Funktionen für Bewegung, Rotation, Kollisionserkennung und Andockvorgänge bereit. |
| `Pose` | Speichert die Position und Ausrichtung eines Objekts. |
| `Parameters` und `ViewParameters` | Enthalten Anwendungs- und geometrische Parameter. |

## Technische Schwerpunkte

Das Projekt verbindet verschiedene Bereiche der Softwareentwicklung:

- **Objektorientierte Programmierung:** Modellierung der Fabrikkomponenten und ihrer Interaktionen durch Klassen mit klar definierten Verantwortlichkeiten.
- **MVVM-Architektur:** Trennung der grafischen Oberfläche von der Anwendungslogik mithilfe von ViewModels und Data Binding.
- **Anwendungslogik:** Umsetzung koordinierter Abläufe für Produktion, Transport, Auftragsbearbeitung, Qualitätskontrolle und Recycling.
- **Entwicklung grafischer Benutzeroberflächen:** Gestaltung einer interaktiven Desktop-Anwendung mit Avalonia UI, XAML und CommunityToolkit.Mvvm.

## Zusätzliche praktische Prüfungsaufgabe

Nach Abschluss der fünf Laborübungen wurde eine separate praktische Prüfungsaufgabe als eigenständige Erweiterung umgesetzt. Sie ist nicht Bestandteil der ursprünglichen fünf Labs und wird in einem eigenen Git-Branch geführt.

Die Aufgabe wurde eigenständig, ohne KI-Unterstützung und ohne Internetzugang, bearbeitet.

### Zufällige Neuverteilung der Farben auf den Produktionslinien

Die Erweiterung ergänzt die Fabrik um eine Funktion zur zufälligen Neuverteilung der Farben von LEGO-Steinen auf allen vier Produktionslinien.

Die Funktion umfasst:

- Zufällige Neuverteilung der Farben geeigneter Steine auf den vier Produktionslinien.
- Erhaltung der Gesamtanzahl der Steine je Farbe.
- Beibehaltung der physischen Positionen der Steine.
- Keine Farbänderungen bei Steinen, die sich außerhalb der vier Produktionslinien befinden.
- Aktualisierung der Logistikanweisungen und der Auswahl der Abholstation anhand der aktuellen Farbverteilung. Die ausgewählte Station muss tatsächlich einen verfügbaren Stein der benötigten Farbe enthalten.
- Aktivierung der Shuffle-Schaltfläche nach der Einrichtung der Produktion.
- Eine Statusmeldung, die die Anzahl der neu verteilten Steine angibt.

Die Implementierung erweitert die bestehende Architektur, insbesondere die Komponenten `Factory`, `Logistics` und `LegoBrick`, sowie die ViewModels und die grafische Benutzeroberfläche.

Die Erweiterung ist im Branch [`exam-task-shuffle`](https://github.com/ffeith/SWT_SmartFactoryApp/tree/exam-task-shuffle) verfügbar.

## Technologien

- C#
- .NET 10
- Avalonia UI
- XAML
- CommunityToolkit.Mvvm
- MVVM (Model-View-ViewModel)

Das MVVM-Muster sorgt für eine Trennung zwischen grafischer Darstellung, Anwendungssteuerung und fachlicher Logik.

## Voraussetzungen

- .NET 10 SDK
- Visual Studio, Visual Studio Code oder eine andere kompatible .NET-Entwicklungsumgebung

## Projekt erstellen und starten

Repository klonen:

```bash
git clone https://github.com/ffeith/SWT_SmartFactoryApp.git
cd SWT_SmartFactoryApp
```

Abhängigkeiten wiederherstellen:

```bash
dotnet restore
```

Anwendung kompilieren:

```bash
dotnet build
```

Anwendung starten:

```bash
dotnet run
```

---

**Hochschule:** TH Köln · **Modul:** Softwaretechnik · **Projekt:** SmartFactory