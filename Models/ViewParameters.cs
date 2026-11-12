using System.Drawing;

namespace MyAvaloniaApp.Models;

// Beschreibt Position und Ausrichtung eines Objekts.
public struct Pose
{
    public int X { get; }
    public int Y { get; }
    public int Angle { get; }

    public Pose(int x, int y, int angle)
    {
        X = x;
        Y = y;
        Angle = angle;
    }

    public Point Location => new Point(X, Y);
}

// Enthält feste Positionen und Größen der grafischen Fabrikobjekte.
public static class ViewParameters
{
    public static readonly Pose RobotPose = new(600, 280, 0);

    public static readonly Pose ProductionLinePoseRed = new(410, 380, 90);
    public static readonly Pose ProductionLinePoseBlue = new(706, 380, 90);
    public static readonly Pose ProductionLinePoseYellow = new(420, 610, 0);
    public static readonly Pose ProductionLinePoseGreen = new(696, 610, 0);

    public static readonly Pose ProductionLinePoseRecycle = new(558, 465, 0);
    public static readonly Pose ProductionLinePoseDelivery = new(538, 190, 0);

    public static readonly Rectangle Borders = new(357, 182, 522, 524);

    public const int RobotWidth = 38;
    public const int RobotHeight = 80;

    public const int ButtonWidth = 65;
    public const int ButtonHeight = 30;
    public const int FontSize = 14;

    public const int ProductionLineWidth = 120;
    public const int ProductionLineHeight = 36;

    public const int RecycleLineWidth = ProductionLineWidth;
    public const int RecycleLineHeight = ProductionLineHeight;

    public const int DeliveryLineWidth = 150;
    public const int DeliveryLineHeight = 50;

    public const int SmallBrickWidth = 12;
    public const int SmallBrickHeight = 12;

    public const int BigBrickWidth = 20;
    public const int BigBrickHeight = 12;

    public static readonly Pose CustomerArrowPose = new(694, 190, 0);
    public static readonly Pose RecyclingArrowPose = new(435, 190, 0);

    public const double CustomerArrowWidth = 100;
    public const double CustomerArrowHeight = 50;

    public const double RecyclingArrowWidth = 100;
    public const double RecyclingArrowHeight = 50;

    public const double GoToRedAngle = 270;
    public const double GoToBlueAngle = 90;
    public const double GoToGreenAngle = 180;
    public const double GoToYellowAngle = 180;
    public const double GoToRecycleAngle = 180;
    public const double GoToDeliveryAngle = 0;
}