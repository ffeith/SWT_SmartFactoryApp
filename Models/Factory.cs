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
    private ProductionLine productionLineRed = new ProductionLine(ProductionLinePoseRed, Parameters.LegoColor.red);
    private ProductionLine productionLineYellow = new ProductionLine(ProductionLinePoseYellow, Parameters.LegoColor.yellow);
    private ProductionLine productionLineBlue = new ProductionLine(ProductionLinePoseBlue, Parameters.LegoColor.blue);
    private ProductionLine productionLineGreen = new ProductionLine(ProductionLinePoseGreen, Parameters.LegoColor.green);
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
    private readonly List<LegoBrick> logistics = new List<LegoBrick>();
    public double CustomerArrowX => ViewParameters.CustomerArrowPose.X;
    public double CustomerArrowY => ViewParameters.CustomerArrowPose.Y;
    public double RecyclingArrowX => ViewParameters.RecyclingArrowPose.X;
    public double RecyclingArrowY => ViewParameters.RecyclingArrowPose.Y;
    private bool naechsterZumRecycling = false;
    public Factory()
    {
        List<ProductionLine> productionLines = new List<ProductionLine>
        {
            productionLineRed,
            productionLineYellow,
            productionLineBlue,
            productionLineGreen,
            productionLineRecycle,
            productionLineDelivery
        };

        robot = new Robot(RobotPose.Location, Borders, productionLines);
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

        foreach (LegoBrick brick in new List<LegoBrick>(logistics))
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
                    brick.ZurueckZurProduktionslinie();
                    logistics.Remove(brick);
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
                    }
                }
            }
        }

        CheckBrickContacts();
    }

    public Parameters.UndockResult Undock()
    {
        // Steine dürfen nur auf der Delivery Line abgesetzt werden.
        if (!robot.AufDeliveryLinie())
            return Parameters.UndockResult.NotOnDeliveryLine;

        LegoBrick? brick = robot.Undock();

        if (brick == null)
            return Parameters.UndockResult.NoBrick;

        brick.Angle = 0;

        // Transportziel abwechselnd festlegen.
        if (naechsterZumRecycling)
        {
            brick.ZumRecycling();
            naechsterZumRecycling = false;
        }
        else
        {
            brick.ZumKunden();
            naechsterZumRecycling = true;
        }

        logistics.Add(brick);

        // Den Roboter rückwärts bewegen.
        for (int i = 0; i < 4; i++)
            robot.Bewegen(Direction.down);

        return Parameters.UndockResult.Success;
    }
    

    private void CheckBrickContacts()
    {
        CheckBrickContacts(productionLineRed);
        CheckBrickContacts(productionLineYellow);
        CheckBrickContacts(productionLineBlue);
        CheckBrickContacts(productionLineGreen);
        CheckBrickContacts(productionLineRecycle);

        foreach (LegoBrick brick in new List<LegoBrick>(logistics))
        {
            robot.Kontakt(brick);

            // Ein wieder aufgenommener Stein verlässt die Logistikliste.
            if (brick.Status == Parameters.BrickStatus.Docked)
            {
                logistics.Remove(brick);
            }
        }
    }

    private void CheckBrickContacts(ProductionLine line)
    {
        // Kopie erstellen, weil wir die ursprüngliche Liste
        // beim erfolgreichen Aufnehmen verändern können.
        foreach (LegoBrick brick in new List<LegoBrick>(line.Bricks))
            robot.Kontakt(brick);
    }

    private int FreierRecyclePlatz()
    {
        for (int platz = 0; platz < Parameters.bricksPerLine; platz++)
        {
            bool belegt = false;

            foreach (LegoBrick brick in logistics)
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
}
