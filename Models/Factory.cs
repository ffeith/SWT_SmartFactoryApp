using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;
using Avalonia.Data;
using static MyAvaloniaApp.Models.ViewParameters;
using System.Collections.Generic;
using Avalonia.Controls;
using System;

namespace MyAvaloniaApp.Models;

public class Factory
{
    private Robot robot;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double RobotAngle => robot.Rotation;
    public Direction RobotDirection => robot.RobotDirection;
    public double Rotation => RobotAngle;
    // Die Produktionslinien starten zunächst ohne Steine.
    private ProductionLine productionLineRed = new ProductionLine(ProductionLinePoseRed);

    private ProductionLine productionLineYellow = new ProductionLine(ProductionLinePoseYellow);

    private ProductionLine productionLineBlue = new ProductionLine(ProductionLinePoseBlue);

    private ProductionLine productionLineGreen = new ProductionLine(ProductionLinePoseGreen);
    private ProductionLine productionLineRecycle = new ProductionLine(ProductionLinePoseRecycle);
    /*private ProductionLine productionLineRecycle =
    new ProductionLine(
        ProductionLinePoseRecycle,
        new[]
        {
            Parameters.LegoColor.yellow,
            Parameters.LegoColor.red,
            Parameters.LegoColor.green,
            Parameters.LegoColor.blue
        },
        true);*/

    private ProductionLine productionLineDelivery = new ProductionLine(ProductionLinePoseDelivery);
    public ProductionLine ProductionLineRed => productionLineRed;
    public ProductionLine ProductionLineYellow => productionLineYellow;
    public ProductionLine ProductionLineBlue => productionLineBlue;
    public ProductionLine ProductionLineGreen => productionLineGreen;
    public ProductionLine ProductionLineRecycle => productionLineRecycle;
    public ProductionLine ProductionLineDelivery => productionLineDelivery;
    private double ProductionLineRedX => productionLineRed.ProductionLinePose.X;
    private double ProductionLineRedY => productionLineRed.ProductionLinePose.Y;
    private double ProductionLineRedAngle => productionLineRed.ProductionLinePose.Angle;
    private double ProductionLineYellowX => productionLineYellow.ProductionLinePose.X;
    private double ProductionLineYellowY => productionLineYellow.ProductionLinePose.Y;
    private double ProductionLineYellowAngle => productionLineYellow.ProductionLinePose.Angle;
    private double ProductionLineBlueX => productionLineBlue.ProductionLinePose.X;
    private double ProductionLineBlueY => productionLineBlue.ProductionLinePose.Y;
    private double ProductionLineBlueAngle => productionLineBlue.ProductionLinePose.Angle;
    private double ProductionLineGreenX => productionLineGreen.ProductionLinePose.X;
    private double ProductionLineGreenY => productionLineGreen.ProductionLinePose.Y;
    private double ProductionLineGreenAngle => productionLineGreen.ProductionLinePose.Angle;
    private double ProductionLineRecycleX => productionLineRecycle.ProductionLinePose.X;
    private double ProductionLineRecycleY => productionLineRecycle.ProductionLinePose.Y;
    private double ProductionLineRecycleAngle => productionLineRecycle.ProductionLinePose.Angle;
    private double ProductionLineDeliveryX => productionLineDelivery.ProductionLinePose.X;
    private double ProductionLineDeliveryY => productionLineDelivery.ProductionLinePose.Y;
    private double ProductionLineDeliveryYngle => productionLineDelivery.ProductionLinePose.Angle;
    public List<LegoBrick> RedBricks => productionLineRed.Bricks;
    public List<LegoBrick> BlueBricks => productionLineBlue.Bricks;
    public List<LegoBrick> GreenBricks => productionLineGreen.Bricks;
    public List<LegoBrick> YellowBricks => productionLineYellow.Bricks;
    public double CustomerArrowX => ViewParameters.CustomerArrowPose.X;
    public double CustomerArrowY => ViewParameters.CustomerArrowPose.Y;
    public double RecyclingArrowX => ViewParameters.RecyclingArrowPose.X;
    public double RecyclingArrowY => ViewParameters.RecyclingArrowPose.Y;

