namespace MyAvaloniaApp.Models;

public class Parameters
{

        public enum BrickStatus
    {
        Moving,
        Docked,
        ToCustomer,
        ToRecycle
    }

    public enum UndockResult
    {
        Success,
        NoBrick,
        NotOnDeliveryLine
    }
    
    public enum Direction
    {
        stop = 0,
        up = 1,
        down = 2,
        left = 3,
        right = 4,
        turnLeft = 5,
        turnRight = 6,
        zeroDegrees = 7,
    }
    public enum LegoColor
    {
        red = 0,
        green = 1,
        blue = 2,
        yellow = 3,
    }
    public enum LegoSize
    {
        small = 0,
        large = 1,
    }

    public const int step = 3;
    public const int turnAngle = 3;
    public const int bricksPerLine = 4;
    public const int timerInterval = 50;        //Timer Interval in ms
    public const int robotTimerInterval = 150;
}