using System;
using System.Collections.Generic;
using Avalonia.Threading;
using MyAvaloniaApp.Models;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.ViewModels;

// Verbindet die Fabriklogik mit der grafischen Oberfläche.
public partial class MainViewModel : ViewModelBase
{
    private readonly Factory factory;

    private readonly DispatcherTimer timer;
    private readonly DispatcherTimer robotMovementTimer;

    private Action? currentRobotMovement;

    private bool continuousMovementEnabled;
    private bool bricksVisible;

    private string undockInfo = string.Empty;

    // Für jeden möglichen Stein wird ein ViewModel vorbereitet.
    public List<LegoBrickViewModel> BricksView { get; } = new();

    // ------------------------------------------------------------
    // Roboter
    // ------------------------------------------------------------

    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double RobotAngle => factory.RobotAngle;

    // Wird ebenfalls von der bestehenden XAML-Datei verwendet.
    public double Rotation => RobotAngle;

    public double RobotWidth => ViewParameters.RobotWidth;
    public double RobotHeight => ViewParameters.RobotHeight;

    // ------------------------------------------------------------
    // Produktionslinien
    // ------------------------------------------------------------

    public ProductionLine ProductionLineRed =>
        factory.ProductionLineRed;

    public ProductionLine ProductionLineYellow =>
        factory.ProductionLineYellow;

    public ProductionLine ProductionLineBlue =>
        factory.ProductionLineBlue;

    public ProductionLine ProductionLineGreen =>
        factory.ProductionLineGreen;

    public ProductionLine ProductionLineRecycle =>
        factory.ProductionLineRecycle;

    public ProductionLine ProductionLineDelivery =>
        factory.ProductionLineDelivery;

    // Zusätzliche Aliase für alternative Bindungsnamen.
    public ProductionLine RecycleLine =>
        factory.ProductionLineRecycle;

    public ProductionLine DeliveryLine =>
        factory.ProductionLineDelivery;

    public double ProductionLineWidth =>
        ViewParameters.ProductionLineWidth;

    public double ProductionLineHeight =>
        ViewParameters.ProductionLineHeight;

    public double RecycleLineWidth =>
        ViewParameters.RecycleLineWidth;

    public double RecycleLineHeight =>
        ViewParameters.RecycleLineHeight;

    public double DeliveryLineWidth =>
        ViewParameters.DeliveryLineWidth;

    public double DeliveryLineHeight =>
        ViewParameters.DeliveryLineHeight;

    // ------------------------------------------------------------
    // Pfeile
    // ------------------------------------------------------------

    public double CustomerArrowX => factory.CustomerArrowX;
    public double CustomerArrowY => factory.CustomerArrowY;

    public double RecyclingArrowX => factory.RecyclingArrowX;
    public double RecyclingArrowY => factory.RecyclingArrowY;

    public double CustomerArrowWidth =>
        ViewParameters.CustomerArrowWidth;

    public double CustomerArrowHeight =>
        ViewParameters.CustomerArrowHeight;

    public double RecyclingArrowWidth =>
        ViewParameters.RecyclingArrowWidth;

    public double RecyclingArrowHeight =>
        ViewParameters.RecyclingArrowHeight;

    // ------------------------------------------------------------
    // Benutzeroberfläche
    // ------------------------------------------------------------

    public bool CanSetupProductionLines => !bricksVisible;

    public bool CanCreateOrder =>
        bricksVisible && !factory.HasOrder;

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

    public string CurrentStepInfo
    {
        get
        {
            if (!factory.HasOrder)
                return string.Empty;

            if (factory.OrderComplete)
            {
                return $"Order complete: all {Parameters.orderSize} bricks delivered";
            }

            if (factory.IsRecyclingRequired)
            {
                return "The rejected brick must be collected from the recycling line.";
            }

            if (factory.CurrentBrickColor == null)
                return string.Empty;

            string color =
                ColorName(factory.CurrentBrickColor.Value);

            string step =
                $"Step {factory.LogisticsStep}/{factory.OrderCount}: ";

            if (factory.IsBrickPickedUp)
            {
                return step + $"Bring the {color} brick to delivery.";
            }

            string station =
                StationName(factory.CurrentStation);

            return step +
                   $"Pick the {color} brick from {station} and bring to delivery.";
        }
    }

    // ------------------------------------------------------------
    // Bewegungsmodus
    // ------------------------------------------------------------

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

    // ------------------------------------------------------------
    // Bestellübersicht
    // ------------------------------------------------------------

    public int RedOrder => factory.GetOrder(LegoColor.red);
    public int BlueOrder => factory.GetOrder(LegoColor.blue);
    public int GreenOrder => factory.GetOrder(LegoColor.green);
    public int YellowOrder => factory.GetOrder(LegoColor.yellow);

    public int RedDelivered => factory.GetDelivered(LegoColor.red);
    public int BlueDelivered => factory.GetDelivered(LegoColor.blue);
    public int GreenDelivered => factory.GetDelivered(LegoColor.green);
    public int YellowDelivered => factory.GetDelivered(LegoColor.yellow);

    // ------------------------------------------------------------
    // Konstruktor
    // ------------------------------------------------------------

    public MainViewModel()
    {
        factory = new Factory();

        factory.BrickDelivered += HandleBrickDelivered;
        factory.TransportMessage += HandleTransportMessage;

        for (int i = 0;
             i < 4 * Parameters.bricksPerLine;
             i++)
        {
            BricksView.Add(new LegoBrickViewModel());
        }

        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(
                Parameters.timerInterval)
        };

