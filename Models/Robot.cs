using System.Drawing;

namespace MyAvaloniaApp.Models;

internal class Robot
{
    private Point location;
    private Rectangle borders;

    public Point Location                               //lange Syntax
    {
        get {return location;}
        set {location = value;}
    }

    public Rectangle Borders                            //kurze Syntax
    {
        get => borders; set => borders = value;
    }

    public Robot(Point location, Rectangle borders)
    {
        //this.location = location;
        //this.borders = borders;
        Location = location;                            //Properties -> Location ist Methode, die location als value verwendet (Alternative zu this->): Macht den Code flexibler
        Borders = borders;
    }

    public Robot(Point location)
    {
        Location = location;
    }

    public void Bewegen(Parameters.Direction direction)
    {
        switch (direction)
        {
            case Parameters.Direction.up:
                if (location.Y - Parameters.step >= borders.Top)
                    location.Y -= Parameters.step;
                break;
            case Parameters.Direction.down:
                if (location.Y + Parameters.step <= borders.Bottom)
                    location.Y += Parameters.step;
                break;
            case Parameters.Direction.left:
                if (location.X - Parameters.step >= borders.Left)
                    location.X -= Parameters.step;
                break;
            case Parameters.Direction.right:
                if (location.X + Parameters.step <= borders.Right)
                    location.X += Parameters.step;
                break;
            default:
                break;
        }
    }
}