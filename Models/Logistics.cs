using System;
using System.Collections.Generic;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

// Plant den Kundenauftrag und verwaltet den Lieferfortschritt.
public class Logistics
{
    private readonly LegoColor[] farben =
    {
        LegoColor.red,
        LegoColor.green,
        LegoColor.blue,
        LegoColor.yellow
    };

    private readonly Dictionary<LegoColor, int> delivered = new();

    private readonly Random random = new();

    private int naechsterSchritt;

    private bool brickPickedUp;
    private bool recycleRequired;

    private LegoBrick? rejectedBrick;

    // Die Bestellung enthält Referenzen auf ausgewählte Steine.
    // Mehrfachauswahl ist erlaubt.
    public List<LegoBrick> Order { get; private set; } = new();

    public bool HasOrder => Order.Count > 0;

    public bool IsComplete =>
        HasOrder && naechsterSchritt >= Order.Count;

    public bool IsBrickPickedUp => brickPickedUp;

    public bool IsRecyclingRequired => recycleRequired;

    // Nach einer Ablehnung wird derselbe Stein erneut benötigt.
    public LegoBrick? PickupBrick =>
        recycleRequired ? rejectedBrick : CurrentStep();

    public Logistics()
    {
        ResetDeliveryCounters();
    }

    // Erstellt einen zufälligen Auftrag.
    public void CreateRandomOrder(List<LegoBrick> alleBricks)
    {
        if (alleBricks == null || alleBricks.Count == 0)
        {
            throw new InvalidOperationException(
                "Es sind noch keine Produktionssteine vorhanden.");
        }

        Order.Clear();

        naechsterSchritt = 0;
        brickPickedUp = false;
        recycleRequired = false;

        rejectedBrick = null;

        ResetDeliveryCounters();

        // Acht unabhängige Ziehungen mit Zurücklegen.
        for (int i = 0; i < Parameters.orderSize; i++)
        {
            int index = random.Next(alleBricks.Count);

            Order.Add(alleBricks[index]);
        }
    }

    // Gibt den Stein des aktuellen Auftragsschritts zurück.
    public LegoBrick? CurrentStep()
    {
        if (!HasOrder || IsComplete)
            return null;

        return Order[naechsterSchritt];
    }

    // Bestimmt die Linie des nächsten Steins.
    public ProductionLine? PickStation(
        List<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        if (recycleRequired)
            return recycleLine;

        LegoBrick? currentBrick = CurrentStep();

        if (currentBrick == null)
            return null;

        foreach (ProductionLine line in productionLines)
        {
            if (line.Bricks.Contains(currentBrick))
                return line;
        }

        return null;
    }

    // Prüft, ob ein Stein zum aktuellen Auftragsschritt passt.
    public bool IsNeeded(
        LegoBrick brick,
        ProductionLine source,
        List<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        if (!HasOrder || IsComplete || brickPickedUp)
            return false;

        if (brick.Status != BrickStatus.Moving)
            return false;

        LegoBrick? currentBrick = CurrentStep();

        if (currentBrick == null ||
            brick.Color != currentBrick.Color)
        {
            return false;
        }

        // Nach einer Ablehnung ist nur der abgelehnte Stein erlaubt.
        if (recycleRequired)
        {
            return source == recycleLine &&
                   brick == rejectedBrick &&
                   brick.RecyclePlatz >= 0;
        }

        ProductionLine? requiredStation =
            PickStation(productionLines, recycleLine);

        return source == requiredStation &&
               source.Bricks.Contains(brick) &&
               brick.RecyclePlatz < 0;
    }

    // Wird aufgerufen, wenn der Roboter einen Stein aufgenommen hat.
    public void BrickPickedUp()
    {
        brickPickedUp = true;
    }

    // 50 % Ablehnungswahrscheinlichkeit für jeden Prüfversuch.
    public bool ShouldRecycle()
    {
        return random.NextDouble() < 0.5;
    }

    // Merkt sich einen abgelehnten Stein.
    public void BrickRejected(LegoBrick brick)
    {
        recycleRequired = true;
        rejectedBrick = brick;

        brickPickedUp = false;
    }

    // Schließt einen erfolgreich gelieferten Auftragsschritt ab.
    public void BrickDelivered()
    {
        LegoBrick? currentBrick = CurrentStep();

        if (currentBrick == null)
            return;

        delivered[currentBrick.Color]++;

        naechsterSchritt++;

        brickPickedUp = false;
        recycleRequired = false;

        rejectedBrick = null;
    }

    public int GetDelivered(LegoColor color)
    {
        return delivered[color];
    }

    public int GetOrder(LegoColor color)
    {
        int count = 0;

        foreach (LegoBrick brick in Order)
        {
            if (brick.Color == color)
                count++;
        }

        return count;
    }

    public int GetDelivered()
    {
        int total = 0;

        foreach (int count in delivered.Values)
        {
            total += count;
        }

        return total;
    }

    // Die Anzeige beginnt bei Schritt 1.
    public int CurrentStepNumber()
    {
        if (!HasOrder)
            return 0;

        return IsComplete
            ? Order.Count
            : naechsterSchritt + 1;
    }

    public static char ToChar(LegoColor color)
    {
        return color switch
        {
            LegoColor.red => 'R',
            LegoColor.green => 'G',
            LegoColor.blue => 'B',
            LegoColor.yellow => 'Y',
            _ => '?'
        };
    }

    public static LegoColor ToColor(char color)
    {
        return char.ToUpperInvariant(color) switch
        {
            'R' => LegoColor.red,
            'G' => LegoColor.green,
            'B' => LegoColor.blue,
            'Y' => LegoColor.yellow,
            _ => throw new ArgumentException(
                "Unbekannte Brick-Farbe.")
        };
    }

    private void ResetDeliveryCounters()
    {
        delivered.Clear();

        foreach (LegoColor color in farben)
        {
            delivered[color] = 0;
        }
    }
}