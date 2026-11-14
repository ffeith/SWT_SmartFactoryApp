# SmartFactory – Erweiterung zur praktischen Prüfungsaufgabe

[English](README.md) | [Deutsch](README.de.md)

Dieser Branch enthält eine eigenständige Erweiterung des SmartFactory-Projekts, die nach Abschluss der fünf ursprünglichen Laborübungen an der TH Köln umgesetzt wurde.

Im Mittelpunkt der praktischen Prüfungsaufgabe stand die Erweiterung der bestehenden Fabriksimulation um eine Funktion zur zufälligen Neuverteilung der Farben von LEGO-Steinen auf allen vier Produktionslinien.

Die Aufgabe wurde eigenständig, ohne KI-Unterstützung und ohne Internetzugang, bearbeitet.

## Aufgabenstellung: Neuverteilung der Farben auf den Produktionslinien

Ziel war die Implementierung einer Shuffle-Funktion, die die vorhandenen Farben auf geeignete Steine aller vier Produktionslinien zufällig verteilt, ohne den zugrunde liegenden Fabrikzustand zu verändern.

### Umgesetzte Anforderungen

- **Zufällige Farbverteilung:** Die Farben der Steine werden auf allen vier Produktionslinien zufällig neu zugewiesen.
- **Erhaltung der Farbanzahlen:** Die Gesamtanzahl der Steine je Farbe bleibt unverändert.
- **Unveränderte Positionen:** Die physischen Positionen der Steine bleiben erhalten.
- **Keine Änderungen außerhalb der Produktionslinien:** Steine außerhalb der vier Produktionslinien behalten ihre ursprünglichen Farben.
- **Konsistente Logistik:** Die Auswahl der Abholstation berücksichtigt die aktuelle Farbverteilung. Die ausgewählte Station muss einen verfügbaren Stein der benötigten Farbe enthalten.
- **Abhängigkeit vom Produktions-Setup:** Die Shuffle-Schaltfläche wird erst nach der Einrichtung der Produktion aktiviert.
- **Statusanzeige:** Die Benutzeroberfläche gibt die Anzahl der neu verteilten Steine aus.

## Umsetzung

Die Erweiterung baut auf der bestehenden objektorientierten Architektur auf und umfasst mehrere Anwendungskomponenten:

| Komponente | Aufgabe innerhalb der Erweiterung |
|---|---|
| `Factory` | Identifiziert geeignete Steine und koordiniert die Neuverteilung der Farben. |
| `Logistics` | Ermittelt anhand der aktuellen Verfügbarkeit der benötigten Steinfarbe eine geeignete Abholstation. |
| `LegoBrick` | Speichert die Farbe und weitere Zustandsinformationen eines Steins. |
| `MainViewModel` | Verbindet die Shuffle-Funktion mit der Benutzeroberfläche und verwaltet ihre Verfügbarkeit. |
| `LegoBrickViewModel` | Stellt aktualisierte Darstellungsinformationen für die Steine bereit. |
| `MainWindow` | Stellt die Shuffle-Schaltfläche und die Statusmeldung bereit. |

Bei der Neuverteilung werden die bereits vorhandenen Farbwerte verwendet. Dadurch bleibt die Anzahl der Steine je Farbe erhalten, ohne die physische Anordnung auf den Produktionslinien zu verändern.

## Technologien

- C#
- .NET 10
- Avalonia UI
- XAML
- CommunityToolkit.Mvvm
- MVVM (Model-View-ViewModel)

## Projekt erstellen und starten

Voraussetzungen:
- .NET 10 SDK
- Eine kompatible .NET-Entwicklungsumgebung

Diesen Branch klonen:

```bash
git clone --branch exam-task-shuffle https://github.com/ffeith/SWT_SmartFactoryApp.git
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

## Projektkontext

Diese praktische Prüfungsaufgabe ist eine eigenständige Erweiterung nach Abschluss der ursprünglichen fünf Laborübungen und wird in einem separaten Git-Branch geführt.

Eine vollständige Projektübersicht sowie die Dokumentation der Architektur und des Semesterprojekts befinden sich im Branch [`main`](https://github.com/ffeith/SWT_SmartFactoryApp/blob/main/README.de.md).

---

**Hochschule:** TH Köln · **Modul:** Softwaretechnik · **Projekt:** SmartFactory