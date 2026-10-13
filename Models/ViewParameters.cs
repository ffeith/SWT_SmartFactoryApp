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
    public static Pose ProductionLinePoseRed = new Pose(380, 235, -45);
    public static Pose ProductionLinePoseYellow = new Pose(380, 625, 45);
    public static Pose ProductionLinePoseBlue = new Pose(767, 235, 45);
    public static Pose ProductionLinePoseGreen = new Pose(767, 625, -45);
    public static Rectangle Borders = new Rectangle(360, 183, 447, 474);
}