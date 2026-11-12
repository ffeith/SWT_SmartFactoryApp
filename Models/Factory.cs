using System;
using System.Collections.Generic;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

// Koordiniert Roboter, Produktionslinien, Aufträge und Steintransport.
public class Factory
{
    private readonly Robot robot;

    private readonly ProductionLine productionRed =
        new(ViewParameters.ProductionLinePoseRed);

    private readonly ProductionLine productionGreen =
        new(ViewParameters.ProductionLinePoseGreen);

    private readonly ProductionLine productionBlue =
        new(ViewParameters.ProductionLinePoseBlue);

    private readonly ProductionLine productionYellow =
        new(ViewParameters.ProductionLinePoseYellow);

    private readonly ProductionLine productionRecycle =
        new(ViewParameters.ProductionLinePoseRecycle);

    private readonly ProductionLine productionDelivery =
        new(ViewParameters.ProductionLinePoseDelivery);

    private readonly Logistics logistics = new();

    private readonly List<LegoBrick> transportBricks = new();

    private readonly List<ProductionLine> productionLines;

    public double RobotX => robot.X;
    public double RobotY => robot.Y;
    public double RobotAngle => robot.Angle;

    public Direction RobotDirection => robot.RobotDirection;

    public ProductionLine ProductionLineRed => productionRed;
    public ProductionLine ProductionLineGreen => productionGreen;
    public ProductionLine ProductionLineBlue => productionBlue;
    public ProductionLine ProductionLineYellow => productionYellow;

    public ProductionLine ProductionLineRecycle => productionRecycle;
    public ProductionLine ProductionLineDelivery => productionDelivery;

    // Zusätzliche sprechende Namen für andere Klassen.
    public ProductionLine RecycleLine => productionRecycle;
    public ProductionLine DeliveryLine => productionDelivery;

    public List<LegoBrick> RedBricks => productionRed.Bricks;
    public List<LegoBrick> GreenBricks => productionGreen.Bricks;
    public List<LegoBrick> BlueBricks => productionBlue.Bricks;
    public List<LegoBrick> YellowBricks => productionYellow.Bricks;

    public double CustomerArrowX =>
        ViewParameters.CustomerArrowPose.X;

    public double CustomerArrowY =>
        ViewParameters.CustomerArrowPose.Y;

    public double RecyclingArrowX =>
        ViewParameters.RecyclingArrowPose.X;

    public double RecyclingArrowY =>
        ViewParameters.RecyclingArrowPose.Y;

    public List<LegoBrick> LegoOrder => logistics.Order;

    public int LogisticsStep =>
        logistics.CurrentStepNumber();

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

    public event Action<LegoColor>? BrickDelivered;
    public event Action<string>? TransportMessage;

    public Factory()
    {
        productionLines = new List<ProductionLine>
        {
            productionRed,
            productionGreen,
            productionBlue,
            productionYellow
        };

        List<ProductionLine> allLines = new(productionLines)
        {
            productionRecycle,
            productionDelivery
        };

        robot = new Robot(
            ViewParameters.RobotPose,
            ViewParameters.Borders,
            allLines,
            allLines);
    }

    // Bewegt den Roboter und prüft danach Kontakte.
    public void RobotBewegen(Direction direction)
    {
        robot.Bewegen(direction);

        CheckBrickContacts();
    }

    // Bewegt Produktionslinien und transportierte Steine.
    public void LegoBricksBewegen()
    {
        productionRed.Bewegen(-1);
        productionBlue.Bewegen(-1);
        productionYellow.Bewegen(1);
        productionGreen.Bewegen(-1);

        int deliveryLeft =
            ViewParameters.ProductionLinePoseDelivery.X;

        int deliveryRight =
            deliveryLeft + ViewParameters.DeliveryLineWidth;

        foreach (LegoBrick brick in new List<LegoBrick>(transportBricks))
        {
            int brickWidth = brick.Size == LegoSize.large
                ? ViewParameters.BigBrickWidth
                : ViewParameters.SmallBrickWidth;

            if (brick.Status == BrickStatus.ToCustomer)
            {
                MoveBrickToCustomer(brick, deliveryRight);
            }
            else if (brick.Status == BrickStatus.ToRecycle)
            {
                MoveBrickToRecycle(
                    brick,
                    deliveryLeft,
                    brickWidth);
            }
        }

        CheckBrickContacts();
    }

