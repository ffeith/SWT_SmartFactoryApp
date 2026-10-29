using System.Drawing;

namespace MyAvaloniaApp.Models;
public struct Pose
{
    public int X{ get; }
    public int Y{ get; }
    public int Angle{ get; }
    public Pose(int x, int y, int angle)
    {
        X = x;
        Y = y;
        Angle = angle;
    }
    public Point Location => new Point(X, Y);
}
public static class ViewParameters
{
    public static Pose RobotPose = new Pose(600, 280, 0);
    public static Pose BrickPose = new Pose(550, 550, 45);
    public static Pose ProductionLinePoseRed =new Pose(410, 380, 90);
    public static Pose ProductionLinePoseBlue =new Pose(706, 380, 90);
    public static Pose ProductionLinePoseYellow =new Pose(420, 570, 0);
    public static Pose ProductionLinePoseGreen =new Pose(696, 570, 0);
    public static Pose ProductionLinePoseRecycle =new Pose(558, 425, 0);
    public static Pose ProductionLinePoseDelivery =new Pose(558, 190, 0);
    public static Rectangle Borders = new Rectangle(357, 182, 522, 524);
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
    public const int BigBrickWidth = 20;
    public const int SmallBrickHeight = 12;
    public const int BigBrickHeight = 12;
    public static Pose CustomerArrowPose = new Pose(714, 190, 0);
    public static Pose RecyclingArrowPose = new Pose(455, 190, 0);
    public const double CustomerArrowWidth = 100;
    public const double CustomerArrowHeight = 50;
    public const double RecyclingArrowWidth = 100;
    public const double RecyclingArrowHeight = 50;
}