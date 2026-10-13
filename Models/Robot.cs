using System.Drawing;

namespace MyAvaloniaApp.Models;

public class Robot : Beweger
{
    public Robot(Point location, Rectangle borders) : base(location, borders)
    {
        //this.location = location;
        //this.borders = borders;
        Location = location;                            //Properties -> Location ist Methode, die location als value verwendet (Alternative zu this->): Macht den Code flexibler
        Borders = borders;
    }
    public override void Bewegen(Parameters.Direction direction)
    {
        base.Bewegen(direction);
    }
}