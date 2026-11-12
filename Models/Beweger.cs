using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyAvaloniaApp.Models;

// Beweger ist die gemeinsame Basisklasse für Roboter und LEGO-Steine.
// Sie enthält Funktionen für Bewegung, Drehung, Kollisionen und Andocken.
public class Beweger
{
    // Aktuelle Position und Ausrichtung des Objekts.
    private Pose pose;

    // Bereich, in dem sich das Objekt befinden darf.
    private Rectangle borders;

    // Objekt, das aktuell an diesem Beweger angedockt ist.
    // Beim Roboter ist das normalerweise ein LEGO-Stein.
    private Beweger? dockobjekt;

    // Winkel zwischen der ursprünglichen Ausrichtung des Steins
    // und der aktuellen Ausrichtung des Roboters.
    // Dadurch behält der Stein beim Tragen seine relative Ausrichtung.
    private int dockWinkelOffset;

    // Produktionslinien, die bei der Bewegung Hindernisse darstellen.
    private readonly IReadOnlyList<ProductionLine> productionLines;

    // Linien, an denen der Roboter Steine aufnehmen darf.
    private readonly IReadOnlyList<ProductionLine> dockLines;

    // Zusätzlicher Bereich um eine Linie, in dem Andocken erlaubt ist.
    private const int LinienToleranz = 25;

    // Maximaler Abstand zwischen Greifpunkt und Steinmittelpunkt,
    // damit ein Stein aufgenommen werden kann.
    private const int GreifToleranz = 10;


    // ------------------------------------------------------------
    // Eigenschaften
    // ------------------------------------------------------------

    // Position und Ausrichtung des Objekts.
    public Pose Pose
    {
        get => pose;
        set => pose = value;
    }

    // Position ohne Ausrichtung.
    // Wird die Position verändert, bleibt der bisherige Winkel erhalten.
    public Point Location
    {
        get => pose.Location;
        set => pose = new Pose(value.X, value.Y, pose.Angle);
    }

    // X- und Y-Koordinate der aktuellen Position.
    public int X => pose.X;
    public int Y => pose.Y;

    // Aktueller Drehwinkel in Grad.
    public int Angle
    {
        get => pose.Angle;
        set => pose = new Pose(pose.X, pose.Y, value);
    }

    // Erlaubter Bewegungsbereich.
    public Rectangle Borders
    {
        get => borders;
        set => borders = value;
    }

    // Gibt das aktuell angedockte Objekt zurück.
    // Wenn nichts angedockt ist, ist der Wert null.
    public Beweger? Dockobjekt => dockobjekt;


    // ------------------------------------------------------------
    // Konstruktoren
    // ------------------------------------------------------------

    // Konstruktor mit Position und Bewegungsgrenzen.
    // Es werden keine Produktions- oder Docklinien übergeben.
    public Beweger(Point location, Rectangle borders)
        : this(
            new Pose(location.X, location.Y, 0),
            borders,
            Array.Empty<ProductionLine>(),
            Array.Empty<ProductionLine>())
    {
    }

    // Konstruktor mit Position, Bewegungsgrenzen und Linien.
    public Beweger(
        Point location,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines,
        IReadOnlyList<ProductionLine> dockLines)
        : this(
            new Pose(location.X, location.Y, 0),
            borders,
            productionLines,
            dockLines)
    {
    }

    // Konstruktor mit vollständiger Pose und Bewegungsgrenzen.
    public Beweger(Pose pose, Rectangle borders)
        : this(
            pose,
            borders,
            Array.Empty<ProductionLine>(),
            Array.Empty<ProductionLine>())
    {
    }

    // Hauptkonstruktor.
    // Hier werden alle benötigten Informationen gespeichert.
    public Beweger(
        Pose pose,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines,
        IReadOnlyList<ProductionLine> dockLines)
    {
        this.pose = pose;
        this.borders = borders;
        this.productionLines = productionLines;
        this.dockLines = dockLines;
    }


    // ------------------------------------------------------------
    // Bewegung
    // ------------------------------------------------------------

