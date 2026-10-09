using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;
using Avalonia.Data;
using static MyAvaloniaApp.Models.ViewParameters;
using System.Collections.Generic;

namespace MyAvaloniaApp.ViewModels;

public class Factory
{
    Robot robot;
    List <LegoBrick> bricks = new List<LegoBrick>();
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double Rotation => robot.Rotation;
    public List<LegoBrick> Bricks => bricks;

    public Factory()
    {
        robot = new Robot(RobotPose.Location, Borders);
        CreateBricks();
    }
    private void CreateBricks()
    {
        Pose pose1 = new Pose(BrickPose.X + 0, BrickPose.Y, BrickPose.Angle);
        Pose pose2 = new Pose(BrickPose.X + 50, BrickPose.Y, BrickPose.Angle);
        Pose pose3 = new Pose(BrickPose.X + 100, BrickPose.Y, BrickPose.Angle);
        Pose pose4 = new Pose(BrickPose.X + 150, BrickPose.Y, BrickPose.Angle);
        bricks.Add(new LegoBrick(pose1, LegoColor.red, LegoSize.small));
        bricks.Add(new LegoBrick(pose2, LegoColor.green, LegoSize.large));
        bricks.Add(new LegoBrick(pose3, LegoColor.yellow, LegoSize.small));
        bricks.Add(new LegoBrick(pose4, LegoColor.blue, LegoSize.large));
    }
    public void RobotBewegen(Direction direction)
    {
        robot.Bewegen(direction);
    }
    int tickCount = 0;

public void LegoBrickBewegen()
    {
        bool outward = tickCount < 10;
        tickCount = (tickCount + 1) % 20;
        foreach (LegoBrick brick in Bricks)
        {
            if (outward)
                brick.Bewegen(Parameters.Direction.right);
            else
                brick.Bewegen(Parameters.Direction.left);
        }
    }
}