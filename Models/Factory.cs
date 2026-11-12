using System;
using System.Collections.Generic;

namespace MyAvaloniaApp.Models;


// Koordiniert Roboter, Produktionslinien, LEGO-Steine
// und den Transport innerhalb der SmartFactory.
public class Factory
{
    // --------------------------------------------------------
    // Felder
    // --------------------------------------------------------

    private readonly Robot robot;

    // Die vier normalen Produktionslinien.
    private readonly ProductionLine productionBlue =
        new(ViewParameters.RectBluePose);

    private readonly ProductionLine productionGreen =
        new(ViewParameters.RectGreenPose);

    private readonly ProductionLine productionRed =
        new(ViewParameters.RectRedPose);

    private readonly ProductionLine productionYellow =
        new(ViewParameters.RectYellowPose);

    // Zusätzliche Linien für Recycling und Auslieferung.
    private readonly ProductionLine productionRecycle =
        new(ViewParameters.RectRecyclePose);

    private readonly ProductionLine productionDelivery =
        new(ViewParameters.RectDeliveryPose);

    // Verwaltet Kundenaufträge und Lieferfortschritte.
    private readonly Logistics logistics = new();

    // Steine, die gerade zum Kunden oder Recycling transportiert werden.
    private readonly List<LegoBrick> transportBricks = new();

    // Gemeinsame Liste der vier normalen Produktionslinien.
    private readonly List<ProductionLine> productionLines;

    // Abstand zwischen zwei Recyclingplätzen.
    private const int RecyclePlatzAbstand = 24;

    // Abstand des ersten Recyclingplatzes vom Linienanfang.
    private const int RecycleStartOffset = 25;


    // --------------------------------------------------------
    // Eigenschaften
    // --------------------------------------------------------

    // LEGO-Steine der vier Produktionslinien.
    public IReadOnlyList<LegoBrick> BlueBricks =>
        productionBlue.Bricks;

    public IReadOnlyList<LegoBrick> GreenBricks =>
        productionGreen.Bricks;

    public IReadOnlyList<LegoBrick> RedBricks =>
        productionRed.Bricks;

    public IReadOnlyList<LegoBrick> YellowBricks =>
        productionYellow.Bricks;


    // Aktuelle Kundenbestellung.
    public IReadOnlyList<LegoBrick> LegoOrder =>
        logistics.Order;


    // Aktueller Auftragsschritt.
    public int LogisticsStep =>
        logistics.CurrentStepNumber();


    // Aktuelle Position und Ausrichtung des Roboters.
    public double RobotX => robot.X;

    public double RobotY => robot.Y;

    public double RobotAngle => robot.Angle;


    // Zugriff auf die Produktionslinien.
    // Wird unter anderem vom MainViewModel benötigt.
    public ProductionLine ProductionLineBlue =>
        productionBlue;

    public ProductionLine ProductionLineGreen =>
        productionGreen;

    public ProductionLine ProductionLineRed =>
        productionRed;

    public ProductionLine ProductionLineYellow =>
        productionYellow;

    public ProductionLine ProductionLineRecycle =>
        productionRecycle;

    public ProductionLine ProductionLineDelivery =>
        productionDelivery;


    // Informationen zum aktuellen Auftrag.
    public bool OrderComplete =>
        logistics.IsComplete;

    public bool HasOrder =>
        logistics.HasOrder;

    public LegoColor? CurrentBrickColor =>
        logistics.CurrentStep()?.Color;

    public ProductionLine? CurrentStation =>
        logistics.PickStation(productionLines, productionRecycle);

    public bool IsBrickPickedUp =>
        logistics.IsBrickPickedUp;

    public bool IsRecyclingRequired =>
        logistics.IsRecyclingRequired;

    public int OrderCount =>
        logistics.Order.Count;


    // --------------------------------------------------------
    // Ereignisse
    // --------------------------------------------------------

    // Informiert das MainViewModel über eine erfolgreiche Lieferung.
    public event Action<LegoColor>? BrickDelivered;

    // Übermittelt Statusmeldungen an das MainViewModel.
    public event Action<string>? TransportMessage;


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public Factory()
    {
        // Die vier normalen Produktionslinien zusammenfassen.
        productionLines = new List<ProductionLine>
        {
            productionRed,
            productionGreen,
            productionBlue,
            productionYellow
        };

        // Alle sechs Linien werden bei der Roboterbewegung
        // als Hindernisse und Dockstationen berücksichtigt.
        List<ProductionLine> allLines = new(productionLines)
        {
            productionRecycle,
            productionDelivery
        };

        // Roboter mit Startposition, Fabrikgrenzen
        // und den vorhandenen Linien erstellen.
        robot = new Robot(
            ViewParameters.RobotPose,
            ViewParameters.Borders,
            allLines);
    }


