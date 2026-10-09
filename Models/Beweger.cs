using System;
using System.Drawing;

namespace MyAvaloniaApp.Models
{
    public class Beweger
    {
        private Point location;
        private Rectangle borders;
        private double rotation;
        private double angle;

        public Point Location                               //lange Syntax
        {
            get {return location;}
            set {location = value;}
        }

        public Rectangle Borders                            //kurze Syntax
        {
            get => borders; set => borders = value;
        }

        public double Rotation
        {
            get {return rotation;}
        }
        public double Angle
        {
            get {return angle;} set {angle = value;}
        }

        public Beweger(Point location, Rectangle borders)
        {
            //this.location = location;
            //this.borders = borders;
            Location = location;                            //Properties -> Location ist Methode, die location als value verwendet (Alternative zu this->): Macht den Code flexibler
            Borders = borders;
        }

        public Beweger(Point location)
        {
            Location = location;
        }
        public Beweger(Pose pose, Rectangle borders)
        {
            Location = pose.Location;
            angle = pose.Angle;
            Borders = ViewParameters.Borders;
        }

        public virtual void Bewegen(Parameters.Direction direction)
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
                case Parameters.Direction.turnLeft:
                    rotation -= Parameters.turnAngle;
                    if(rotation == -360) rotation = 0;
                    break;
                case Parameters.Direction.turnRight:
                    rotation += Parameters.turnAngle;
                    if(rotation == 360) rotation = 0;
                    break;
                default:
                    break;
            }
        }
    }
}