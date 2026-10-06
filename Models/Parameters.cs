namespace MyAvaloniaApp.Models;

internal class Parameters
{
    public enum Direction
    {
        stop = 0,
        up = 1,
        down = 2,
        left = 3,
        right = 4,
        turnLeft = 5,
        turnRight = 6,
    }
    public const int step = 3;
    public const int turnAngle = 90;
}