# SmartFactory

[English](README.md) | [Deutsch](README.de.md)

SmartFactory ist eine Desktopanwendung zur Simulation einer automatisierten Fabrik. Sie verbindet Produktionsabläufe, manuelle Robotersteuerung, Logistikplanung, Kundenaufträge, Qualitätskontrolle und Recycling in einer interaktiven grafischen Umgebung.

Das Projekt entstand im Rahmen des Moduls **Softwaretechnik** an der **TH Köln**. Die ursprüngliche Anwendung entwickelte sich über fünf aufeinander aufbauende Laboraufgaben hinweg. Jedes Labor führte neue Funktionen ein und erweiterte die bestehende objektorientierte Architektur.

## Funktionen

- **Produktionslinien:** Vier Produktionslinien erzeugen und bewegen LEGO-Steine mit zufällig zugewiesenen Farben.
- **Robotersteuerung:** Ein manuell steuerbarer Roboter bewegt sich durch die Fabrik, interagiert mit den Produktionslinien und kann Steine aufnehmen und absetzen.
- **Kundenaufträge:** Aufträge bestehen aus acht zufällig ausgewählten Steinen.
- **Logistikplanung:** Die Logistik bestimmt den nächsten benötigten Stein und die passende Abholstation.
- **Qualitätskontrolle:** Die Steine durchlaufen eine zufallsbasierte Qualitätskontrolle.
- **Recycling:** Zurückgewiesene Steine werden zur Recyclinglinie transportiert und können erneut abgeholt werden.
- **Grafische Simulation:** Die Avalonia-Benutzeroberfläche visualisiert Produktionslinien, Roboter, LEGO-Steine, Transportabläufe und den Auftragsfortschritt.

## Architektur

Die Anwendung folgt einer objektorientierten Architektur mit klar getrennten Verantwortlichkeiten.

| Komponente | Verantwortung |
|---|---|
| `MainWindow` | Darstellung der grafischen Benutzeroberfläche und Weiterleitung von Benutzereingaben. |
| `MainViewModel` | Verbindung zwischen Benutzeroberfläche und Anwendungslogik sowie Verwaltung der Darstellungszustände. |
| `LegoBrickViewModel` | Bereitstellung der Darstellungsdaten einzelner LEGO-Steine. |
| `Factory` | Koordination der Produktionslinien, des Roboters, des Transports und der Fabrikabläufe. |
| `Logistics` | Verwaltung von Aufträgen, Lieferfortschritt und Auswahl der Abholstation. |
| `ProductionLine` | Erzeugung und Bewegung der LEGO-Steine auf den Produktionslinien. |
| `Robot` | Spezifische Bewegungs- und Positionierungslogik des Roboters. |
| `LegoBrick` | Repräsentation eines Steins einschließlich seiner Eigenschaften, seines Zustands und seines Transportverhaltens. |
| `Beweger` | Gemeinsame Bewegungs-, Rotations-, Kollisions- und Dockingfunktionen. |
| `Pose` | Speicherung der Position und Ausrichtung eines Objekts. |
| `Parameters` und `ViewParameters` | Zentrale Anwendungswerte und geometrische Einstellungen. |

## Technologien

- C#
- .NET 10
- Avalonia UI
- CommunityToolkit.Mvvm
- XAML

Die grafische Benutzeroberfläche folgt dem MVVM-Pattern (Model-View-ViewModel). Dadurch werden Darstellung, Anwendungssteuerung und fachliche Logik voneinander getrennt.

## Eigenständige praktische Prüfungsaufgabe (nach Lab 5)

Nach Abschluss der fünf Laboraufgaben wurde eine separate praktische Prüfungsaufgabe als eigenständige Erweiterung umgesetzt. Sie ist nicht Bestandteil der ursprünglichen fünf Labore. Die Umsetzung erfolgte eigenständig, ohne KI-Unterstützung und ohne Internetzugriff.

### LEGO-Steine auf den Produktionslinien shufflen

Die Erweiterung ergänzt die Fabrik um eine Funktion zur zufälligen Neuverteilung der Farben über alle vier Produktionslinien.

Dabei gelten folgende Eigenschaften:

- Die vorhandenen Farben werden zufällig über alle vier Produktionslinien neu verteilt.
- Die Gesamtanzahl der Steine je Farbe bleibt unverändert.
- Die physischen Positionen der Steine bleiben erhalten.
- Steine, die sich außerhalb der vier Produktionslinien befinden, behalten ihre Farbe.
- Die Logistikanweisung und die Abholstationsauswahl berücksichtigen die aktuelle Farbverteilung. Die ausgewählte Station muss tatsächlich einen verfügbaren Stein der benötigten Farbe enthalten.
- Der Shuffle-Button wird nach dem Einrichten der Produktionslinien aktiviert.
- Eine Statusmeldung informiert über die Anzahl der geshuffelten Steine.

Die Erweiterung baut auf der bestehenden Architektur auf und betrifft insbesondere die Komponenten `Factory`, `Logistics` und `LegoBrick` sowie die ViewModels und die grafische Benutzeroberfläche.

## Voraussetzungen

- .NET 10 SDK
- Visual Studio, Visual Studio Code oder eine andere kompatible .NET-Entwicklungsumgebung

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

---

**Hochschule:** TH Köln<br>
**Modul:** Softwaretechnik<br>
**Projekt:** SmartFactory