    // Bewegt das Objekt abhängig von seiner aktuellen Ausrichtung.
    public virtual void Bewegen(Parameters.Direction direction)
    {
        // Drehbefehle werden separat behandelt.
        if (direction == Parameters.Direction.turnLeft ||
            direction == Parameters.Direction.turnRight ||
            direction == Parameters.Direction.zeroDegrees)
        {
            Drehen(direction);
            return;
        }

        // Der Winkel muss für die Berechnung in Radiant umgerechnet werden.
        double winkelInRadiant = Angle * Math.PI / 180.0;

        double deltaX;
        double deltaY;

        // Berechnet, wie weit das Objekt in X- und Y-Richtung bewegt wird.
        switch (direction)
        {
            case Parameters.Direction.up:
                deltaX = Math.Sin(winkelInRadiant) * Parameters.step;
                deltaY = -Math.Cos(winkelInRadiant) * Parameters.step;
                break;

            case Parameters.Direction.down:
                deltaX = -Math.Sin(winkelInRadiant) * Parameters.step;
                deltaY = Math.Cos(winkelInRadiant) * Parameters.step;
                break;

            case Parameters.Direction.left:
                deltaX = -Math.Cos(winkelInRadiant) * Parameters.step;
                deltaY = -Math.Sin(winkelInRadiant) * Parameters.step;
                break;

            case Parameters.Direction.right:
                deltaX = Math.Cos(winkelInRadiant) * Parameters.step;
                deltaY = Math.Sin(winkelInRadiant) * Parameters.step;
                break;

            // Bei einer unbekannten oder nicht unterstützten Richtung
            // findet keine Bewegung statt.
            default:
                return;
        }

        // Berechnet die neue Position.
        Point neuePosition = new(
            Location.X + (int)Math.Round(deltaX),
            Location.Y + (int)Math.Round(deltaY));

        // Für den Roboter müssen zusätzliche Prüfungen durchgeführt werden.
        if (this is Robot)
        {
            Pose neuePose = new(
                neuePosition.X,
                neuePosition.Y,
                Angle);

            // Prüft Grenzen, Hindernisse und gegebenenfalls den getragenen Stein.
            if (!IstRoboterPoseGueltig(neuePose))
                return;
        }
        else
        {
            // Bei anderen Objekten genügt die Prüfung der neuen Position.
            if (!borders.Contains(neuePosition))
                return;
        }

        // Die Bewegung ist erlaubt. Die neue Position wird übernommen.
        Location = neuePosition;

        // Falls ein Objekt angedockt ist, muss es dem Beweger folgen.
        DockobjektNachziehen();
    }


    // ------------------------------------------------------------
    // Drehung
    // ------------------------------------------------------------

    // Verändert den Drehwinkel des Objekts.
    private void Drehen(Parameters.Direction direction)
    {
        int neuerWinkel = Angle;

        switch (direction)
        {
            case Parameters.Direction.turnLeft:
                neuerWinkel -= Parameters.rotationStep;

                // Verhindert Winkelwerte außerhalb des Bereichs.
                if (neuerWinkel <= -360)
                    neuerWinkel += 360;

                break;

            case Parameters.Direction.turnRight:
                neuerWinkel += Parameters.rotationStep;

                if (neuerWinkel >= 360)
                    neuerWinkel -= 360;

                break;

            case Parameters.Direction.zeroDegrees:
                neuerWinkel = 0;
                break;

            default:
                return;
        }

        // Beim Roboter muss auch die neue Ausrichtung geprüft werden.
        // Eine Drehung ist beispielsweise nicht erlaubt, wenn dadurch
        // ein Teil des Roboters außerhalb der Fabrik liegen würde.
        if (this is Robot)
        {
            Pose neuePose = new(X, Y, neuerWinkel);

            if (!IstRoboterPoseGueltig(neuePose))
                return;
        }

        // Die Drehung ist erlaubt.
        Angle = neuerWinkel;

        // Ein angedockter Stein übernimmt die neue Ausrichtung.
        DockobjektNachziehen();
    }


    // ------------------------------------------------------------
    // Freie Bewegung
    // ------------------------------------------------------------

    // Bewegt das Objekt um eine frei angegebene Strecke.
    //
    // Diese Methode wird beispielsweise von den Produktionslinien
    // verwendet, um LEGO-Steine mit dem Förderband zu bewegen.
    //
    // Anders als bei Bewegen(Direction) werden hier keine
    // Bewegungsgrenzen oder Hindernisse geprüft.
    public virtual void Bewegen(double deltaX, double deltaY)
    {
        Location = new Point(
            Location.X + (int)Math.Round(deltaX),
            Location.Y + (int)Math.Round(deltaY));
    }