    // Verwaltet Aufträge, Lieferungen und den nächsten Logistikschritt.
    private readonly Logistics logistics = new Logistics();

    // Enthält Steine, die sich gerade außerhalb ihrer Produktionslinie befinden.
    private readonly List<LegoBrick> transportBricks = new();

    // Die vier normalen Produktionslinien.
    private readonly List<ProductionLine> productionLines;
    public event Action<Parameters.LegoColor>? BrickDelivered;
    public event Action<string>? TransportMessage;
    public Factory()
    {
        // Die vier normalen Produktionslinien zusammenfassen.
        productionLines = new List<ProductionLine>
        {
            productionLineRed,
            productionLineYellow,
            productionLineBlue,
            productionLineGreen
        };

        // Alle Linien an den Roboter übergeben,
        // damit seine Kollisionsprüfung weiterhin funktioniert.
        List<ProductionLine> allLines = new List<ProductionLine>(productionLines)
        {
            productionLineRecycle,
            productionLineDelivery
        };

        robot = new Robot(RobotPose.Location, Borders, allLines);
    }
    public void RobotBewegen(Direction direction)
    {
        robot.Bewegen(direction);
        CheckBrickContacts();
    }
    
    public void LegoBricksBewegen()
    {
        productionLineRed.Bewegen(-1);
        productionLineBlue.Bewegen(-1);
        productionLineYellow.Bewegen(1);
        productionLineGreen.Bewegen(-1);

        int deliveryLeft = ViewParameters.ProductionLinePoseDelivery.X;
        int deliveryRight = deliveryLeft
                            + ViewParameters.DeliveryLineWidth;

        foreach (LegoBrick brick in new List<LegoBrick>(transportBricks))
        {
            int brickWidth = brick.Size == Parameters.LegoSize.large
                ? ViewParameters.BigBrickWidth
                : ViewParameters.SmallBrickWidth;

            // Erfolgreiche Lieferung: nach rechts zum Kunden.
            if (brick.Status == Parameters.BrickStatus.ToCustomer)
            {
                if (brick.Location.X < deliveryRight)
                {
                    brick.BewegenZumKunden();
                }
                else if (brick.VersatzIstNull)
                {
                    Parameters.LegoColor color = brick.Color;

                    // Brick an seine ursprüngliche Produktionsposition zurücksetzen.
                    brick.ZurueckZurProduktionslinie();

                    transportBricks.Remove(brick);

                    // Logistics erhöht den Lieferzähler und aktiviert
                    // erst jetzt den nächsten Auftragsschritt.
                    logistics.BrickDelivered();

                    // Das MainViewModel über die erfolgreiche Lieferung informieren.
                    BrickDelivered?.Invoke(color);

                    TransportMessage?.Invoke(
                        $"{Farbname(color)} brick successfully delivered to customer.");
                }
            }

            // Fehlerhafter Stein: nach links zum Recycling.
            else if (brick.Status == Parameters.BrickStatus.ToRecycle)
            {
                // Erst warten, bis der Stein vollständig links aus
                // der Delivery Line herausgefahren ist.
                if (brick.Location.X + brickWidth > deliveryLeft)
                {
                    brick.BewegenZumRecycling();
                }
                else
                {
                    int platz = FreierRecyclePlatz();

                    if (platz >= 0)
                    {
                        brick.AufRecyclePlatz(
                            RecyclePlatzPose(platz),
                            platz);
                        TransportMessage?.Invoke($"{Farbname(brick.Color)} Stein wurde zur Reparatur auf Recyclingplatz {platz + 1} abgelegt.");
                    }
                }
            }
        }

        CheckBrickContacts();
    }

