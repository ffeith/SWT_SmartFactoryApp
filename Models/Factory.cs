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

namespace MyAvaloniaApp.ViewModels;

public class Factory
{
    private Robot robot;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double Rotation => robot.Rotation;
    private ProductionLine productionLineRed = new ProductionLine(ProductionLinePoseRed, Parameters.LegoColor.red, LegoSize.small);
    private ProductionLine productionLineYellow = new ProductionLine(ProductionLinePoseYellow, Parameters.LegoColor.yellow, LegoSize.small);
    private ProductionLine productionLineBlue = new ProductionLine(ProductionLinePoseBlue, Parameters.LegoColor.blue, LegoSize.small);
    private ProductionLine productionLineGreen = new ProductionLine(ProductionLinePoseGreen, Parameters.LegoColor.green, LegoSize.small);
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
    int tickCount = 0;
    public void LegoBricksBewegen()
    {
        bool outward = tickCount < 10;
        tickCount = (tickCount + 1) % 20;

        ProductionLine[] productionLines =
        {
            productionLineRed,
            productionLineYellow,
            productionLineBlue,
            productionLineGreen
        };

        foreach (ProductionLine line in productionLines)
        {
            foreach (LegoBrick brick in line.Bricks)
            {
                if (outward)
                {
                    brick.Bewegen(Parameters.Direction.right);
                    brick.Bewegen(Parameters.Direction.up);
                }
                else
                {
                    brick.Bewegen(Parameters.Direction.left);
                    brick.Bewegen(Parameters.Direction.down);
                }
            }
        }
    }
}