    // ------------------------------------------------------------
    // Greifpunkt und Andocken
    // ------------------------------------------------------------

    // Berechnet den Greifpunkt (TCP) an der Vorderseite des Roboters.
    //
    // Der Greifpunkt befindet sich in der Mitte der Vorderseite.
    // Er wird abhängig vom Drehwinkel des Roboters berechnet.
    private static Point Greifpunkt(Point robotLocation, int robotAngle)
    {
        double winkelInRadiant = robotAngle * Math.PI / 180.0;

        // Mittelpunkt des Roboters.
        double centerX =
            robotLocation.X + ViewParameters.RobotWidth / 2.0;

        double centerY =
            robotLocation.Y + ViewParameters.RobotHeight / 2.0;

        // Position des Greifpunkts vor dem Roboter.
        double tcpX =
            centerX +
            ViewParameters.RobotHeight / 2.0 *
            Math.Sin(winkelInRadiant);

        double tcpY =
            centerY -
            ViewParameters.RobotHeight / 2.0 *
            Math.Cos(winkelInRadiant);

        return new Point(
            (int)Math.Round(tcpX),
            (int)Math.Round(tcpY));
    }


    // Aktualisiert die Position eines angedockten LEGO-Steins.
    //
    // Diese Methode wird nach jeder Bewegung und Drehung des Roboters
    // aufgerufen. Dadurch bleibt der Stein am Greifpunkt befestigt.
    private void DockobjektNachziehen()
    {
        if (dockobjekt is not LegoBrick brick)
            return;

        // Größe des LEGO-Steins bestimmen.
        int brickWidth = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickWidth
            : ViewParameters.SmallBrickWidth;

        int brickHeight = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickHeight
            : ViewParameters.SmallBrickHeight;

        // Aktuelle Position des Greifpunkts berechnen.
        Point tcp = Greifpunkt(Location, Angle);

        // Der Mittelpunkt des Steins soll auf dem Greifpunkt liegen.
        brick.Location = new Point(
            tcp.X - brickWidth / 2,
            tcp.Y - brickHeight / 2);

        // Auch die Ausrichtung des Steins wird aktualisiert.
        // Der gespeicherte Winkelversatz bleibt dabei erhalten.
        brick.Angle = Angle + dockWinkelOffset;
    }


    // Versucht, einen LEGO-Stein aufzunehmen.
    //
    // Die Methode liefert true zurück, wenn das Andocken erfolgreich war.
    public bool Kontakt(Beweger objekt)
    {
        // Es dürfen nur LEGO-Steine aufgenommen werden.
        if (objekt is not LegoBrick brick)
            return false;

        // Der Stein muss sich auf einer Linie bewegen.
        if (brick.Status != Parameters.BrickStatus.Moving)
            return false;

        // Ein Beweger kann immer nur ein Objekt gleichzeitig tragen.
        if (dockobjekt != null)
            return false;

        // Der Greifpunkt muss sich in der Nähe einer erlaubten Docklinie befinden.
        if (!AufDockLinie())
            return false;

        // Position des Greifpunkts berechnen.
        Point tcp = Greifpunkt(Location, Angle);

        // Größe des Steins bestimmen.
        int brickWidth = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickWidth
            : ViewParameters.SmallBrickWidth;

        int brickHeight = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickHeight
            : ViewParameters.SmallBrickHeight;

        // Mittelpunkt des LEGO-Steins berechnen.
        Point brickCenter = new(
            brick.Location.X + brickWidth / 2,
            brick.Location.Y + brickHeight / 2);

        // Prüfen, ob Greifpunkt und Steinmittelpunkt nah genug beieinander liegen.
        bool nahGenug =
            Math.Abs(tcp.X - brickCenter.X) <= GreifToleranz &&
            Math.Abs(tcp.Y - brickCenter.Y) <= GreifToleranz;

        if (!nahGenug)
            return false;

        // Winkelunterschied zwischen Roboter und Stein speichern.
        // So kann der Stein beim Tragen seine relative Ausrichtung behalten.
        dockWinkelOffset = brick.Angle - Angle;

        // Stein als angedockt markieren.
        brick.Andocken();

        // Verbindung zwischen Roboter und Stein herstellen.
        dockobjekt = brick;

        // Stein direkt an den Greifpunkt setzen.
        DockobjektNachziehen();

        return true;
    }