    // --------------------------------------------------------
    // Roboterbewegung
    // --------------------------------------------------------

    // Leitet eine Bewegungsanweisung an den Roboter weiter.
    // Anschließend werden mögliche Steinkontakte geprüft.
    public void RobotBewegen(Direction direction)
    {
        robot.Bewegen(direction);

        PruefeSteinkontakte();
    }


    // --------------------------------------------------------
    // Produktionslinien und Transport bewegen
    // --------------------------------------------------------

    // Bewegt die Produktionslinien und transportiert
    // abgesetzte LEGO-Steine zum Kunden oder Recycling.
    //
    // Diese Methode wird regelmäßig vom Timer aufgerufen.
    public void LegoBricksBewegen()
    {
        // Förderbewegung der vier Produktionslinien.
        productionRed.Bewegen(-1);
        productionBlue.Bewegen(-1);
        productionYellow.Bewegen(1);
        productionGreen.Bewegen(-1);


        // Grenzen der Delivery Line bestimmen.
        int deliveryLeft =
            ViewParameters.RectDeliveryPose.X;

        int deliveryRight =
            deliveryLeft + ViewParameters.ProductionDeliveryWidth;


        // Eine Kopie verwenden, weil Steine während der Verarbeitung
        // aus der Transportliste entfernt werden können.
        foreach (LegoBrick brick in new List<LegoBrick>(transportBricks))
        {
            switch (brick.Status)
            {
                case BrickStatus.ToCustomer:
                    BewegeSteinZumKunden(brick, deliveryRight);
                    break;

                case BrickStatus.ToRecycle:
                    BewegeSteinZumRecycling(brick, deliveryLeft);
                    break;
            }
        }

        // Nach der Bewegung erneut prüfen, ob der Roboter
        // einen passenden Stein erreichen kann.
        PruefeSteinkontakte();
    }


    // --------------------------------------------------------
    // Transport zum Kunden
    // --------------------------------------------------------

    // Bewegt einen akzeptierten Stein zum Kunden.
    private void BewegeSteinZumKunden(
        LegoBrick brick,
        int deliveryRight)
    {
        // Der Stein muss zuerst die Delivery Line verlassen.
        if (brick.Location.X < deliveryRight)
        {
            brick.BewegenZumKunden();
            return;
        }

        // Die Lieferung erst abschließen, wenn der Stein
        // seine ursprüngliche Bewegungsphase wieder erreicht hat.
        if (!brick.VersatzIstNull)
            return;

        LegoColor color = brick.Color;

        // Stein auf seine ursprüngliche Produktionsposition zurücksetzen.
        brick.ZurueckZurProduktionslinie();

        transportBricks.Remove(brick);

        // Auftragsschritt in der Logistik abschließen.
        logistics.BrickDelivered();

        // MainViewModel über die erfolgreiche Lieferung informieren.
        BrickDelivered?.Invoke(color);

        TransportMessage?.Invoke(
            $"{color} brick successfully delivered to customer.");
    }


    // --------------------------------------------------------
    // Transport zum Recycling
    // --------------------------------------------------------

    // Bewegt einen abgelehnten Stein zur Recyclinglinie.
    private void BewegeSteinZumRecycling(
        LegoBrick brick,
        int deliveryLeft)
    {
        // Breite des Steins bestimmen.
        int brickWidth = brick.Size == LegoSize.large
            ? ViewParameters.BigBrickWidth
            : ViewParameters.SmallBrickWidth;

        // Der Stein muss die Delivery Line vollständig verlassen.
        if (brick.Location.X + brickWidth > deliveryLeft)
        {
            brick.BewegenZumRecycling();
            return;
        }

        // Einen freien Recyclingplatz suchen.
        int freePlace = FreierRecyclePlatz();

        // Sind alle vier Plätze belegt, muss der Stein warten.
        if (freePlace < 0)
            return;

        // Stein auf dem freien Recyclingplatz ablegen.
        brick.AufRecyclePlatz(
            RecyclePlatzPose(freePlace),
            freePlace);

        TransportMessage?.Invoke(
            $"{brick.Color} brick placed on recycle position {freePlace + 1}.");
    }


    // --------------------------------------------------------
    // Undocken und Qualitätsprüfung
    // --------------------------------------------------------

