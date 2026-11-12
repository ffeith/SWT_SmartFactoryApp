namespace MyAvaloniaApp.Models;


// Enthält die allgemeinen Konfigurationswerte der SmartFactory.
public static class Parameters
{
    // Bewegungsschritte des Roboters und der LEGO-Steine.
    public const int step = 3;

    // Drehwinkel pro Drehschritt.
    public const int rotationStep = 3;

    // Anzahl der LEGO-Steine pro Produktionslinie.
    public const int bricksPerLine = 4;

    // Anzahl der Steine pro Kundenauftrag.
    public const int orderSize = 8;

    // Zeitintervall für die Bewegung der Produktionslinien.
    public const int timerInterval = 50;
}


// Beschreibt den aktuellen Zustand eines LEGO-Steins.
public enum BrickStatus
{
    Moving,
    Docked,
    ToCustomer,
    ToRecycle
}


// Beschreibt die Bewegungsrichtungen des Roboters.
public enum Direction
{
    stop = 0,
    up = 1,
    down = 2,
    left = 3,
    right = 4,
    turnLeft = 5,
    turnRight = 6,
    zeroDegrees = 7
}


// Verfügbare Farben der LEGO-Steine.
public enum LegoColor
{
    red = 0,
    green = 1,
    blue = 2,
    yellow = 3
}


// Verfügbare Größen der LEGO-Steine.
public enum LegoSize
{
    small = 0,
    large = 1
}


// Beschreibt das Ergebnis eines Undock-Vorgangs.
public enum UndockResult
{
    Success,
    NoBrick,
    NotOnDeliveryLine
}