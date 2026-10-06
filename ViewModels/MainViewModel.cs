using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;
using Avalonia.Data;

namespace MyAvaloniaApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Robot robot;
    Rectangle borders;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double Rotation => robot.Rotation;

    public MainViewModel()
    {
        Point point = new Point(450, 300);
        borders = new Rectangle(360, 195, 447, 450);
        robot = new Robot(point, borders);
    }

    public void MoveUp()
    {
        robot.Bewegen(Direction.up);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveDown()
    {
        robot.Bewegen(Direction.down);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveLeft()
    {
        robot.Bewegen(Direction.left);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveRight()
    {
        robot.Bewegen(Direction.right);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveUpLeft()
    {
        robot.Bewegen(Direction.left);
        robot.Bewegen(Direction.up);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveUpRight()
    {
        robot.Bewegen(Direction.right);
        robot.Bewegen(Direction.up);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveDownLeft()
    {
        robot.Bewegen(Direction.left);
        robot.Bewegen(Direction.down);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void MoveDownRight()
    {
        robot.Bewegen(Direction.right);
        robot.Bewegen(Direction.down);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
    }
    public void TurnLeft()
    {
        robot.Bewegen(Direction.turnLeft);
        OnPropertyChanged(nameof(Rotation));
    }
    public void TurnRight()
    {
        robot.Bewegen(Direction.turnRight);
        OnPropertyChanged(nameof(Rotation));
    }

}
