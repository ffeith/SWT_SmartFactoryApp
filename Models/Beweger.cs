using System;
using System.Collections;
using System.Collections.Generic;
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
    private double dockWinkelOffset;
    private readonly IReadOnlyList<ProductionLine> productionLines;

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

    public Beweger(
    Point location,
    Rectangle borders,
    IReadOnlyList<ProductionLine>? productionLines = null)
    {
        Location = location;
        Borders = borders;
        this.productionLines = productionLines ?? Array.Empty<ProductionLine>();
    }

    public Beweger(
        Pose pose,
        Rectangle borders,
        IReadOnlyList<ProductionLine>? productionLines = null)
    {
        Location = pose.Location;
        Angle = pose.Angle;
        Borders = borders;
        this.productionLines = productionLines ?? Array.Empty<ProductionLine>();
    }

    public virtual void Bewegen(Parameters.Direction direction)
    {
        if (direction == Parameters.Direction.turnLeft || direction == Parameters.Direction.turnRight || direction == Parameters.Direction.zeroDegrees)
        {
            Drehen(direction);
            return;
        }
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

        DockobjektNachziehen();
    }

    private void Drehen(Parameters.Direction direction)
    {
        double candidateRotation = rotation;

        switch (direction)
        {
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

            default:
                return;
        }

        // Prüfen, ob die Drehung zulässig ist.
        if (!PositionErlaubt(Location, candidateRotation))
            return;

        rotation = candidateRotation;

        // Einen aufgenommenen Stein nachziehen.
        DockobjektNachziehen();
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

    private void DockobjektNachziehen()
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
        brick.Angle = rotation + dockWinkelOffset;
    }
    public bool Kontakt(Beweger objekt)
    {
        if (objekt is not LegoBrick brick || brick.Status != Parameters.BrickStatus.Moving)
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

        if (proximity && dockobjekt == null)
        {
            dockWinkelOffset = brick.Angle - Rotation;
            brick.Andocken();;
            dockobjekt = brick;
            DockobjektNachziehen();
        }

        return proximity;
    }

    public LegoBrick? Undock()
    {
        if (dockobjekt is not LegoBrick brick)
            return null;
        brick.Status = Parameters.BrickStatus.Moving;
        dockobjekt = null;
        dockWinkelOffset = 0;
        return brick;
    }

    protected virtual bool PositionErlaubt(Point position, double candidateRotation)
    {
        // Für andere Beweger, beispielsweise LEGO-Steine,
        // bleibt die bisherige Grenzprüfung bestehen.
        if (this is not Robot)
            return Borders.Contains(position);

        // 1. Die tatsächlichen Ecken des gedrehten Roboters berechnen.
        PointF[] robotCorners = RotatedCorners(
            position,
            candidateRotation,
            RobotWidth,
            RobotHeight);

        // Prüfen, ob der Roboter innerhalb der Fabrik bleibt.
        foreach (PointF corner in robotCorners)
        {
            if (corner.X < Borders.Left
                || corner.Y < Borders.Top
                || corner.X > Borders.Right
                || corner.Y > Borders.Bottom)
            {
                return false;
            }
        }

        // 2. Kollisionen mit den Produktionslinien prüfen.
        if (KollidiertMitProduktionslinie(position, candidateRotation))
            return false;

        // 3. Falls ein LEGO-Stein aufgenommen ist,
        // prüfen, ob auch dieser innerhalb der Fabrik bleibt.
        if (dockobjekt is LegoBrick brick)
        {
            int brickWidth = brick.Size == Parameters.LegoSize.large
                ? BigBrickWidth
                : SmallBrickWidth;

            int brickHeight = brick.Size == Parameters.LegoSize.large
                ? BigBrickHeight
                : SmallBrickHeight;

            // TCP für die geplante Position und Rotation berechnen.
            Point tcp = Greifpunkt(position, candidateRotation);

            // Die Mitte des Steins liegt am TCP.
            Point brickLocation = new Point(
                tcp.X - brickWidth / 2,
                tcp.Y - brickHeight / 2);

            // Den relativen Winkel des aufgenommenen Steins berücksichtigen.
            double brickRotation =
                candidateRotation + dockWinkelOffset;

            // Ecken des gedrehten LEGO-Steins berechnen.
            PointF[] brickCorners = RotatedCorners(
                brickLocation,
                brickRotation,
                brickWidth,
                brickHeight);

            foreach (PointF corner in brickCorners)
            {
                if (corner.X < Borders.Left
                    || corner.Y < Borders.Top
                    || corner.X > Borders.Right
                    || corner.Y > Borders.Bottom)
                {
                    return false;
                }
            }
        }

        // Alle Prüfungen bestanden.
        return true;
    }
    private bool KollidiertMitProduktionslinie(
    Point robotPosition,
    double robotRotation)
    {
        PointF[] robotCorners = RotatedCorners(
            robotPosition,
            robotRotation,
            RobotWidth,
            RobotHeight);

        foreach (ProductionLine line in productionLines)
        {
            Point lineLocation = new Point(
                line.ProductionLinePose.X,
                line.ProductionLinePose.Y);

            PointF[] lineCorners = RotatedCorners(
                lineLocation,
                line.ProductionLinePose.Angle,
                ProductionLineWidth,
                ProductionLineHeight);

            if (RechteckeÜberlappen(robotCorners, lineCorners))
                return true;
        }

        return false;
    }
    protected static PointF[] RotatedCorners(
    Point topLeft,
    double angle,
    double width,
    double height)
    {
        double angleRad = angle * Math.PI / 180.0;

        double centerX = topLeft.X + width / 2.0;
        double centerY = topLeft.Y + height / 2.0;

        double halfWidth = width / 2.0;
        double halfHeight = height / 2.0;

        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);

        PointF Rotate(double x, double y)
        {
            return new PointF(
                (float)(centerX + x * cos - y * sin),
                (float)(centerY + x * sin + y * cos));
        }

        return new[]
        {
            Rotate(-halfWidth, -halfHeight),
            Rotate( halfWidth, -halfHeight),
            Rotate( halfWidth,  halfHeight),
            Rotate(-halfWidth,  halfHeight)
        };
    }
    private static bool RechteckeÜberlappen(
    PointF[] rectangleA,
    PointF[] rectangleB)
    {
        return !HasSeparatingAxis(rectangleA, rectangleB)
            && !HasSeparatingAxis(rectangleB, rectangleA);
    }

    private static bool HasSeparatingAxis(
        PointF[] polygon,
        PointF[] other)
    {
        for (int i = 0; i < polygon.Length; i++)
        {
            PointF first = polygon[i];
            PointF second = polygon[(i + 1) % polygon.Length];

            double axisX = -(second.Y - first.Y);
            double axisY = second.X - first.X;

            double minA = double.PositiveInfinity;
            double maxA = double.NegativeInfinity;
            double minB = double.PositiveInfinity;
            double maxB = double.NegativeInfinity;

            foreach (PointF point in polygon)
            {
                double projection =
                    point.X * axisX + point.Y * axisY;

                minA = Math.Min(minA, projection);
                maxA = Math.Max(maxA, projection);
            }

            foreach (PointF point in other)
            {
                double projection =
                    point.X * axisX + point.Y * axisY;

                minB = Math.Min(minB, projection);
                maxB = Math.Max(maxB, projection);
            }

            if (maxA <= minB || maxB <= minA)
                return true;
        }

        return false;
    }
}