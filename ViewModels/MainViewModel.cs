using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;
using Avalonia.Data;
using System.Collections.Generic;
using Avalonia.Threading;
using System;
using System.Threading;

namespace MyAvaloniaApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Factory factory;
    DispatcherTimer timer;
    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double Rotation => factory.Rotation;
    public ProductionLine ProductionLineRed => factory.ProductionLineRed;
    public ProductionLine ProductionLineYellow => factory.ProductionLineYellow;
    public ProductionLine ProductionLineBlue => factory.ProductionLineBlue;
    public ProductionLine ProductionLineGreen => factory.ProductionLineGreen;
    public List<LegoBricksViewModel> BricksView { get; } = new List<LegoBricksViewModel>();

    public MainViewModel()
    {
        factory = new Factory();

        foreach (var brick in factory.ProductionLineRed.Bricks)
            BricksView.Add(new LegoBricksViewModel(brick));

        foreach (var brick in factory.ProductionLineYellow.Bricks)
            BricksView.Add(new LegoBricksViewModel(brick));

        foreach (var brick in factory.ProductionLineBlue.Bricks)
            BricksView.Add(new LegoBricksViewModel(brick));

        foreach (var brick in factory.ProductionLineGreen.Bricks)
            BricksView.Add(new LegoBricksViewModel(brick));

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        timer.Tick += Timer_Tick;
    }
        private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.MoveBricks();
        NotifyLegoBricksPoseChange();
    }

    public void MoveUp()
    {
        factory.RobotBewegen(Direction.up);
        NotifyRobotPoseChanged();
    }
    public void MoveDown()
    {
        factory.RobotBewegen(Direction.down);
        NotifyRobotPoseChanged();
    }
    public void MoveLeft()
    {
        factory.RobotBewegen(Direction.left);
        NotifyRobotPoseChanged();
    }
    public void MoveRight()
    {
        factory.RobotBewegen(Direction.right);
        NotifyRobotPoseChanged();
    }
    public void MoveUpLeft()
    {
        factory.RobotBewegen(Direction.left);
        factory.RobotBewegen(Direction.up);
        NotifyRobotPoseChanged();
    }
    public void MoveUpRight()
    {
        factory.RobotBewegen(Direction.right);
        factory.RobotBewegen(Direction.up);
        NotifyRobotPoseChanged();
    }
    public void MoveDownLeft()
    {
        factory.RobotBewegen(Direction.left);
        factory.RobotBewegen(Direction.down);
        NotifyRobotPoseChanged();
    }
    public void MoveDownRight()
    {
        factory.RobotBewegen(Direction.right);
        factory.RobotBewegen(Direction.down);
        NotifyRobotPoseChanged();
    }
    public void TurnLeft()
    {
        factory.RobotBewegen(Direction.turnLeft);
        NotifyRobotPoseChanged();
    }
    public void TurnRight()
    {
        factory.RobotBewegen(Direction.turnRight);
        NotifyRobotPoseChanged();
    }
    public void TurnToZeroDegrees()
    {
        factory.RobotBewegen(Direction.zeroDegrees);
        NotifyRobotPoseChanged();
    }
    public void TimerStart()
    {
        timer.Start();
    }
    public void TimerStop()
    {
        timer.Stop();
    }
    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(Rotation));
    }
    private void NotifyLegoBricksPoseChange()
    {
        foreach (var brick in BricksView)
        {
            brick.RefreshProperties();
        }
    }
}