        timer.Tick += Timer_Tick;

        robotMovementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(
                Parameters.robotTimerInterval)
        };

        robotMovementTimer.Tick += RobotMovementTimer_Tick;
    }

    // ------------------------------------------------------------
    // Produktionslinien
    // ------------------------------------------------------------

    public void SetupProductionLines()
    {
        if (bricksVisible)
            return;

        factory.SetupProductionLines();

        List<LegoBrick> allBricks = factory.AlleBricks();

        for (int i = 0; i < allBricks.Count; i++)
        {
            BricksView[i].SetBrick(allBricks[i]);
        }

        bricksVisible = true;

        OnPropertyChanged(nameof(CanSetupProductionLines));
        OnPropertyChanged(nameof(CanCreateOrder));

        NotifyLegoBricksPoseChanged();
    }

    // ------------------------------------------------------------
    // Bestellung
    // ------------------------------------------------------------

    public void CreateOrder()
    {
        if (!CanCreateOrder)
            return;

        factory.CreateOrder();

        NotifyOrderProperties();

        OnPropertyChanged(nameof(CurrentStepInfo));
        OnPropertyChanged(nameof(CanCreateOrder));
    }

    // ------------------------------------------------------------
    // Roboterbewegung
    // ------------------------------------------------------------

    private void ExecuteRobotMovement(Action movement)
    {
        if (!continuousMovementEnabled)
        {
            movement();

            NotifyRobotPoseChanged();
            NotifyLegoBricksPoseChanged();

            return;
        }

        currentRobotMovement = movement;

        movement();

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();

        robotMovementTimer.Start();
    }

    private void RobotMovementTimer_Tick(
        object? sender,
        EventArgs e)
    {
        currentRobotMovement?.Invoke();

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }

    public void MoveUp() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.up));

    public void MoveDown() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.down));

    public void MoveLeft() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.left));

    public void MoveRight() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.right));

    public void MoveUpLeft()
    {
        ExecuteRobotMovement(() =>
        {
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
        NotifyLegoBricksPoseChanged();
    }

    public void TurnLeft() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.turnLeft));

    public void TurnRight() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.turnRight));

    public void TurnToZeroDegrees()
    {
        StopContinuousRobotMovement();

        factory.RobotBewegen(Direction.zeroDegrees);

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }

    private void StopContinuousRobotMovement()
    {
        robotMovementTimer.Stop();

        currentRobotMovement = null;
    }

    private void SetRobotMovementMode(bool continuous)
    {
        if (continuousMovementEnabled == continuous)
            return;

        continuousMovementEnabled = continuous;

        StopContinuousRobotMovement();

        OnPropertyChanged(nameof(IsStepMovementSelected));
        OnPropertyChanged(nameof(IsContinuousMovementSelected));
    }

    // ------------------------------------------------------------
    // Timer für Produktionslinien
    // ------------------------------------------------------------

    public void TimerStart()
    {
        timer.Start();
    }

    public void TimerStop()
    {
        timer.Stop();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.LegoBricksBewegen();

        NotifyLegoBricksPoseChanged();
    }

    // ------------------------------------------------------------
    // Transport
    // ------------------------------------------------------------

    public void GoToLine()
    {
        StopContinuousRobotMovement();

        if (!factory.GoToLine())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }

    public void GoToDelivery()
    {
        StopContinuousRobotMovement();

        if (!factory.GoToDelivery())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }

    public void Undock()
    {
        UndockResult result = factory.Undock();

        UndockInfo = result switch
        {
            UndockResult.Success =>
                "Erfolgreich abgesetzt. Transport gestartet.",

            UndockResult.NoBrick =>
                "Kein Stein angedockt. Es gibt nichts abzusetzen.",

            UndockResult.NotOnDeliveryLine =>
                "Undocken ist nur auf der Delivery Line möglich.",

            _ =>
                "Unbekannter Fehler beim Undocken."
        };

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }

    // ------------------------------------------------------------
    // Aktualisierung der Anzeige
    // ------------------------------------------------------------

    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(RobotAngle));
        OnPropertyChanged(nameof(Rotation));
    }

    private void NotifyLegoBricksPoseChanged()
    {
        foreach (LegoBrickViewModel brickView in BricksView)
        {
            brickView.Refresh();
        }

        OnPropertyChanged(nameof(CurrentStepInfo));
    }

    private void NotifyOrderProperties()
    {
        OnPropertyChanged(nameof(RedOrder));
        OnPropertyChanged(nameof(BlueOrder));
        OnPropertyChanged(nameof(GreenOrder));
        OnPropertyChanged(nameof(YellowOrder));

        OnPropertyChanged(nameof(RedDelivered));
        OnPropertyChanged(nameof(BlueDelivered));
        OnPropertyChanged(nameof(GreenDelivered));
        OnPropertyChanged(nameof(YellowDelivered));
    }

    // ------------------------------------------------------------
    // Factory-Events
    // ------------------------------------------------------------

    private void HandleBrickDelivered(LegoColor color)
    {
        NotifyOrderProperties();

        OnPropertyChanged(nameof(CurrentStepInfo));
        OnPropertyChanged(nameof(CanCreateOrder));
    }

    private void HandleTransportMessage(string message)
    {
        UndockInfo = message;

        OnPropertyChanged(nameof(CurrentStepInfo));
    }

    // ------------------------------------------------------------
    // Hilfsmethoden
    // ------------------------------------------------------------

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
}