    // Löst einen aufgenommenen Stein auf der Delivery Line.
    public UndockResult Undock()
    {
        // Prüfen, ob der Roboter einen LEGO-Stein trägt.
        if (robot.Dockobjekt is not LegoBrick brick)
            return UndockResult.NoBrick;

        // Undocken ist ausschließlich auf der Delivery Line erlaubt.
        if (!robot.AufDeliveryLinie())
            return UndockResult.NotOnDeliveryLine;

        // Stein vom Roboter lösen.
        LegoBrick? abgesetzterBrick = robot.Undock();

        if (abgesetzterBrick == null)
            return UndockResult.NoBrick;

        // Stein für den anschließenden Transport ausrichten.
        abgesetzterBrick.Angle = 0;

        // Qualitätsprüfung mit 50 % Ablehnungswahrscheinlichkeit.
        if (logistics.ShouldRecycle())
        {
            // Abgelehnten Stein zur Recyclinglinie schicken.
            abgesetzterBrick.ZumRecycling();

            // Die Logistik speichert genau diesen Stein.
            // Der aktuelle Auftragsschritt bleibt unverändert.
            logistics.BrickRejected(abgesetzterBrick);

            TransportMessage?.Invoke(
                "Brick rejected: sent to recycle line.");
        }
        else
        {
            // Akzeptierten Stein zum Kunden transportieren.
            abgesetzterBrick.ZumKunden();

            TransportMessage?.Invoke(
                "Brick sent to customer.");
        }

        // Stein zur Transportliste hinzufügen.
        transportBricks.Add(abgesetzterBrick);

        return UndockResult.Success;
    }


    // --------------------------------------------------------
    // LEGO-Steine aufnehmen
    // --------------------------------------------------------

    // Prüft, ob der Roboter einen benötigten Stein erreichen kann.
    private void PruefeSteinkontakte()
    {
        // Ohne Auftrag, nach Abschluss oder bei bereits
        // aufgenommenem Stein ist keine weitere Aufnahme möglich.
        if (!logistics.HasOrder ||
            logistics.IsComplete ||
            logistics.IsBrickPickedUp)
        {
            return;
        }

        // Zuerst die normalen Produktionslinien prüfen.
        foreach (ProductionLine line in productionLines)
        {
            if (VersucheSteinAufzunehmen(line.Bricks, line))
                return;
        }

        // Anschließend die Recyclinglinie prüfen.
        // Dort befindet sich gegebenenfalls der abgelehnte Stein.
        VersucheSteinAufzunehmen(
            transportBricks,
            productionRecycle);
    }


    // Versucht, einen passenden Stein aus einer Quelle aufzunehmen.
    private bool VersucheSteinAufzunehmen(
        IReadOnlyList<LegoBrick> verfuegbareSteine,
        ProductionLine quelle)
    {
        // Eine Kopie verwenden, damit die Transportliste
        // während der Verarbeitung verändert werden kann.
        foreach (LegoBrick brick in new List<LegoBrick>(verfuegbareSteine))
        {
            // Die Logistik prüft Farbe, Quelle und Auftragsstatus.
            if (!logistics.IsNeeded(
                brick,
                quelle,
                productionLines,
                productionRecycle))
            {
                continue;
            }

            // Prüfen, ob der Roboter den Stein tatsächlich greifen kann.
            if (!robot.Kontakt(brick))
                continue;

            // Erfolgreiche Aufnahme in der Logistik registrieren.
            logistics.BrickPickedUp();

            // Ein Stein aus dem Recycling wird aus der Transportliste
            // entfernt, sobald er wieder vom Roboter getragen wird.
            if (quelle == productionRecycle)
                transportBricks.Remove(brick);

            return true;
        }

        return false;
    }


    // --------------------------------------------------------
    // Recyclingplätze
    // --------------------------------------------------------

    // Sucht den ersten freien Recyclingplatz.
    //
    // Gibt -1 zurück, wenn alle vier Plätze belegt sind.
    private int FreierRecyclePlatz()
    {
        for (int place = 0;
             place < Parameters.bricksPerLine;
             place++)
        {
            bool belegt = false;

            foreach (LegoBrick brick in transportBricks)
            {
                if (brick.RecyclePlatz == place)
                {
                    belegt = true;
                    break;
                }
            }

            if (!belegt)
                return place;
        }

        return -1;
    }