    // ------------------------------------------------------------
    // Prüfung der Docklinien
    // ------------------------------------------------------------

    // Prüft, ob sich ein Punkt innerhalb oder nahe an einer Linie befindet.
    //
    // Die Linie kann gedreht sein. Deshalb wird der Punkt zuerst
    // in das lokale Koordinatensystem der Linie umgerechnet.
    private static bool AufLinie(
        Point point,
        Pose linePose,
        double lineWidth,
        double lineHeight,
        double tolerance)
    {
        double winkelInRadiant = linePose.Angle * Math.PI / 180.0;

        // Mittelpunkt der Linie.
        double centerX = linePose.X + lineWidth / 2.0;
        double centerY = linePose.Y + lineHeight / 2.0;

        // Abstand des Punktes zum Linienmittelpunkt.
        double deltaX = point.X - centerX;
        double deltaY = point.Y - centerY;

        // Umrechnung in das lokale Koordinatensystem der Linie.
        // Die Rotation erfolgt mit dem negativen Linienwinkel.
        double localX =
            deltaX * Math.Cos(winkelInRadiant) +
            deltaY * Math.Sin(winkelInRadiant);

        double localY =
            -deltaX * Math.Sin(winkelInRadiant) +
            deltaY * Math.Cos(winkelInRadiant);

        // Der Punkt muss innerhalb der halben Linienbreite und -höhe liegen.
        // Die Toleranz vergrößert den erlaubten Bereich.
        return
            Math.Abs(localX) <= lineWidth / 2.0 + tolerance &&
            Math.Abs(localY) <= lineHeight / 2.0 + tolerance;
    }


    // Prüft, ob sich der Greifpunkt in der Nähe einer erlaubten Docklinie befindet.
    private bool AufDockLinie()
    {
        Point tcp = Greifpunkt(Location, Angle);

        foreach (ProductionLine line in dockLines)
        {
            // Die Delivery Line hat andere Abmessungen als die Produktionslinien.
            bool istDeliveryLine =
                line.ProductionLinePose.X ==
                ViewParameters.ProductionLinePoseDelivery.X &&
                line.ProductionLinePose.Y ==
                ViewParameters.ProductionLinePoseDelivery.Y;

            double width = istDeliveryLine
                ? ViewParameters.DeliveryLineWidth
                : ViewParameters.ProductionLineWidth;

            double height = istDeliveryLine
                ? ViewParameters.DeliveryLineHeight
                : ViewParameters.ProductionLineHeight;

            if (AufLinie(
                tcp,
                line.ProductionLinePose,
                width,
                height,
                LinienToleranz))
            {
                return true;
            }
        }

        return false;
    }


    // Prüft, ob der Greifpunkt direkt auf der Delivery Line liegt.
    //
    // Anders als bei der normalen Aufnahme wird hier keine zusätzliche
    // Toleranz verwendet. Der Stein darf nur auf der Delivery Line
    // abgesetzt werden.
    public bool AufDeliveryLinie()
    {
        Point tcp = Greifpunkt(Location, Angle);

        return AufLinie(
            tcp,
            ViewParameters.ProductionLinePoseDelivery,
            ViewParameters.DeliveryLineWidth,
            ViewParameters.DeliveryLineHeight,
            0);
    }


    // ------------------------------------------------------------
    // Grenzen des getragenen Objekts
    // ------------------------------------------------------------

    // Prüft, ob der getragene Stein bei einer geplanten Bewegung
    // vollständig innerhalb der Fabrikgrenzen bleibt.
    private bool DockobjektInnerhalbGrenzen(
        Point robotPosition,
        int robotAngle)
    {
        if (dockobjekt is not LegoBrick brick)
            return true;

        int brickWidth = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickWidth
            : ViewParameters.SmallBrickWidth;

        int brickHeight = brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickHeight
            : ViewParameters.SmallBrickHeight;

        // Position des Greifpunkts für die geplante Roboterposition.
        Point tcp = Greifpunkt(robotPosition, robotAngle);

        // Geplante Position des Steins.
        Point brickLocation = new(
            tcp.X - brickWidth / 2,
            tcp.Y - brickHeight / 2);

        // Die vier Ecken des gedrehten Steins berechnen.
        PointF[] corners = RotatedCorners(
            brickLocation,
            robotAngle + dockWinkelOffset,
            brickWidth,
            brickHeight);

        // Jede Ecke muss innerhalb der Fabrikgrenzen liegen.
        foreach (PointF corner in corners)
        {
            if (corner.X < borders.Left ||
                corner.Y < borders.Top ||
                corner.X >= borders.Right ||
                corner.Y >= borders.Bottom)
            {
                return false;
            }
        }

        return true;
    }