    // Bewegt einen akzeptierten Stein zum Kunden.
    private void MoveBrickToCustomer(
        LegoBrick brick,
        int deliveryRight)
    {
        if (brick.Location.X < deliveryRight)
        {
            brick.BewegenZumKunden();
            return;
        }

        if (!brick.VersatzIstNull)
            return;

        LegoColor color = brick.Color;

        brick.ZurueckZurProduktionslinie();

        transportBricks.Remove(brick);

        logistics.BrickDelivered();

        BrickDelivered?.Invoke(color);

        TransportMessage?.Invoke(
            $"{ColorName(color)} brick successfully delivered to customer.");
    }

    // Bewegt einen abgelehnten Stein zum Recycling.
    private void MoveBrickToRecycle(
        LegoBrick brick,
        int deliveryLeft,
        int brickWidth)
    {
        if (brick.Location.X + brickWidth > deliveryLeft)
        {
            brick.BewegenZumRecycling();
            return;
        }

        int freePlace = FreierRecyclePlatz();

        if (freePlace < 0)
            return;

        brick.AufRecyclePlatz(
            RecyclePlatzPose(freePlace),
            freePlace);

        TransportMessage?.Invoke(
            $"{ColorName(brick.Color)} brick placed on recycle position {freePlace + 1}.");
    }

