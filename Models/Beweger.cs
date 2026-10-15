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
        public Beweger(Pose pose, Rectangle borders)
        {
            Location = pose.Location;
            Angle = pose.Angle;
            Borders = borders;
        }

        public virtual void Bewegen(Parameters.Direction direction)
        {
            double angleRad = rotation * Math.PI / 180.0;

            double deltaX = 0;
            double deltaY = 0;

            switch (direction)
            {
                case Parameters.Direction.up:
                    deltaX = Math.Sin(angleRad) * Parameters.step;
                    deltaY = -Math.Cos(angleRad) * Parameters.step;
                    break;
                case Parameters.Direction.down:
                    deltaX = -Math.Sin(angleRad) * Parameters.step;
                    deltaY = Math.Cos(angleRad) * Parameters.step;
                    break;
                case Parameters.Direction.left:
                    deltaX = -Math.Cos(angleRad) * Parameters.step;
                    deltaY = -Math.Sin(angleRad) * Parameters.step;
                    break;
                case Parameters.Direction.right:
                    deltaX = Math.Cos(angleRad) * Parameters.step;
                    deltaY = Math.Sin(angleRad) * Parameters.step;
                    break;
                case Parameters.Direction.turnLeft:
                    rotation -= Parameters.turnAngle;
                    if (rotation <= -360)
                        rotation += 360;
                    return;
                case Parameters.Direction.turnRight:
                    rotation += Parameters.turnAngle;
                    if (rotation >= 360)
                        rotation -= 360;
                    return;
                case Parameters.Direction.zeroDegrees:
                    rotation = 0;
                    return;
                case Parameters.Direction.stop:
                default:
                    return;
            }

            int newX = location.X + (int)Math.Round(deltaX);
            int newY = location.Y + (int)Math.Round(deltaY);

            if (newX >= borders.Left && newX <= borders.Right && newY >= borders.Top && newY <= borders.Bottom)
            {
                Location = new Point(newX, newY);
            }
        }
        public virtual void Bewegen(double deltaX, double deltaY)
        {
            int newX = Location.X + (int)Math.Round(deltaX);
            int newY = Location.Y + (int)Math.Round(deltaY);

            Location = new Point(newX, newY);
        }
    }
}