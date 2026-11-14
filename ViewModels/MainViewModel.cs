using System;
using System.Collections.Generic;
using Avalonia.Threading;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;


// Verbindet die Fachlogik der Factory mit der Benutzeroberfläche.
public partial class MainViewModel : ViewModelBase
{
    // --------------------------------------------------------
    // Felder
    // --------------------------------------------------------

    private readonly Factory factory;

    // Timer für die Bewegung der Produktionslinien.
    private readonly DispatcherTimer timer;

    // Timer für die kontinuierliche Roboterbewegung.
    private readonly DispatcherTimer robotMovementTimer;

    // Aktuell ausgeführter Roboterbewegungsbefehl.
    private Action? currentRobotMovement;

    // Bestimmt den aktiven Bewegungsmodus.
    private bool isStepMode = true;

    // Gibt an, ob die LEGO-Steine bereits angezeigt werden.
    private bool bricksVisible;

    // Statusmeldungen für die Oberfläche.
    private string orderInfo = string.Empty;
    private string undockInfo = string.Empty;

    // Zeitabstand zwischen zwei Roboterbewegungen.
    private const int RobotMovementInterval = 150;

    // ViewModels für die Darstellung der 16 LEGO-Steine.
    public List<LegoBrickViewModel> BricksView { get; } = new();


    // --------------------------------------------------------
    // Eigenschaften: Roboter
    // --------------------------------------------------------

    public double RobotX => factory.RobotX;

    public double RobotY => factory.RobotY;

    public double RobotAngle => factory.RobotAngle;

    public double RobotWidth => ViewParameters.RobotWidth;

    public double RobotHeight => ViewParameters.RobotHeight;


    // --------------------------------------------------------
    // Eigenschaften: Produktionslinien
    // --------------------------------------------------------

    // Blaue Produktionslinie.
    public double ProductionBlueX =>
        factory.ProductionLineBlue.Pose.X;

    public double ProductionBlueY =>
        factory.ProductionLineBlue.Pose.Y;

    public double ProductionBlueAngle =>
        factory.ProductionLineBlue.Pose.Angle;


    // Grüne Produktionslinie.
    public double ProductionGreenX =>
        factory.ProductionLineGreen.Pose.X;

    public double ProductionGreenY =>
        factory.ProductionLineGreen.Pose.Y;

    public double ProductionGreenAngle =>
        factory.ProductionLineGreen.Pose.Angle;


    // Rote Produktionslinie.
    public double ProductionRedX =>
        factory.ProductionLineRed.Pose.X;

    public double ProductionRedY =>
        factory.ProductionLineRed.Pose.Y;

    public double ProductionRedAngle =>
        factory.ProductionLineRed.Pose.Angle;


    // Gelbe Produktionslinie.
    public double ProductionYellowX =>
        factory.ProductionLineYellow.Pose.X;

    public double ProductionYellowY =>
        factory.ProductionLineYellow.Pose.Y;

    public double ProductionYellowAngle =>
        factory.ProductionLineYellow.Pose.Angle;


    // Gemeinsame Abmessungen der vier normalen Produktionslinien.
    public double ProductionLineWidth =>
        ViewParameters.ProductionLineWidth;

    public double ProductionLineHeight =>
        ViewParameters.ProductionLineHeight;


    // --------------------------------------------------------
    // Eigenschaften: Recyclinglinie
    // --------------------------------------------------------

    public double ProductionRecycleX =>
        factory.ProductionLineRecycle.Pose.X;

    public double ProductionRecycleY =>
        factory.ProductionLineRecycle.Pose.Y;

    public double ProductionRecycleAngle =>
        factory.ProductionLineRecycle.Pose.Angle;


    // --------------------------------------------------------
    // Eigenschaften: Delivery Line
    // --------------------------------------------------------

    public double ProductionDeliveryX =>
        factory.ProductionLineDelivery.Pose.X;

    public double ProductionDeliveryY =>
        factory.ProductionLineDelivery.Pose.Y;

    public double ProductionDeliveryAngle =>
        factory.ProductionLineDelivery.Pose.Angle;

    public double ProductionDeliveryWidth =>
        ViewParameters.ProductionDeliveryWidth;

    public double ProductionDeliveryHeight =>
        ViewParameters.ProductionDeliveryHeight;


    // --------------------------------------------------------
    // Eigenschaften: Transportlinien
    // --------------------------------------------------------

    // Transportlinie zum Kunden.
    public double ToCustomerX =>
        ViewParameters.ToCustomerLinePosition.X;

    public double ToCustomerY =>
        ViewParameters.ToCustomerLinePosition.Y;

    public double ToCustomerWidth =>
        ViewParameters.ToCustomerLineWidth;

    public double ToCustomerHeight =>
        ViewParameters.ToCustomerLineHeight;


    // Transportlinie zum Recycling.
    public double ToRecycleX =>
        ViewParameters.ToRecycleLinePosition.X;

    public double ToRecycleY =>
        ViewParameters.ToRecycleLinePosition.Y;