    public Parameters.UndockResult Undock()
    {
        // Prüfen, ob der Roboter einen Stein hält.
        if (robot.Dockobjekt is not LegoBrick)
            return Parameters.UndockResult.NoBrick;

        // Undocken ist ausschließlich auf der Delivery Line erlaubt.
        if (!robot.AufDeliveryLinie())
            return Parameters.UndockResult.NotOnDeliveryLine;

        LegoBrick? brick = robot.Undock();

        if (brick == null)
            return Parameters.UndockResult.NoBrick;

        // Vor dem Transport auf 0 Grad drehen.
        brick.Angle = 0;

        // Echte Zufallsentscheidung über die Qualitätsprüfung.
        if (logistics.ShouldRecycle())
        {
            brick.ZumRecycling();

            // Den aktuellen Auftragsschritt beibehalten.
            // Beim nächsten Versuch muss dieser Brick aus
            // der Recycling Line aufgenommen werden.
            logistics.BrickRejected(brick);

            TransportMessage?.Invoke(
                "Brick rejected: sent to recycle line.");
        }
        else
        {
            brick.ZumKunden();

            TransportMessage?.Invoke(
                "Brick sent to customer.");
        }

        // Stein während des Transports verwalten.
        transportBricks.Add(brick);

        return Parameters.UndockResult.Success;
    }

    private void CheckBrickContacts()
    {
        // Ohne aktiven Auftrag darf kein Stein aufgenommen werden.
        if (!logistics.HasOrder || logistics.IsComplete)
            return;

        // Wenn bereits ein Stein gehalten wird,
        // ist keine weitere Aufnahme erlaubt.
        if (logistics.IsBrickPickedUp)
            return;

        // Normale Produktionslinien prüfen.
        foreach (ProductionLine line in productionLines)
        {
            CheckBrickContacts(line);

            if (robot.Dockobjekt != null)
                return;
        }

        // Anschließend die Recyclingsteine prüfen.
        foreach (LegoBrick brick in new List<LegoBrick>(transportBricks))
        {
            if (!logistics.IsNeeded(
                brick,
                productionLineRecycle,
                productionLines,
                productionLineRecycle))
            {
                continue;
            }

            robot.Kontakt(brick);

            if (robot.Dockobjekt == brick)
            {
                logistics.BrickPickedUp();

                // Der Brick wird jetzt vom Roboter gehalten.
                transportBricks.Remove(brick);

                return;
            }
        }
    }

    private void CheckBrickContacts(ProductionLine line)
    {
        foreach (LegoBrick brick in new List<LegoBrick>(line.Bricks))
        {
            // Nur den Brick aufnehmen, den Logistics erlaubt.
            if (!logistics.IsNeeded(
                brick,
                line,
                productionLines,
                productionLineRecycle))
            {
                continue;
            }

            robot.Kontakt(brick);

            if (robot.Dockobjekt == brick)
            {
                logistics.BrickPickedUp();
                return;
            }
        }
    }

    private int FreierRecyclePlatz()
    {
        for (int platz = 0; platz < Parameters.bricksPerLine; platz++)
        {
            bool belegt = false;

            // Prüfen, ob der Recyclingplatz bereits belegt ist.
            foreach (LegoBrick brick in transportBricks)
            {
                if (brick.RecyclePlatz == platz)
                {
                    belegt = true;
                    break;
                }
            }

            if (!belegt)
                return platz;
        }

        return -1;
    }

    private Pose RecyclePlatzPose(int platz)
    {
        int x = ViewParameters.ProductionLinePoseRecycle.X + 25 + platz * 24;
        int y = ViewParameters.ProductionLinePoseRecycle.Y
                - ViewParameters.SmallBrickHeight;

        return new Pose(x, y, 0);
    }

    private static string Farbname(Parameters.LegoColor color)
    {
        return color switch
        {
            Parameters.LegoColor.red => "Roter",
            Parameters.LegoColor.blue => "Blauer",
            Parameters.LegoColor.green => "Grüner",
            Parameters.LegoColor.yellow => "Gelber",
            _ => "LEGO"
        };
    }
    public void SetupProductionLines()
    {
        // Jede Produktionslinie erzeugt vier Steine
        // mit unabhängig zufällig bestimmter Farbe.
        productionLineRed.CreateBricks();
        productionLineYellow.CreateBricks();
        productionLineBlue.CreateBricks();
        productionLineGreen.CreateBricks();
    }
    public List<LegoBrick> AlleBricks()
    {
        List<LegoBrick> alleBricks = new List<LegoBrick>();

        alleBricks.AddRange(productionLineRed.Bricks);
        alleBricks.AddRange(productionLineGreen.Bricks);
        alleBricks.AddRange(productionLineBlue.Bricks);
        alleBricks.AddRange(productionLineYellow.Bricks);

        return alleBricks;
    }
    // Erstellt einen neuen Kundenauftrag aus den vorhandenen Steinen.
    public void CreateOrder()
    {
        List<LegoBrick> allBricks = AlleBricks();

        if (allBricks.Count == 0)
            throw new InvalidOperationException(
                "Bitte zuerst die Produktionslinien einrichten.");

        logistics.CreateRandomOrder(allBricks);
    }

