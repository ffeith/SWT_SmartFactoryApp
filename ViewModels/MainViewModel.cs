using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Robot robot;
    Rectangle borders;
    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;

    public MainViewModel()
    {
        Point point = new Point(450, 300);
        Rectangle borders = new Rectangle(0, 0, 900, 600);
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

}
