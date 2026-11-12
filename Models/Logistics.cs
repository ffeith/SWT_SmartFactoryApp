using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Models;


// Verwaltet Kundenbestellungen, Qualitätsprüfungen
// und den Fortschritt der Auslieferung.
public class Logistics
{
    // --------------------------------------------------------
    // Felder
    // --------------------------------------------------------

    // Alle verfügbaren LEGO-Farben.
    private readonly LegoColor[] farben =
    {
        LegoColor.red,
        LegoColor.green,
        LegoColor.blue,
        LegoColor.yellow
    };

    // Anzahl der erfolgreich gelieferten Steine je Farbe.
    private readonly Dictionary<LegoColor, int> delivered = new();

    // Zufallsgenerator für Bestellung und Qualitätsprüfung.
    private readonly Random random = new();

    // Position des nächsten Steins innerhalb der Bestellung.
    private int naechsterSchritt;

    // Gibt an, ob der Roboter aktuell einen Stein trägt.
    private bool brickPickedUp;

    // Gibt an, ob der aktuelle Stein zuerst recycelt werden muss.
    private bool recycleRequired;

    // Referenz auf den zuletzt abgelehnten Stein.
    private LegoBrick? rejectedBrick;

    // Enthält die zufällig ausgewählten Steine des Auftrags.
    private readonly List<LegoBrick> order = new();


    // --------------------------------------------------------
    // Eigenschaften
    // --------------------------------------------------------

    public IReadOnlyList<LegoBrick> Order => order;

    public bool HasOrder => order.Count > 0;

    public bool IsComplete =>
        HasOrder && naechsterSchritt >= order.Count;

    public bool IsBrickPickedUp => brickPickedUp;

    public bool IsRecyclingRequired => recycleRequired;

    // Nach einer Ablehnung muss derselbe Stein erneut aufgenommen werden.
    public LegoBrick? PickupBrick =>
        recycleRequired ? rejectedBrick : CurrentStep();


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public Logistics()
    {
        ResetDeliveryCounters();
    }


    // --------------------------------------------------------
    // Bestellung erstellen
    // --------------------------------------------------------

    // Erstellt einen zufälligen Auftrag aus den vorhandenen Steinen.
    public void CreateRandomOrder(IReadOnlyList<LegoBrick> alleBricks)
    {
        if (alleBricks == null)
            throw new ArgumentNullException(nameof(alleBricks));

        if (alleBricks.Count == 0)
        {
            throw new InvalidOperationException(
                "Es sind noch keine Produktionssteine vorhanden.");
        }

        order.Clear();

        // Zustand für den neuen Auftrag zurücksetzen.
        naechsterSchritt = 0;
        brickPickedUp = false;
        recycleRequired = false;
        rejectedBrick = null;

        ResetDeliveryCounters();

        // Acht Steine zufällig auswählen.
        // Da die ausgewählten Steine nicht entfernt werden,
        // sind Mehrfachauswahlen möglich.
        for (int i = 0; i < Parameters.orderSize; i++)
        {
            int index = random.Next(alleBricks.Count);
            order.Add(alleBricks[index]);
        }
    }


    // --------------------------------------------------------
    // Aktuellen Auftragsschritt bestimmen
    // --------------------------------------------------------

    public LegoBrick? CurrentStep()
    {
        if (!HasOrder || IsComplete)
            return null;

        return order[naechsterSchritt];
    }


    // Bestimmt die Produktionslinie des nächsten benötigten Steins.
    public ProductionLine? PickStation(
        IReadOnlyList<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        if (recycleRequired)
            return recycleLine;

        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null)
            return null;

        foreach (ProductionLine line in productionLines)
        {
            if (EnthaeltStein(line, aktuellerBrick))
                return line;
        }