    // Anzahl der bestellten Steine einer Farbe.
    public int GetOrder(LegoColor color)
    {
        return logistics.GetOrder(color);
    }

    // Anzahl der erfolgreich gelieferten Steine einer Farbe.
    public int GetDelivered(LegoColor color)
    {
        return logistics.GetDelivered(color);
    }

    // Informationen zum aktuellen Logistikschritt.
    public int LogisticsStep => logistics.CurrentStepNumber();

    public bool OrderComplete => logistics.IsComplete;

    public bool HasOrder => logistics.HasOrder;

    // Informationen zum aktuellen Logistikschritt.
    public LegoColor? CurrentBrickColor => logistics.CurrentStep()?.Color;

    public ProductionLine? CurrentStation =>
        logistics.PickStation(productionLines, productionLineRecycle);

    public bool IsBrickPickedUp => logistics.IsBrickPickedUp;

    public int OrderCount => logistics.Order.Count;

    // Positioniert den Roboter vor der nächsten Aufnahmestation.
    public bool GoToLine()
    {
        // Ohne Auftrag oder bei bereits aufgenommenem Brick
        // darf keine neue Positionierung erfolgen.
        if (!logistics.HasOrder || logistics.IsComplete || logistics.IsBrickPickedUp)
            return false;

        ProductionLine? station = logistics.PickStation(
            productionLines,
            productionLineRecycle);

        LegoBrick? brick = logistics.PickupBrick;

        if (station == null || brick == null)
            return false;

        // Ein Recyclingstein muss zuerst vollständig abgelegt sein.
        if (station == productionLineRecycle && brick.RecyclePlatz < 0)
        {
            TransportMessage?.Invoke(
                "Please wait until the brick reaches the recycle line.");

            return false;
        }

        // Passenden Winkel für die ausgewählte Station bestimmen.
        double zielwinkel;

        if (station == productionLineRed)
            zielwinkel = ViewParameters.GoToRedAngle;

        else if (station == productionLineBlue)
            zielwinkel = ViewParameters.GoToBlueAngle;

        else if (station == productionLineGreen)
            zielwinkel = ViewParameters.GoToGreenAngle;

        else if (station == productionLineYellow)
            zielwinkel = ViewParameters.GoToYellowAngle;

        else
            zielwinkel = ViewParameters.GoToRecycleAngle;

        // Roboter direkt vor der Station positionieren.
        robot.GeheZuLinie(
            station.ProductionLinePose,
            ViewParameters.ProductionLineWidth,
            ViewParameters.ProductionLineHeight,
            zielwinkel);

        TransportMessage?.Invoke(
            "Robot at the line: drive forward to dock the brick.");

        return true;
    }

    // Positioniert den Roboter vor der Delivery Line.
    public bool GoToDelivery()
    {
        // Die Positionierung ist nur mit aufgenommenem Brick sinnvoll.
        if (robot.Dockobjekt is not LegoBrick || !logistics.IsBrickPickedUp)
            return false;

        // Mittelpunkt der Delivery Line berechnen.
        Point deliveryCenter = new Point(
            productionLineDelivery.ProductionLinePose.X
                + ViewParameters.DeliveryLineWidth / 2,

            productionLineDelivery.ProductionLinePose.Y
                + ViewParameters.DeliveryLineHeight / 2);

        robot.GeheZuLinie(
            productionLineDelivery.ProductionLinePose,
            ViewParameters.DeliveryLineWidth,
            ViewParameters.DeliveryLineHeight,
            ViewParameters.GoToDeliveryAngle);

        TransportMessage?.Invoke(
            "Robot at delivery: drive forward, then click undock.");

        return true;
    }
}
