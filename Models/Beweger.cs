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

        // Zunächst nur die geplante Rotation berechnen.
        double candidateRotation = rotation;

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
                candidateRotation -= Parameters.turnAngle;

                if (candidateRotation <= -360)
                    candidateRotation += 360;

                break;

            case Parameters.Direction.turnRight:
                candidateRotation += Parameters.turnAngle;

                if (candidateRotation >= 360)
                    candidateRotation -= 360;

                break;

            case Parameters.Direction.zeroDegrees:
                candidateRotation = 0;
                break;

            case Parameters.Direction.stop:
                return;

            default:
                return;
        }

        // Vorgeschlagene Position berechnen.
        Point newLoc = new Point(
            location.X + (int)Math.Round(deltaX),
            location.Y + (int)Math.Round(deltaY));

        // Position UND Rotation überprüfen.
        if (!PositionErlaubt(newLoc, candidateRotation))
            return;

        // Erst jetzt die Bewegung übernehmen.
        Location = newLoc;
        rotation = candidateRotation;

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
        if (dockobjekt is not LegoBrick brick)
            return;

        int brickWidth = brick.Size == Parameters.LegoSize.large
            ? BigBrickWidth
            : SmallBrickWidth;

        int brickHeight = brick.Size == Parameters.LegoSize.large
            ? BigBrickHeight
            : SmallBrickHeight;

        Point tcp = Greifpunkt(Location, rotation);

        // Die linke obere Ecke so berechnen,
        // dass die Mitte des Steins am TCP liegt.
        brick.Location = new Point(
            tcp.X - brickWidth / 2,
            tcp.Y - brickHeight / 2);

        // Der Stein dreht sich mit dem Roboter.
        brick.Angle = rotation;
    }
    public bool Kontak(Beweger objekt)
    {
        if (objekt is not LegoBrick brick)
            return false;

        Point tcp = Greifpunkt(Location, rotation);

        int brickWidth = brick.Size == Parameters.LegoSize.large
            ? BigBrickWidth
            : SmallBrickWidth;

        int brickHeight = brick.Size == Parameters.LegoSize.large
            ? BigBrickHeight
            : SmallBrickHeight;

        // Mitte des LEGO-Steins berechnen.
        Point brickCenter = new Point(
            brick.Location.X + brickWidth / 2,
            brick.Location.Y + brickHeight / 2);

        // Prüfen, ob der TCP nahe genug an der Steinmitte liegt.
        bool proximity =
            Math.Abs(tcp.X - brickCenter.X) <= 10
            && Math.Abs(tcp.Y - brickCenter.Y) <= 10;

        if (proximity && dockobjekt == null && !brick.Gedockt)
        {
            brick.Gedockt = true;
            dockobjekt = brick;

            UpdateDockedObject();
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

    protected virtual bool PositionErlaubt(
    Point position,
    double candidateRotation)
    {
        if (this is Robot)
        {
            Rectangle robotBounds = RotatedBounds(
                position,
                candidateRotation,
                RobotWidth,
                RobotHeight);

            return robotBounds.Left >= Borders.Left
                && robotBounds.Top >= Borders.Top
                && robotBounds.Right <= Borders.Right
                && robotBounds.Bottom <= Borders.Bottom;
        }

        return Borders.Contains(position);
    }

    protected static Rectangle RotatedBounds(
        Point topLeft,
        double angle,
        double width,
        double height)
    {
        double angleRad = angle * Math.PI / 180.0;

        // Mittelpunkt des ursprünglichen Rechtecks.
        double centerX = topLeft.X + width / 2.0;
        double centerY = topLeft.Y + height / 2.0;

        // Abmessungen der umschließenden Bounding Box.
        double boundingWidth =
            Math.Abs(width * Math.Cos(angleRad))
            + Math.Abs(height * Math.Sin(angleRad));

        double boundingHeight =
            Math.Abs(width * Math.Sin(angleRad))
            + Math.Abs(height * Math.Cos(angleRad));

        int left = (int)Math.Floor(
            centerX - boundingWidth / 2.0);

        int top = (int)Math.Floor(
            centerY - boundingHeight / 2.0);

        int right = (int)Math.Ceiling(
            centerX + boundingWidth / 2.0);

        int bottom = (int)Math.Ceiling(
            centerY + boundingHeight / 2.0);

        return Rectangle.FromLTRB(
            left,
            top,
            right,
            bottom);
    }
}