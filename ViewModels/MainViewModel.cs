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
    private DispatcherTimer robotMovementTimer;
    private Action? currentRobotMovement;
    private bool continuousMovementEnabled;
    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double Rotation => factory.Rotation;
    public ProductionLine ProductionLineRed => factory.ProductionLineRed;
    public ProductionLine ProductionLineYellow => factory.ProductionLineYellow;
    public ProductionLine ProductionLineBlue => factory.ProductionLineBlue;
    public ProductionLine ProductionLineGreen => factory.ProductionLineGreen;
    public ProductionLine ProductionLineRecycle => factory.ProductionLineRecycle;
    public ProductionLine ProductionLineDelivery => factory.ProductionLineDelivery;
    public double ProductionLineWidth => ViewParameters.ProductionLineWidth;
    public double ProductionLineHeight => ViewParameters.ProductionLineHeight;
    public double RobotWidth => ViewParameters.RobotWidth;
    public double RobotHeight => ViewParameters.RobotHeight;
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
        robotMovementTimer = new DispatcherTimer();
        robotMovementTimer.Interval = TimeSpan.FromMilliseconds(Parameters.robotTimerInterval);
        robotMovementTimer.Tick += RobotMovementTimer_Tick;
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.MoveBricks();
        NotifyLegoBricksPoseChange();
    }

    private void ExecuteRobotMovement(Action movement)
    {
        // Im Schrittmodus genau einmal bewegen.
        if (!continuousMovementEnabled)
        {
            movement();
            NotifyRobotPoseChanged();
            NotifyLegoBricksPoseChange();
            return;
        }

        // Im kontinuierlichen Modus die Bewegung speichern.
        currentRobotMovement = movement;

        // Sofort den ersten Schritt ausführen.
        movement();
        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();

        // Anschließend die Bewegung regelmäßig wiederholen.
        robotMovementTimer.Start();
    }

    private void RobotMovementTimer_Tick(object? sender, EventArgs e)
    {
        currentRobotMovement?.Invoke();
        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }

    public void MoveUp()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.up));
    }
    public void MoveDown()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.down));
    }
    public void MoveLeft()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.left));
    }
    public void MoveRight()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.right));
    }
    public void MoveUpLeft()
    {
        ExecuteRobotMovement(() => {
            factory.RobotBewegen(Direction.left);
            factory.RobotBewegen(Direction.up);
        });
    }
    public void MoveUpRight()
    {
        ExecuteRobotMovement(() =>
        {
            factory.RobotBewegen(Direction.right);
            factory.RobotBewegen(Direction.up);
        });
    }

    public void MoveDownLeft()
    {
        ExecuteRobotMovement(() =>
        {
            factory.RobotBewegen(Direction.left);
            factory.RobotBewegen(Direction.down);
        });
    }

    public void MoveDownRight()
    {
        ExecuteRobotMovement(() =>
        {
            factory.RobotBewegen(Direction.right);
            factory.RobotBewegen(Direction.down);
        });
    }
    public void StopButton()
    {
        StopContinuousRobotMovement();
        factory.RobotBewegen(Direction.stop);
        NotifyRobotPoseChanged();
    }
    public void TurnLeft()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.turnLeft));
    }

    public void TurnRight()
    {
        ExecuteRobotMovement(() => factory.RobotBewegen(Direction.turnRight));
    }

    public void TurnToZeroDegrees()
    {
        StopContinuousRobotMovement();
        factory.RobotBewegen(Direction.zeroDegrees);
        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }

    private void StopContinuousRobotMovement()
    {
        robotMovementTimer.Stop();
        currentRobotMovement = null;
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
    public bool IsStepMovementSelected
    {
        get => !continuousMovementEnabled;

        set
        {
            if (value)
                SetRobotMovementMode(false);
        }
    }

    public bool IsContinuousMovementSelected
    {
        get => continuousMovementEnabled;

        set
        {
            if (value)
                SetRobotMovementMode(true);
        }
    }
    private void SetRobotMovementMode(bool continuous)
    {
        if (continuousMovementEnabled == continuous)
            return;
        continuousMovementEnabled = continuous;

        // Eine eventuell laufende Bewegung beenden.
        StopContinuousRobotMovement();

        // Beide Auswahlkreise aktualisieren.
        OnPropertyChanged(nameof(IsStepMovementSelected));
        OnPropertyChanged(nameof(IsContinuousMovementSelected));
    }

    public void Undock()
    {
        factory.Undock();
        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }
}
