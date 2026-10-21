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
    public static Pose RobotPose = new Pose(450, 300, 0);
    public static Pose BrickPose = new Pose(550, 550, 45);
    public static Pose ProductionLinePoseRed = new Pose(300, 320, 90);
    public static Pose ProductionLinePoseBlue = new Pose(687, 320, 90);
    public static Pose ProductionLinePoseYellow = new Pose(370, 535, 0);
    public static Pose ProductionLinePoseGreen = new Pose(617, 535, 0);
    public static Pose ProductionLinePoseRecycle = new Pose(493, 425, 0);
    public static Pose ProductionLinePoseDelivery = new Pose(493, 183, 0);
    public static Rectangle Borders = new Rectangle(360, 183, 447, 474);
    public const int RobotWidth = 50;
    public const int RobotHeight = 30;
    public const int ButtonWidth = 65;
    public const int ButtonHeight = 30;
    public const int FontSize = 14;
    public const int ProductionLineWidth = 250;
    public const int ProductionLineHeight = 40;
    public const int BrickWidth = 20;
    public const int BrickHeight = 20;
}