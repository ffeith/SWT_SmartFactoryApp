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
    public double RecycleLineWidth => ViewParameters.RecycleLineWidth;
    public double RecycleLineHeight => ViewParameters.RecycleLineHeight;
    public double DeliveryLineWidth => ViewParameters.DeliveryLineWidth;
    public double DeliveryLineHeight => ViewParameters.DeliveryLineHeight;
    public double RobotWidth => ViewParameters.RobotWidth;
    public double RobotHeight => ViewParameters.RobotHeight;
    public List<LegoBricksViewModel> BricksView { get; } = new List<LegoBricksViewModel>();
    public double CustomerArrowX => factory.CustomerArrowX;
    public double CustomerArrowY => factory.CustomerArrowY;
    public double RecyclingArrowX => factory.RecyclingArrowX;
    public double RecyclingArrowY => factory.RecyclingArrowY;
    public double CustomerArrowWidth => ViewParameters.CustomerArrowWidth;
    public double CustomerArrowHeight => ViewParameters.CustomerArrowHeight;
    public double RecyclingArrowWidth => ViewParameters.RecyclingArrowWidth;
    public double RecyclingArrowHeight => ViewParameters.RecyclingArrowHeight;
    // Gibt an, ob die Produktionslinien bereits eingerichtet wurden.
    private bool bricksVisible;

    // Der Setup-Button ist nach der Initialisierung deaktiviert.
    public bool CanSetupProductionLines => !bricksVisible;

    private string undockInfo = string.Empty;
    public string UndockInfo
    {
        get => undockInfo;
        private set
        {
            if (undockInfo == value)
                return;

            undockInfo = value;
            OnPropertyChanged(nameof(UndockInfo));
        }
    }

    // Bestellte Steine je Farbe.
    public int RedOrder => factory.GetOrder(LegoColor.red);
    public int BlueOrder => factory.GetOrder(LegoColor.blue);
    public int GreenOrder => factory.GetOrder(LegoColor.green);
    public int YellowOrder => factory.GetOrder(LegoColor.yellow);

    // Erfolgreich gelieferte Steine je Farbe.
    public int RedDelivered => factory.GetDelivered(LegoColor.red);
    public int BlueDelivered => factory.GetDelivered(LegoColor.blue);
    public int GreenDelivered => factory.GetDelivered(LegoColor.green);
    public int YellowDelivered => factory.GetDelivered(LegoColor.yellow);

    // Ein Auftrag kann erst nach der Produktion erstellt werden.
    public bool CanCreateOrder => bricksVisible && !factory.HasOrder;
    public void CreateOrder()
    {
        if (!CanCreateOrder)
            return;

        factory.CreateOrder();

        // Auftragstabelle aktualisieren.
        OnPropertyChanged(nameof(RedOrder));
        OnPropertyChanged(nameof(BlueOrder));
        OnPropertyChanged(nameof(GreenOrder));
        OnPropertyChanged(nameof(YellowOrder));

        // Ersten Logistikschritt anzeigen.
        OnPropertyChanged(nameof(CurrentStepInfo));

        // Order-Button deaktivieren.
        OnPropertyChanged(nameof(CanCreateOrder));
    }

    public string CurrentStepInfo
    {
        get
        {
            if (!factory.HasOrder)
                return string.Empty;

            if (factory.OrderComplete)
                return $"Order complete: all {Parameters.orderSize} bricks delivered.";

            if (factory.CurrentBrickColor == null)
                return string.Empty;

            string color = ColorName(factory.CurrentBrickColor.Value);

            string step =
                $"Step {factory.LogisticsStep}/{factory.OrderCount}: ";

            // Sobald der Roboter den richtigen Stein hält,
            // ändert sich die Anweisung.
            if (factory.IsBrickPickedUp)
                return step + $"Bring the {color} brick to delivery.";

            string station = StationName(factory.CurrentStation);

            return step +
                $"Pick the {color} brick from {station} and bring to delivery.";
        }
    }

    private static string ColorName(LegoColor color)
    {
        return color switch
        {
            LegoColor.red => "red",
            LegoColor.green => "green",
            LegoColor.blue => "blue",
            LegoColor.yellow => "yellow",
            _ => "unknown"
        };
    }

    private string StationName(ProductionLine? station)
    {
        if (station == factory.ProductionLineRed)
            return "red production line";

        if (station == factory.ProductionLineGreen)
            return "green production line";

        if (station == factory.ProductionLineBlue)
            return "blue production line";

        if (station == factory.ProductionLineYellow)
            return "yellow production line";

        if (station == factory.ProductionLineRecycle)
            return "recycle line";

        return "unknown line";
    }

    public MainViewModel()
    {
        factory = new Factory();
        factory.BrickDelivered += HandleBrickDelivered;
        factory.TransportMessage += HandleTransportMessage;

        // 16 leere Anzeigeplätze für die später erzeugten Steine vorbereiten.
        for (int i = 0; i < 4 * Parameters.bricksPerLine; i++)
        {
            BricksView.Add(new LegoBricksViewModel());
        }

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        timer.Tick += Timer_Tick;
        robotMovementTimer = new DispatcherTimer();
        robotMovementTimer.Interval = TimeSpan.FromMilliseconds(Parameters.robotTimerInterval);
        robotMovementTimer.Tick += RobotMovementTimer_Tick;
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.LegoBricksBewegen();
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
        OnPropertyChanged(nameof(CurrentStepInfo));
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
        Parameters.UndockResult result = factory.Undock();

        UndockInfo = result switch
        {
            Parameters.UndockResult.Success =>
                "Erfolgreich abgesetzt. Transport gestartet.",

            Parameters.UndockResult.NoBrick =>
                "Kein Stein angedockt. Es gibt nichts abzusetzen.",

            Parameters.UndockResult.NotOnDeliveryLine =>
                "Undocken ist nur auf der Delivery Line möglich.",

            _ => "Unbekannter Fehler beim Undocken."
        };
        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }

    private void HandleBrickDelivered(Parameters.LegoColor color)
    {
        switch (color)
        {
            case LegoColor.red:
                OnPropertyChanged(nameof(RedDelivered));
                break;

            case LegoColor.blue:
                OnPropertyChanged(nameof(BlueDelivered));
                break;

            case LegoColor.green:
                OnPropertyChanged(nameof(GreenDelivered));
                break;

            case LegoColor.yellow:
                OnPropertyChanged(nameof(YellowDelivered));
                break;
        }

        OnPropertyChanged(nameof(CurrentStepInfo));
    }

   private void HandleTransportMessage(string message)
    {
        UndockInfo = message;
        OnPropertyChanged(nameof(CurrentStepInfo));
    }

    public void SetupProductionLines()
    {
        // Ein erneuter Aufruf ist nicht erlaubt.
        if (bricksVisible)
            return;

        // Factory erzeugt die Steine auf allen vier Produktionslinien.
        factory.SetupProductionLines();

        // Alle erzeugten Steine in einer Liste zusammenfassen.
        List<LegoBrick> allBricks = new List<LegoBrick>();

        allBricks.AddRange(factory.RedBricks);
        allBricks.AddRange(factory.YellowBricks);
        allBricks.AddRange(factory.BlueBricks);
        allBricks.AddRange(factory.GreenBricks);

        // Jeden Stein mit einem GUI-ViewModel verbinden.
        for (int i = 0; i < allBricks.Count; i++)
        {
            BricksView[i].SetBrick(allBricks[i]);
        }

        // Setup als abgeschlossen markieren.
        bricksVisible = true;
        OnPropertyChanged(nameof(CanCreateOrder));

        // GUI über den neuen Zustand informieren.
        OnPropertyChanged(nameof(CanSetupProductionLines));

        NotifyLegoBricksPoseChange();
    }
    public void GoToLine()
    {
        // Eventuell laufende kontinuierliche Bewegung beenden.
        StopContinuousRobotMovement();

        if (!factory.GoToLine())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }

    public void GoToDelivery()
    {
        StopContinuousRobotMovement();

        if (!factory.GoToDelivery())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChange();
    }
}
