using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Models;

public class Logistics
{
    // Alle möglichen Brick-Farben.
    private readonly List<Parameters.LegoColor> farben = new()
    {
        Parameters.LegoColor.red,
        Parameters.LegoColor.green,
        Parameters.LegoColor.blue,
        Parameters.LegoColor.yellow
    };

    // Anzahl der erfolgreich gelieferten Bricks je Farbe.
    private readonly Dictionary<Parameters.LegoColor, int> delivered = new();

    // Zufallsgenerator für Auftrag und Qualitätsprüfung.
    private readonly Random random = new();

    // Index des aktuellen Auftragsschritts.
    private int naechsterSchritt = 0;

    // Gibt an, ob der aktuelle Brick bereits aufgenommen wurde.
    private bool brickPickedUp = false;

    // Nach einer Ablehnung muss der Brick vom Recycling kommen.
    private bool recycleRequired = false;

    // Referenz auf den zuletzt abgelehnten Brick.
    private LegoBrick? rejectedBrick;

    // Der Auftrag besteht aus acht zufällig ausgewählten Bricks.
    // Mehrfachauswahl ist erlaubt.
    public List<LegoBrick> Order { get; private set; } = new();

    public bool HasOrder => Order.Count > 0;

    public bool IsComplete =>
        HasOrder && naechsterSchritt >= Order.Count;

    public bool IsBrickPickedUp => brickPickedUp;

    // Brick, zu dem der Roboter für den nächsten Aufnahmeversuch fahren soll.
    public LegoBrick? PickupBrick =>
        recycleRequired ? rejectedBrick : CurrentStep();

    public Logistics()
    {
        // Lieferzähler für alle Farben auf 0 setzen.
        foreach (Parameters.LegoColor farbe in farben)
        {
            delivered[farbe] = 0;
        }
    }

    // Erstellt einen zufälligen Auftrag aus den vorhandenen Bricks.
    public void CreateRandomOrder(List<LegoBrick> alleBricks)
    {
        if (alleBricks == null || alleBricks.Count == 0)
            throw new InvalidOperationException(
                "Es sind noch keine Produktionssteine vorhanden.");

        Order.Clear();

        naechsterSchritt = 0;
        brickPickedUp = false;
        recycleRequired = false;
        rejectedBrick = null;

        foreach (Parameters.LegoColor farbe in farben)
        {
            delivered[farbe] = 0;
        }

        // Acht unabhängige Zufallsauswahlen.
        // Derselbe Brick kann mehrfach vorkommen.
        for (int i = 0; i < Parameters.orderSize; i++)
        {
            int index = random.Next(alleBricks.Count);
            Order.Add(alleBricks[index]);
        }
    }

    // Liefert den Brick des aktuellen Auftragsschritts.
    public LegoBrick? CurrentStep()
    {
        if (IsComplete || !HasOrder)
            return null;

        return Order[naechsterSchritt];
    }

    // Bestimmt die Produktionslinie des aktuellen Schritts.
    public ProductionLine? PickStation(
        List<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null)
            return null;

        // Nach einer Ablehnung immer Recycling verwenden.
        if (recycleRequired)
            return recycleLine;

        // Ursprüngliche Produktionslinie des Auftragssteins finden.
        foreach (ProductionLine line in productionLines)
        {
            if (line.Bricks.Contains(aktuellerBrick))
                return line;
        }

        return null;
    }

    // Prüft, ob ein Brick für den aktuellen Auftragsschritt erlaubt ist.
    public bool IsNeeded(
        LegoBrick brick,
        ProductionLine source,
        List<ProductionLine> productionLines,
        ProductionLine recycleLine)
    {
        if (!HasOrder || IsComplete || brickPickedUp)
            return false;

        if (brick.Status != Parameters.BrickStatus.Moving)
            return false;

        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null || brick.Color != aktuellerBrick.Color)
            return false;

        // Nach einer Ablehnung darf ausschließlich der abgelehnte
        // Brick von der Recycling Line aufgenommen werden.
        if (recycleRequired)
        {
            return source == recycleLine
                && brick == rejectedBrick
                && brick.RecyclePlatz >= 0;
        }

        // Im normalen Auftragsschritt ist nur die vorgesehene
        // Produktionslinie erlaubt.
        if (source != PickStation(productionLines, recycleLine))
            return false;

        return source.Bricks.Contains(brick)
            && brick.RecyclePlatz < 0;
    }

    // Wird nach erfolgreicher Aufnahme durch den Roboter aufgerufen.
    public void BrickPickedUp()
    {
        brickPickedUp = true;
    }

    // Echte 50-%-Zufallsentscheidung für die Qualitätsprüfung.
    public bool ShouldRecycle()
    {
        return random.NextDouble() < 0.5;
    }

    // Wird aufgerufen, wenn ein Brick abgelehnt wurde.
    public void BrickRejected(LegoBrick brick)
    {
        recycleRequired = true;
        rejectedBrick = brick;
        brickPickedUp = false;
    }

    // Wird erst nach erfolgreicher Kundenlieferung aufgerufen.
    public void BrickDelivered()
    {
        LegoBrick? aktuellerBrick = CurrentStep();

        if (aktuellerBrick == null)
            return;

        delivered[aktuellerBrick.Color]++;

        naechsterSchritt++;

        // Der abgeschlossene Schritt ist vollständig beendet.
        brickPickedUp = false;
        recycleRequired = false;
        rejectedBrick = null;
    }

    // Anzahl erfolgreich gelieferter Bricks einer Farbe.
    public int GetDelivered(Parameters.LegoColor color)
    {
        return delivered[color];
    }

    // Anzahl bestellter Bricks einer Farbe.
    public int GetOrder(Parameters.LegoColor color)
    {
        int count = 0;

        foreach (LegoBrick brick in Order)
        {
            if (brick.Color == color)
                count++;
        }

        return count;
    }

    // Gesamtzahl aller erfolgreichen Lieferungen.
    public int GetDelivered()
    {
        int total = 0;

        foreach (int count in delivered.Values)
        {
            total += count;
        }

        return total;
    }

    // Nummer des aktuellen Auftragsschritts.
    public int CurrentStepNumber()
    {
        return IsComplete ? Order.Count : naechsterSchritt + 1;
    }

    // Wandelt eine Farbe in einen Buchstaben um.
    public static char ToChar(Parameters.LegoColor color)
    {
        return color switch
        {
            Parameters.LegoColor.red => 'R',
            Parameters.LegoColor.green => 'G',
            Parameters.LegoColor.blue => 'B',
            Parameters.LegoColor.yellow => 'Y',
            _ => '?'
        };
    }

    // Wandelt einen Buchstaben in eine Farbe um.
    public static Parameters.LegoColor ToColor(char color)
    {
        return char.ToUpperInvariant(color) switch
        {
            'R' => Parameters.LegoColor.red,
            'G' => Parameters.LegoColor.green,
            'B' => Parameters.LegoColor.blue,
            'Y' => Parameters.LegoColor.yellow,
            _ => throw new ArgumentException("Unbekannte Brick-Farbe.")
        };
    }
}