    public double ToRecycleWidth =>
        ViewParameters.ToRecycleLineWidth;

    public double ToRecycleHeight =>
        ViewParameters.ToRecycleLineHeight;


    // --------------------------------------------------------
    // Eigenschaften: Bestellung und Lieferübersicht
    // --------------------------------------------------------

    public IReadOnlyList<LegoBrick> Order =>
        factory.LegoOrder;


    // Anzahl der bestellten Steine je Farbe.
    public int RedOrder =>
        factory.GetOrder(LegoColor.red);

    public int GreenOrder =>
        factory.GetOrder(LegoColor.green);

    public int BlueOrder =>
        factory.GetOrder(LegoColor.blue);

    public int YellowOrder =>
        factory.GetOrder(LegoColor.yellow);


    // Anzahl der erfolgreich gelieferten Steine je Farbe.
    public int RedDelivered =>
        factory.GetDelivered(LegoColor.red);

    public int GreenDelivered =>
        factory.GetDelivered(LegoColor.green);

    public int BlueDelivered =>
        factory.GetDelivered(LegoColor.blue);

    public int YellowDelivered =>
        factory.GetDelivered(LegoColor.yellow);


    // Informationen zum aktuellen Auftragsschritt.
    public string CurrentStepInfo => orderInfo;


    // --------------------------------------------------------
    // Eigenschaften: Benutzeroberfläche
    // --------------------------------------------------------

    public bool CanSetupProductionLines =>
        !bricksVisible;

    public bool CanCreateOrder =>
        bricksVisible && !factory.HasOrder;

    public bool CanShuffleBricks =>
        bricksVisible;


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


    // --------------------------------------------------------
    // Eigenschaften: Bewegungsmodus
    // --------------------------------------------------------

    public bool IsStepMovementSelected
    {
        get => isStepMode;

        set
        {
            if (value)
                SetRobotMovementMode(true);
        }
    }


    public bool IsContinuousMovementSelected
    {
        get => !isStepMode;

        set
        {
            if (value)
                SetRobotMovementMode(false);
        }
    }


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public MainViewModel()
    {
        factory = new Factory();

        // Ereignisse der Factory abonnieren.
        factory.BrickDelivered += HandleBrickDelivered;
        factory.TransportMessage += HandleTransportMessage;


        // Für jeden der 16 möglichen LEGO-Steine
        // ein leeres ViewModel erstellen.
        for (int i = 0; i < 4 * Parameters.bricksPerLine; i++)
        {
            BricksView.Add(new LegoBrickViewModel());
        }


        // Timer für die Bewegung der Produktionslinien.
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(
                Parameters.timerInterval)
        };

        timer.Tick += Timer_Tick;


