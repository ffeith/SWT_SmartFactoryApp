using System.Drawing;

namespace MyAvaloniaApp.Models;


// Beschreibt Position und Ausrichtung eines Objekts.
public readonly struct Pose
{
    public int X { get; }

    public int Y { get; }

    public int Angle { get; }

    // Gibt die Position ohne Drehwinkel zurück.
    public Point Location => new(X, Y);


    // Erstellt eine Pose mit Standardwerten.
    public Pose()
    {
        X = 0;
        Y = 0;
        Angle = 0;
    }


    // Erstellt eine Pose mit Position und Drehwinkel.
    public Pose(int x, int y, int angle)
    {
        X = x;
        Y = y;
        Angle = angle;
    }
}


// Enthält die zentralen Positionen und Größen der Fabrik.
public static class ViewParameters
{
    // --------------------------------------------------------
    // Positionen der Fabriklinien
    // --------------------------------------------------------

    // Startposition des Roboters.
    public static readonly Pose RobotPose =
        new(600, 280, 0);

    // Normale Produktionslinien.
    public static readonly Pose RectRedPose =
        new(410, 380, 90);

    public static readonly Pose RectBluePose =
        new(706, 380, 90);

    public static readonly Pose RectGreenPose =
        new(696, 610, 0);

    public static readonly Pose RectYellowPose =
        new(420, 610, 0);

    // Recycling- und Delivery Line.
    public static readonly Pose RectRecyclePose =
        new(558, 465, 0);

    public static readonly Pose RectDeliveryPose =
        new(538, 190, 0);


    // --------------------------------------------------------
    // Transportlinien
    // --------------------------------------------------------

    // Position der Linie zum Kunden.
    public static readonly Pose ToCustomerLinePosition =
        new(694, 190, 0);

    // Position der Linie zum Recycling.
    public static readonly Pose ToRecycleLinePosition =
        new(435, 190, 0);


    // --------------------------------------------------------
    // Bewegungsgrenzen
    // --------------------------------------------------------

    // Äußerer Bewegungsbereich der Fabrik.
    public static readonly Rectangle Borders =
        new(357, 182, 522, 524);


    // --------------------------------------------------------
    // Anfahrwinkel des Roboters
    // --------------------------------------------------------

    public const double GoToRedAngle = 270;

    public const double GoToBlueAngle = 90;

    public const double GoToGreenAngle = 180;

    public const double GoToYellowAngle = 180;

    public const double GoToRecycleAngle = 180;

    public const double GoToDeliveryAngle = 0;


    // --------------------------------------------------------
    // Produktionslinien
    // --------------------------------------------------------

    public const int ProductionLineWidth = 120;

    public const int ProductionLineHeight = 36;


    // Abmessungen der Delivery Line.
    public const int ProductionDeliveryWidth = 150;

    public const int ProductionDeliveryHeight = 50;


    // --------------------------------------------------------
    // Transportlinien
    // --------------------------------------------------------

    public const double ToCustomerLineWidth = 100;

    public const double ToCustomerLineHeight = 50;

    public const double ToRecycleLineWidth = 100;

    public const double ToRecycleLineHeight = 50;


    // --------------------------------------------------------
    // Roboterabmessungen
    // --------------------------------------------------------

    public const int RobotWidth = 38;

    public const int RobotHeight = 80;


    // --------------------------------------------------------
    // LEGO-Abmessungen
    // --------------------------------------------------------

    public const int SmallBrickWidth = 12;

    public const int SmallBrickHeight = 12;

    public const int BigBrickWidth = 20;

    public const int BigBrickHeight = 12;
}