    // ------------------------------------------------------------
    // Andocken lösen
    // ------------------------------------------------------------

    // Löst die Verbindung zwischen Roboter und LEGO-Stein.
    //
    // Gibt den zuvor getragenen Stein zurück.
    // Wenn kein Stein angedockt ist, wird null zurückgegeben.
    public LegoBrick? Undock()
    {
        if (dockobjekt is not LegoBrick brick)
            return null;

        // Der Stein kann sich anschließend wieder normal bewegen.
        brick.Status = Parameters.BrickStatus.Moving;

        // Verbindung und Winkelversatz zurücksetzen.
        dockobjekt = null;
        dockWinkelOffset = 0;

        return brick;
    }


    // ------------------------------------------------------------
    // Position direkt setzen
    // ------------------------------------------------------------

    // Setzt Position und Ausrichtung direkt.
    //
    // Diese Methode wird beispielsweise verwendet, wenn der Roboter
    // automatisch vor einer Produktionslinie positioniert wird.
    public void SetzePose(Pose newPose)
    {
        pose = newPose;

        // Ein angedockter Stein muss ebenfalls an die neue Position.
        DockobjektNachziehen();
    }


    // ------------------------------------------------------------
    // Prüfung der Roboterbewegung
    // ------------------------------------------------------------

    // Prüft, ob eine geplante Roboterposition gültig ist.
    //
    // Dabei werden drei Dinge geprüft:
    // 1. Der Roboter muss vollständig innerhalb der Fabrik bleiben.
    // 2. Der Roboter darf keine Produktionslinie berühren.
    // 3. Ein getragener Stein muss ebenfalls innerhalb der Grenzen bleiben.
    //
    // Die Methode wird sowohl bei Bewegungen als auch bei Drehungen verwendet.
    private bool IstRoboterPoseGueltig(Pose geplantePose)
    {
        // Die vier Ecken des Roboters für die geplante Pose berechnen.
        PointF[] robotCorners = RotatedCorners(
            geplantePose.Location,
            geplantePose.Angle,
            ViewParameters.RobotWidth,
            ViewParameters.RobotHeight);

        // Prüfen, ob alle vier Ecken innerhalb der Fabrikgrenzen liegen.
        foreach (PointF corner in robotCorners)
        {
            if (!borders.Contains(
                (int)Math.Floor(corner.X),
                (int)Math.Floor(corner.Y)))
            {
                return false;
            }
        }

        // Prüfen, ob der Roboter eine Produktionslinie berührt.
        if (KollidiertMitProduktionslinie(geplantePose))
            return false;

        // Falls ein Stein getragen wird, muss auch dieser innerhalb
        // der Fabrikgrenzen bleiben.
        if (!DockobjektInnerhalbGrenzen(
            geplantePose.Location,
            geplantePose.Angle))
        {
            return false;
        }

        return true;
    }


    // Prüft, ob der Roboter mit einer Produktionslinie kollidiert.
    private bool KollidiertMitProduktionslinie(Pose robotPose)
    {
        // Eckpunkte des Roboters berechnen.
        PointF[] robotCorners = RotatedCorners(
            robotPose.Location,
            robotPose.Angle,
            ViewParameters.RobotWidth,
            ViewParameters.RobotHeight);

        foreach (ProductionLine line in productionLines)
        {
            // Die Delivery Line besitzt eigene Abmessungen.
            bool istDeliveryLine =
                line.ProductionLinePose.X ==
                ViewParameters.ProductionLinePoseDelivery.X &&
                line.ProductionLinePose.Y ==
                ViewParameters.ProductionLinePoseDelivery.Y;

            double width = istDeliveryLine
                ? ViewParameters.DeliveryLineWidth
                : ViewParameters.ProductionLineWidth;

            double height = istDeliveryLine
                ? ViewParameters.DeliveryLineHeight
                : ViewParameters.ProductionLineHeight;

            // Eckpunkte der aktuellen Produktionslinie berechnen.
            PointF[] lineCorners = RotatedCorners(
                line.ProductionLinePose.Location,
                line.ProductionLinePose.Angle,
                width,
                height);

            // Überschneiden sich die beiden Rechtecke?
            if (RechteckeUeberlappen(robotCorners, lineCorners))
                return true;
        }

        return false;
    }