    // Löst einen aufgenommenen Stein auf der Delivery Line.
    public UndockResult Undock()
    {
        if (robot.Dockobjekt is not LegoBrick)
            return UndockResult.NoBrick;

        if (!robot.AufDeliveryLinie())
            return UndockResult.NotOnDeliveryLine;

        LegoBrick? brick = robot.Undock();

        if (brick == null)
            return UndockResult.NoBrick;

        brick.Angle = 0;

        if (logistics.ShouldRecycle())
        {
            brick.ZumRecycling();

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

        transportBricks.Add(brick);

        return UndockResult.Success;
    }

    // Prüft, ob der Roboter den benötigten Stein erreicht.
    private void CheckBrickContacts()
    {
        if (!logistics.HasOrder ||
            logistics.IsComplete ||
            logistics.IsBrickPickedUp)
        {
            return;
        }

        // Zuerst normale Produktionslinien prüfen.
        foreach (ProductionLine line in productionLines)
        {
            CheckBrickContacts(line);

            if (robot.Dockobjekt != null)
                return;
        }

        // Nach einer Ablehnung ist nur der abgelehnte Stein erlaubt.
        foreach (LegoBrick brick in new List<LegoBrick>(transportBricks))
        {
            if (!logistics.IsNeeded(
                brick,
                productionRecycle,
                productionLines,
                productionRecycle))
            {
                continue;
            }

            robot.Kontakt(brick);

            if (robot.Dockobjekt == brick)
            {
                logistics.BrickPickedUp();

                transportBricks.Remove(brick);

                return;
            }
        }
    }

    // Prüft die Steine einer normalen Produktionslinie.
    private void CheckBrickContacts(ProductionLine line)
    {
        foreach (LegoBrick brick in new List<LegoBrick>(line.Bricks))
        {
            if (!logistics.IsNeeded(
                brick,
                line,
                productionLines,
                productionRecycle))
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

    // Sucht den ersten freien Recyclingplatz.
    private int FreierRecyclePlatz()
    {
        for (int place = 0;
             place < Parameters.bricksPerLine;
             place++)
        {
            bool occupied = false;

            foreach (LegoBrick brick in transportBricks)
            {
                if (brick.RecyclePlatz == place)
                {
                    occupied = true;
                    break;
                }
            }

            if (!occupied)
                return place;
        }

        return -1;
    }

    // Berechnet die Position eines Recyclingplatzes.
    private Pose RecyclePlatzPose(int place)
    {
        int x =
            ViewParameters.ProductionLinePoseRecycle.X
            + 25
            + place * 24;

        int y =
            ViewParameters.ProductionLinePoseRecycle.Y
            - ViewParameters.SmallBrickHeight;

        return new Pose(x, y, 0);
    }

    private static string ColorName(LegoColor color)
    {
        return color switch
        {
            LegoColor.red => "red",
            LegoColor.green => "green",
            LegoColor.blue => "blue",
            LegoColor.yellow => "yellow",
            _ => "LEGO"
        };
    }

    // Erzeugt vier Steine auf jeder normalen Produktionslinie.
    public void SetupProductionLines()
    {
        foreach (ProductionLine line in productionLines)
        {
            line.CreateBricks();
        }
    }

    // Gibt alle Steine der normalen Produktionslinien zurück.
    public List<LegoBrick> AlleBricks()
    {
        List<LegoBrick> allBricks = new();

        foreach (ProductionLine line in productionLines)
        {
            allBricks.AddRange(line.Bricks);
        }

        return allBricks;
    }

    public void CreateOrder()
    {
        List<LegoBrick> allBricks = AlleBricks();

        if (allBricks.Count == 0)
        {
            throw new InvalidOperationException(
                "Bitte zuerst die Produktionslinien einrichten.");
        }

        logistics.CreateRandomOrder(allBricks);
    }

    public int GetOrder(LegoColor color) =>
        logistics.GetOrder(color);

    public int GetDelivered(LegoColor color) =>
        logistics.GetDelivered(color);

    // Positioniert den Roboter vor der nächsten Station.
    public bool GoToLine()
    {
        if (!logistics.HasOrder ||
            logistics.IsComplete ||
            logistics.IsBrickPickedUp)
        {
            return false;
        }

        ProductionLine? station =
            logistics.PickStation(productionLines, productionRecycle);

        LegoBrick? brick = logistics.PickupBrick;

        if (station == null || brick == null)
            return false;

        if (station == productionRecycle &&
            brick.RecyclePlatz < 0)
        {
            TransportMessage?.Invoke(
                "Please wait until the brick reaches the recycle line.");

            return false;
        }

        double targetAngle;
        double lineWidth;
        double lineHeight;

        if (station == productionRed)
        {
            targetAngle = ViewParameters.GoToRedAngle;
            lineWidth = ViewParameters.ProductionLineWidth;
            lineHeight = ViewParameters.ProductionLineHeight;
        }
        else if (station == productionBlue)
        {
            targetAngle = ViewParameters.GoToBlueAngle;
            lineWidth = ViewParameters.ProductionLineWidth;
            lineHeight = ViewParameters.ProductionLineHeight;
        }
        else if (station == productionGreen)
        {
            targetAngle = ViewParameters.GoToGreenAngle;
            lineWidth = ViewParameters.ProductionLineWidth;
            lineHeight = ViewParameters.ProductionLineHeight;
        }
        else if (station == productionYellow)
        {
            targetAngle = ViewParameters.GoToYellowAngle;
            lineWidth = ViewParameters.ProductionLineWidth;
            lineHeight = ViewParameters.ProductionLineHeight;
        }
        else
        {
            targetAngle = ViewParameters.GoToRecycleAngle;
            lineWidth = ViewParameters.RecycleLineWidth;
            lineHeight = ViewParameters.RecycleLineHeight;
        }

        robot.GeheZuLinie(
            station.ProductionLinePose,
            lineWidth,
            lineHeight,
            targetAngle);

        TransportMessage?.Invoke(
            "Robot at the line: drive forward to dock the brick.");

        return true;
    }

    // Positioniert den Roboter vor der Delivery Line.
    public bool GoToDelivery()
    {
        if (robot.Dockobjekt is not LegoBrick ||
            !logistics.IsBrickPickedUp)
        {
            return false;
        }

        robot.GeheZuLinie(
            productionDelivery.ProductionLinePose,
            ViewParameters.DeliveryLineWidth,
            ViewParameters.DeliveryLineHeight,
            ViewParameters.GoToDeliveryAngle);

        TransportMessage?.Invoke(
            "Robot at delivery: drive forward, then click undock.");

        return true;
    }
}