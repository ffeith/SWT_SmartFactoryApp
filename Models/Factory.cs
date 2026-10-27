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

        CheckBrickContacts();
    }

    public void Undock()
    {
        LegoBrick? releasedBrick = robot.Undock();

        if (releasedBrick == null)
            return;

        if (!logistics.Contains(releasedBrick))
            logistics.Add(releasedBrick);

        // Den Roboter rückwärts bewegen.
        for (int i = 0; i < 4; i++)
            robot.Bewegen(Direction.down);
    }

    private void CheckBrickContacts()
    {
        CheckBrickContacts(productionLineRed);
        CheckBrickContacts(productionLineYellow);
        CheckBrickContacts(productionLineBlue);
        CheckBrickContacts(productionLineGreen);
        CheckBrickContacts(productionLineRecycle);

        // Abgesetzte Steine können erneut aufgenommen werden.
        foreach (LegoBrick brick in new List<LegoBrick>(logistics))
        {
            bool wasDocked = brick.Gedockt;

            robot.Kontakt(brick);

            // Wurde dieser Stein gerade erneut aufgenommen?
            if (!wasDocked && brick.Gedockt)
                logistics.Remove(brick);
        }
    }

    private void CheckBrickContacts(ProductionLine line)
    {
        // Kopie erstellen, weil wir die ursprüngliche Liste
        // beim erfolgreichen Aufnehmen verändern können.
        foreach (LegoBrick brick in new List<LegoBrick>(line.Bricks))
        {
            bool wasDocked = brick.Gedockt;

            robot.Kontakt(brick);

            // Nur bei einer tatsächlich erfolgreichen Aufnahme entfernen.
            if (!wasDocked && brick.Gedockt)
                line.Bricks.Remove(brick);
        }
    }
}