        return null;
    }


    // --------------------------------------------------------
    // Prüfen, ob ein Stein aufgenommen werden darf
    // --------------------------------------------------------

    public bool IsNeeded(
        LegoBrick brick,
        ProductionLine source,
        IReadOnlyList<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        if (!HasOrder || IsComplete || brickPickedUp)
            return false;

        // Nur bewegliche Steine dürfen aufgenommen werden.
        if (brick.Status != BrickStatus.Moving)
            return false;

        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null)
            return false;

        // Nach einer Ablehnung darf ausschließlich derselbe
        // Stein von der Recyclinglinie aufgenommen werden.
        if (recycleRequired)
        {
            return
                ReferenceEquals(source, recycleLine) &&
                ReferenceEquals(brick, rejectedBrick) &&
                brick.RecyclePlatz >= 0;
        }

        // Im normalen Ablauf müssen Farbe und Produktionslinie stimmen.
        if (brick.Color != aktuellerBrick.Color)
            return false;

        ProductionLine? erforderlicheStation =
            PickStation(productionLines, recycleLine);

        return
            ReferenceEquals(source, erforderlicheStation) &&
            EnthaeltStein(source, brick) &&
            brick.RecyclePlatz < 0;
    }


    // Prüft, ob ein bestimmter Stein auf einer Produktionslinie liegt.
    private static bool EnthaeltStein(
        ProductionLine line,
        LegoBrick brick)
    {
        foreach (LegoBrick vorhandenerBrick in line.Bricks)
        {
            if (ReferenceEquals(vorhandenerBrick, brick))
                return true;
        }

        return false;
    }


    // --------------------------------------------------------
    // Steinaufnahme
    // --------------------------------------------------------

    // Wird aufgerufen, sobald der Roboter einen Stein aufgenommen hat.
    public void BrickPickedUp()
    {
        brickPickedUp = true;
    }


    // --------------------------------------------------------
    // Qualitätsprüfung
    // --------------------------------------------------------

    // Jeder Prüfversuch hat eine Ablehnungswahrscheinlichkeit von 50 %.
    public bool ShouldRecycle()
    {
        return random.NextDouble() < 0.5;
    }


    // Speichert einen abgelehnten Stein.
    // Der aktuelle Auftragsschritt bleibt unverändert.
    public void BrickRejected(LegoBrick brick)
    {
        rejectedBrick = brick;

        recycleRequired = true;
        brickPickedUp = false;
    }


    // --------------------------------------------------------
    // Erfolgreiche Lieferung
    // --------------------------------------------------------

    // Schließt den aktuellen Auftragsschritt ab.
    // Die Factory ruft diese Methode erst nach erfolgreicher
    // Rückkehr des Steins zur ursprünglichen Produktionsposition auf.
    public void BrickDelivered()
    {
        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null)
            return;

        // Lieferzähler der entsprechenden Farbe erhöhen.
        delivered[aktuellerBrick.Color]++;

        // Zum nächsten Auftragsschritt wechseln.
        naechsterSchritt++;

        // Zustand des abgeschlossenen Schritts zurücksetzen.
        brickPickedUp = false;
        recycleRequired = false;
        rejectedBrick = null;
    }


    // --------------------------------------------------------
    // Bestell- und Lieferübersicht
    // --------------------------------------------------------

    public int GetOrder(LegoColor color)
    {
        int count = 0;

        foreach (LegoBrick brick in order)
        {
            if (brick.Color == color)
                count++;
        }

        return count;
    }


    public int GetDelivered(LegoColor color)
    {
        return delivered[color];
    }


    // Gibt die Nummer des aktuellen Auftragsschritts zurück.
    // Die Anzeige beginnt bei Schritt 1.
    public int CurrentStepNumber()
    {
        if (!HasOrder)
            return 0;

        if (IsComplete)
            return order.Count;

        return naechsterSchritt + 1;
    }


    // --------------------------------------------------------
    // Hilfsmethoden
    // --------------------------------------------------------

    // Setzt alle Lieferzähler auf null.
    private void ResetDeliveryCounters()
    {
        delivered.Clear();

        foreach (LegoColor farbe in farben)
        {
            delivered[farbe] = 0;
        }
    }
}