    // Berechnet die Position eines bestimmten Recyclingplatzes.
    private Pose RecyclePlatzPose(int place)
    {
        if (place < 0 || place >= Parameters.bricksPerLine)
            throw new ArgumentOutOfRangeException(nameof(place));

        int x =
            ViewParameters.RectRecyclePose.X
            + RecycleStartOffset
            + place * RecyclePlatzAbstand;

        int y =
            ViewParameters.RectRecyclePose.Y
            - ViewParameters.SmallBrickHeight;

        return new Pose(x, y, 0);
    }


    // --------------------------------------------------------
    // Produktionslinien und Steine
    // --------------------------------------------------------

    // Erzeugt vier Steine auf jeder normalen Produktionslinie.
    public void SetupProductionLines()
    {
        foreach (ProductionLine line in productionLines)
        {
            line.CreateBricks();
        }
    }


    // Gibt alle Steine der vier normalen Produktionslinien zurück.
    public List<LegoBrick> AlleBricks()
    {
        List<LegoBrick> alleSteine = new();

        alleSteine.AddRange(productionRed.Bricks);
        alleSteine.AddRange(productionGreen.Bricks);
        alleSteine.AddRange(productionBlue.Bricks);
        alleSteine.AddRange(productionYellow.Bricks);

        return alleSteine;
    }


    // --------------------------------------------------------
    // Bestellung
    // --------------------------------------------------------

    // Erstellt einen zufälligen Kundenauftrag.
    public void CreateOrder()
    {
        logistics.CreateRandomOrder(AlleBricks());
    }


    // Gibt die Anzahl der bestellten Steine einer Farbe zurück.
    public int GetOrder(LegoColor color)
    {
        return logistics.GetOrder(color);
    }


    // Gibt die Anzahl der erfolgreich gelieferten Steine einer Farbe zurück.
    public int GetDelivered(LegoColor color)
    {
        return logistics.GetDelivered(color);
    }


    // --------------------------------------------------------
    // Roboter zur Produktionslinie bewegen
    // --------------------------------------------------------

    // Positioniert den Roboter vor der Station des nächsten Auftragsschritts.
    public bool GoToLine()
    {
        // Ein aktiver Auftrag muss vorhanden sein.
        // Der Auftrag darf nicht abgeschlossen sein.
        // Der Roboter darf keinen Stein tragen.
        if (!logistics.HasOrder ||
            logistics.IsComplete ||
            logistics.IsBrickPickedUp)
        {
            return false;
        }

        // Benötigte Station und Stein ermitteln.
        ProductionLine? station =
            logistics.PickStation(productionLines, productionRecycle);

        LegoBrick? brick = logistics.PickupBrick;

        if (station == null || brick == null)
            return false;

        // Ein Recyclingstein muss zuerst einen Recyclingplatz erreichen.
        if (station == productionRecycle && brick.RecyclePlatz < 0)
        {
            TransportMessage?.Invoke(
                "Please wait until the brick reaches the recycle line.");

            return false;
        }

        // Passenden Anfahrwinkel bestimmen.
        double targetAngle = ErmittleAnfahrwinkel(station);

        // Die normale Produktionslinie und Recyclinglinie
        // besitzen dieselben Abmessungen.
        robot.GeheZuLinie(
            station.Pose,
            ViewParameters.ProductionLineWidth,
            ViewParameters.ProductionLineHeight,
            targetAngle);

        TransportMessage?.Invoke(
            "Robot at the line: drive forward to dock the brick.");

        return true;
    }


    // Ermittelt den passenden Anfahrwinkel für eine Produktionslinie.
    private double ErmittleAnfahrwinkel(ProductionLine station)
    {
        if (station == productionRed)
            return ViewParameters.GoToRedAngle;

        if (station == productionBlue)
            return ViewParameters.GoToBlueAngle;

        if (station == productionGreen)
            return ViewParameters.GoToGreenAngle;

        if (station == productionYellow)
            return ViewParameters.GoToYellowAngle;

        return ViewParameters.GoToRecycleAngle;
    }


    // --------------------------------------------------------
    // Roboter zur Delivery Line bewegen
    // --------------------------------------------------------

    // Positioniert den Roboter vor der Delivery Line.
    public bool GoToDelivery()
    {
        // Der Roboter muss einen LEGO-Stein tragen.
        if (robot.Dockobjekt is not LegoBrick ||
            !logistics.IsBrickPickedUp)
        {
            return false;
        }

        robot.GeheZuLinie(
            productionDelivery.Pose,
            ViewParameters.ProductionDeliveryWidth,
            ViewParameters.ProductionDeliveryHeight,
            ViewParameters.GoToDeliveryAngle);

        TransportMessage?.Invoke(
            "Robot at delivery: drive forward, then click undock.");

        return true;
    }
}