    // ------------------------------------------------------------
    // Geometrische Hilfsmethoden
    // ------------------------------------------------------------

    // Berechnet die vier Eckpunkte eines gedrehten Rechtecks.
    //
    // topLeft ist die Position der linken oberen Ecke vor der Drehung.
    // Die Drehung erfolgt um den Mittelpunkt des Rechtecks.
    private static PointF[] RotatedCorners(
        Point topLeft,
        double angle,
        double width,
        double height)
    {
        double winkelInRadiant = angle * Math.PI / 180.0;

        // Mittelpunkt des Rechtecks.
        double centerX = topLeft.X + width / 2.0;
        double centerY = topLeft.Y + height / 2.0;

        double halfWidth = width / 2.0;
        double halfHeight = height / 2.0;

        // Sinus und Kosinus werden für die Drehung benötigt.
        double cos = Math.Cos(winkelInRadiant);
        double sin = Math.Sin(winkelInRadiant);

        // Dreht einen Punkt um den Mittelpunkt des Rechtecks.
        PointF Rotate(double x, double y)
        {
            return new PointF(
                (float)(centerX + x * cos - y * sin),
                (float)(centerY + x * sin + y * cos));
        }

        // Die vier Ecken werden relativ zum Mittelpunkt angegeben.
        return new[]
        {
            Rotate(-halfWidth, -halfHeight),
            Rotate(halfWidth, -halfHeight),
            Rotate(halfWidth, halfHeight),
            Rotate(-halfWidth, halfHeight)
        };
    }


    // Prüft, ob sich zwei gedrehte Rechtecke überschneiden.
    //
    // Dafür wird das Separating Axis Theorem (SAT) verwendet.
    // Es prüft, ob es eine gedachte Trennlinie gibt,
    // durch die beide Rechtecke voneinander getrennt werden.
    private static bool RechteckeUeberlappen(
        PointF[] rectangleA,
        PointF[] rectangleB)
    {
        // Gibt es eine Trennachse, überlappen die Rechtecke nicht.
        return
            !HasSeparatingAxis(rectangleA, rectangleB) &&
            !HasSeparatingAxis(rectangleB, rectangleA);
    }


    // Sucht nach einer Trennachse zwischen zwei Rechtecken.
    //
    // Eine Trennachse ist eine Richtung, in der die Projektionen
    // der beiden Rechtecke nicht überlappen.
    private static bool HasSeparatingAxis(
        PointF[] polygon,
        PointF[] other)
    {
        // Jede Rechteckkante liefert eine mögliche Trennachse.
        for (int i = 0; i < polygon.Length; i++)
        {
            PointF first = polygon[i];
            PointF second = polygon[(i + 1) % polygon.Length];

            // Senkrechte Richtung zur aktuellen Kante berechnen.
            double axisX = -(second.Y - first.Y);
            double axisY = second.X - first.X;

            // Kleinster und größter Projektionswert des ersten Rechtecks.
            double minA = double.PositiveInfinity;
            double maxA = double.NegativeInfinity;

            // Kleinster und größter Projektionswert des zweiten Rechtecks.
            double minB = double.PositiveInfinity;
            double maxB = double.NegativeInfinity;

            // Alle Ecken des ersten Rechtecks auf die Achse projizieren.
            foreach (PointF point in polygon)
            {
                double projection =
                    point.X * axisX + point.Y * axisY;

                minA = Math.Min(minA, projection);
                maxA = Math.Max(maxA, projection);
            }

            // Alle Ecken des zweiten Rechtecks auf dieselbe Achse projizieren.
            foreach (PointF point in other)
            {
                double projection =
                    point.X * axisX + point.Y * axisY;

                minB = Math.Min(minB, projection);
                maxB = Math.Max(maxB, projection);
            }

            // Wenn sich die Projektionsbereiche nicht überschneiden,
            // sind die Rechtecke voneinander getrennt.
            if (maxA <= minB || maxB <= minA)
                return true;
        }

        // Es wurde keine Trennachse gefunden.
        return false;
    }
}