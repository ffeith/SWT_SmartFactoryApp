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

namespace MyAvaloniaApp.ViewModels;

public class Factory
{
    private Robot robot;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double Rotation => robot.Rotation;
    private ProductionLine productionLineRed = new ProductionLine(ProductionLinePoseRed, Parameters.LegoColor.red);
    private ProductionLine productionLineYellow = new ProductionLine(ProductionLinePoseYellow, Parameters.LegoColor.yellow);
    private ProductionLine productionLineBlue = new ProductionLine(ProductionLinePoseBlue, Parameters.LegoColor.blue);
    private ProductionLine productionLineGreen = new ProductionLine(ProductionLinePoseGreen, Parameters.LegoColor.green);
    public ProductionLine ProductionLineRed => productionLineRed;
    public ProductionLine ProductionLineYellow => productionLineYellow;
    public ProductionLine ProductionLineBlue => productionLineBlue;
    public ProductionLine ProductionLineGreen => productionLineGreen;
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
    public Factory()
    {
        robot = new Robot(RobotPose.Location, Borders);
    }
    public void RobotBewegen(Direction direction)
    {
        robot.Bewegen(direction);
    }
    
    private int tickCount = 0;

    public void MoveBricks()
    {
        const int stepsPerDirection = 10;

        bool forward = tickCount < stepsPerDirection;

        tickCount = (tickCount + 1) % (stepsPerDirection * 2);

        MoveBricksAlongLine(productionLineRed, forward, 1);
        MoveBricksAlongLine(productionLineBlue, forward, -1);
        MoveBricksAlongLine(productionLineYellow, forward, 1);
        MoveBricksAlongLine(productionLineGreen, forward, -1);
    }

    private void MoveBricksAlongLine(
        ProductionLine line,
        bool forward,
        int directionSign)
    {
        double angleRad = line.ProductionLinePose.Angle * Math.PI / 180.0;
        double deltaX = Math.Cos(angleRad) * directionSign;                 //Richtung entlang der Produktionslinie
        double deltaY = Math.Sin(angleRad) * directionSign;
        double timeScale = Parameters.timerInterval / 50.0;                 //Schrittweite an das Timerintervall anpassen, X = Y wegen 45°
        double distance = Parameters.step * Math.Sqrt(2) * timeScale;

        deltaX *= distance;
        deltaY *= distance;

        if (!forward)                                                       //Nach 30 Ticks die Bewegungsrichtung umkehren
        {
            deltaX = -deltaX;
            deltaY = -deltaY;
        }

        // Alle vier Steine dieser Produktionslinie bewegen
        foreach (LegoBrick brick in line.Bricks)
        {
            brick.Bewegen(deltaX, deltaY);
        }
    }
}
