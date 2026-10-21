using System;
using System.Collections;
using System.Drawing;
using static MyAvaloniaApp.Models.ViewParameters;

namespace MyAvaloniaApp.Models;
public class Beweger
{
    private Point location;
    private Rectangle borders;
    private double rotation;
    private double angle;
    private Beweger? dockobjekt;

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
                UpdateDockedObject();
                return;
            case Parameters.Direction.turnRight:
                rotation += Parameters.turnAngle;
                if (rotation >= 360)
                    rotation -= 360;
                UpdateDockedObject();
                return;
            case Parameters.Direction.zeroDegrees:
                rotation = 0;
                UpdateDockedObject();
                return;
            case Parameters.Direction.stop:
                break;
            default:
                break;
        }

        Point newLoc = new Point(
        location.X + (int)Math.Round(deltaX),
        location.Y + (int)Math.Round(deltaY));
        if (!InnerhalbGrenzen(newLoc))
            return;
        Location = newLoc;

        UpdateDockedObject();
    }
    public virtual void Bewegen(double deltaX, double deltaY)
    {
        int newX = Location.X + (int)Math.Round(deltaX);
        int newY = Location.Y + (int)Math.Round(deltaY);

        Location = new Point(newX, newY);
    }
    private static Point Greifpunkt(Point robotLocation, double robotRotation)
    {
        double angleRad = robotRotation * Math.PI / 180.0;

        double centerX = robotLocation.X + RobotWidth / 2.0;
        double centerY = robotLocation.Y + RobotHeight / 2.0;

        double tcpX = centerX
                    + RobotHeight / 2.0 * Math.Sin(angleRad);

        double tcpY = centerY
                    - RobotHeight / 2.0 * Math.Cos(angleRad);

        return new Point(
            (int)Math.Round(tcpX),
            (int)Math.Round(tcpY));
    }

    private void UpdateDockedObject()
    {
        if (dockobjekt != null)
            dockobjekt.Location = Greifpunkt(Location, rotation);
    }
    // private bool kollidiertMitProduktionslinie(Point newLoc)
    // {
    //     if (this is not Robot)
    //     {
    //         return false;
    //     }
    //     Rectangle robot = new Rectangle(newLoc, new Size(RobotWidth, RobotHeight));
    //     return robot.IntersectsWith(ProductionLine);
    // }
    public bool Kontak(Beweger objekt)
    {
        Point tcp = Greifpunkt(location, rotation);
        bool proximity = Math.Abs(tcp.X - objekt.Location.X) <= 10 && Math.Abs(tcp.Y - objekt.Location.Y) <= 10;
        if (proximity && dockobjekt == null && objekt is LegoBrick aBrick && !aBrick.Gedockt)
        {
            aBrick.Gedockt = true;
            dockobjekt = objekt;
            dockobjekt.Location = tcp;
        }
        return proximity;
    }

    public bool Undock()
    {
        if (dockobjekt == null)
            return false;
        if (dockobjekt is LegoBrick aBrick)
            aBrick.Gedockt = false;
        dockobjekt = null;
        return true;
    }

    private bool InnerhalbGrenzen(Point position)
    {
        if (this is Robot)
        {
            return position.X >= borders.Left
                && position.Y >= borders.Top
                && position.X + RobotWidth <= borders.Right
                && position.Y + RobotHeight <= borders.Bottom;
        }

        return borders.Contains(position);
    }
}