        // Timer für die kontinuierliche Roboterbewegung.
        robotMovementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(
                RobotMovementInterval)
        };

        robotMovementTimer.Tick += RobotMovementTimer_Tick;

        UpdateOrderInfo();
    }


    // --------------------------------------------------------
    // Produktionslinien einrichten
    // --------------------------------------------------------

    // Erzeugt die LEGO-Steine und weist sie ihren ViewModels zu.
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
        OnPropertyChanged(nameof(CanShuffleBricks));

        NotifyLegoBricksPoseChanged();
    }


    // --------------------------------------------------------
    // Bestellung erstellen
    // --------------------------------------------------------

    public void CreateOrder()
    {
        if (!CanCreateOrder)
            return;

        factory.CreateOrder();

        OnPropertyChanged(nameof(Order));

        NotifyOrderProperties();

        UpdateOrderInfo();

        OnPropertyChanged(nameof(CanCreateOrder));
    }


    // --------------------------------------------------------
    // Roboterbewegung
    // --------------------------------------------------------

    // Führt einen Bewegungsbefehl abhängig vom Bewegungsmodus aus.
    private void ExecuteRobotMovement(Action movement)
    {
        // Im Schrittmodus wird der Befehl einmal ausgeführt.
        if (isStepMode)
        {
            movement();

            NotifyRobotPoseChanged();
            NotifyLegoBricksPoseChanged();

            return;
        }

        // Im kontinuierlichen Modus wird der Befehl
        // gespeichert und regelmäßig wiederholt.
        currentRobotMovement = movement;

        movement();

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();

        robotMovementTimer.Start();
    }


    // Wird bei jedem Tick des Roboter-Timers aufgerufen.
    private void RobotMovementTimer_Tick(
        object? sender,
        EventArgs e)
    {
        currentRobotMovement?.Invoke();

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }


    // Grundbewegungen.
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


    // Diagonale Bewegungen.
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


    // Stoppt die kontinuierliche Roboterbewegung.
    public void StopButton()
    {
        StopContinuousRobotMovement();

        factory.RobotBewegen(Direction.stop);

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }


    // Drehbewegungen.
    public void TurnLeft() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.turnLeft));

    public void TurnRight() =>
        ExecuteRobotMovement(
            () => factory.RobotBewegen(Direction.turnRight));


    // Setzt die Roboterrotation auf null Grad.
    public void TurnToZeroDegrees()
    {
        StopContinuousRobotMovement();

        factory.RobotBewegen(Direction.zeroDegrees);

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }


    // --------------------------------------------------------
    // Bewegungsmodus
    // --------------------------------------------------------

    private void StopContinuousRobotMovement()
    {
        robotMovementTimer.Stop();

        currentRobotMovement = null;
    }


    private void SetRobotMovementMode(bool stepMode)
    {
        if (isStepMode == stepMode)
            return;

        isStepMode = stepMode;

        StopContinuousRobotMovement();

        OnPropertyChanged(nameof(IsStepMovementSelected));
        OnPropertyChanged(nameof(IsContinuousMovementSelected));
    }


    // --------------------------------------------------------
    // Timer der Produktionslinien
    // --------------------------------------------------------

    public void TimerStart()
    {
        timer.Start();
    }


    public void TimerStop()
    {
        timer.Stop();
    }


    // Bewegt die Produktionslinien und aktualisiert anschließend
    // die grafische Darstellung der Steine.
    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.LegoBricksBewegen();

        NotifyLegoBricksPoseChanged();
    }


    // --------------------------------------------------------
    // Automatische Positionierung
    // --------------------------------------------------------

    // Positioniert den Roboter vor der benötigten Produktionslinie.
    public void GoToLine()
    {
        StopContinuousRobotMovement();

        if (!factory.GoToLine())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }


    // Positioniert den Roboter vor der Delivery Line.
    public void GoToDelivery()
    {
        StopContinuousRobotMovement();

        if (!factory.GoToDelivery())
            return;

        NotifyRobotPoseChanged();
        NotifyLegoBricksPoseChanged();
    }


    // --------------------------------------------------------
    // Stein absetzen
    // --------------------------------------------------------

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


    // --------------------------------------------------------
    // Steine tauschen (Shufflen)
    // --------------------------------------------------------

    public void Shuffle()
    {
        factory.Shuffle();
        NotifyLegoBricksPoseChanged();
        UpdateOrderInfo();
    }


    // --------------------------------------------------------
    // Grafische Darstellung aktualisieren
    // --------------------------------------------------------

    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(RobotAngle));
    }


    // Aktualisiert die Darstellung aller LEGO-Steine.
    private void NotifyLegoBricksPoseChanged()
    {
        foreach (LegoBrickViewModel brickView in BricksView)
        {
            brickView.Refresh();
        }

        UpdateOrderInfo();
    }


    // Aktualisiert die Bestell- und Lieferübersicht.
    private void NotifyOrderProperties()
    {
        OnPropertyChanged(nameof(RedOrder));
        OnPropertyChanged(nameof(GreenOrder));
        OnPropertyChanged(nameof(BlueOrder));
        OnPropertyChanged(nameof(YellowOrder));

        OnPropertyChanged(nameof(RedDelivered));
        OnPropertyChanged(nameof(GreenDelivered));
        OnPropertyChanged(nameof(BlueDelivered));
        OnPropertyChanged(nameof(YellowDelivered));
    }


    // --------------------------------------------------------
    // Informationen zum aktuellen Auftrag
    // --------------------------------------------------------

    private void UpdateOrderInfo()
    {
        string newInfo;

        if (!factory.HasOrder)
        {
            newInfo = string.Empty;
        }
        else if (factory.OrderComplete)
        {
            newInfo =
                $"Order complete: all {Parameters.orderSize} bricks delivered";
        }
        else if (factory.IsRecyclingRequired)
        {
            newInfo =
                "The rejected brick must be collected from the recycling line.";
        }
        else if (factory.CurrentBrickColor == null)
        {
            newInfo = string.Empty;
        }
        else
        {
            string color = ColorName(factory.CurrentBrickColor.Value);

            string step =
                $"Step {factory.LogisticsStep}/{factory.OrderCount}: ";

            if (factory.IsBrickPickedUp)
            {
                newInfo =
                    step + $"Bring the {color} brick to delivery.";
            }
            else
            {
                string station = StationName(factory.CurrentStation);

                newInfo =
                    step +
                    $"Pick the {color} brick from {station} and bring to delivery.";
            }
        }

        if (orderInfo == newInfo)
            return;

        orderInfo = newInfo;

        OnPropertyChanged(nameof(CurrentStepInfo));
    }


    // Wandelt einen Enum-Wert in einen lesbaren Farbnamen um.
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


    // Gibt den Namen der Produktionsstation zurück.
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


    // --------------------------------------------------------
    // Ereignisse der Factory
    // --------------------------------------------------------

    // Wird nach einer erfolgreichen Lieferung aufgerufen.
    private void HandleBrickDelivered(LegoColor color)
    {
        NotifyOrderProperties();

        OnPropertyChanged(nameof(Order));

        OnPropertyChanged(nameof(CanCreateOrder));

        UpdateOrderInfo();
    }


    // Übernimmt Statusmeldungen der Factory.
    private void HandleTransportMessage(string message)
    {
        UndockInfo = message;

        UpdateOrderInfo();
    }
}