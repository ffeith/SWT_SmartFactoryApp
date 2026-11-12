using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyAvaloniaApp.Models;


// Basisklasse für Roboter und LEGO-Steine.
// Enthält gemeinsame Funktionen für Bewegung, Drehung,
// Kollisionen und das Andocken.
public class Beweger
{
    // --------------------------------------------------------
    // Felder
    // --------------------------------------------------------

    // Zentrale Beschreibung der aktuellen Position.
    private Pose pose;

    // Einzelne Bestandteile der aktuellen Pose.
    private int x;
    private int y;
    private int angle;

    // Bereich, innerhalb dessen sich das Objekt bewegen darf.
    private readonly Rectangle borders;

    // Aktuell angedocktes Objekt.
    private Beweger? dockobjekt;

    // Winkelunterschied zwischen Roboter und getragenem Stein.
    private int dockWinkelOffset;

    // Linien, die bei der Bewegung als Hindernisse gelten.
    private readonly IReadOnlyList<ProductionLine> productionLines;

    // Linien, an denen der Roboter Steine aufnehmen darf.
    private readonly IReadOnlyList<ProductionLine> dockLines;

    // Zusätzlicher erlaubter Abstand zu einer Docklinie.
    private const int LinienToleranz = 25;

    // Maximaler Abstand zwischen Greifpunkt und Steinmittelpunkt.
    private const int GreifToleranz = 10;

    // Abmessungen des Roboters.
    private readonly int RobotWidth = ViewParameters.RobotWidth;
    private readonly int RobotHeight = ViewParameters.RobotHeight;


    // --------------------------------------------------------
    // Eigenschaften
    // --------------------------------------------------------

    // Position und Ausrichtung des Objekts.
    public Pose Pose
    {
        get => pose;
        set => SetzePose(value);
    }

    // Position ohne Drehwinkel.
    public Point Location
    {
        get => new(x, y);
        set => SetzePose(new Pose(value.X, value.Y, angle));
    }

    public int X => x;

    public int Y => y;

    // Aktueller Drehwinkel.
    public int Angle
    {
        get => angle;
        set => SetzePose(new Pose(x, y, value));
    }

    // Bewegungsgrenzen des Objekts.
    public Rectangle Borders => borders;

    // Aktuell angedocktes Objekt.
    public Beweger? Dockobjekt => dockobjekt;


    // --------------------------------------------------------
    // Konstruktoren
    // --------------------------------------------------------

    // Erstellt einen Beweger mit Standardposition.
    public Beweger()
        : this(
            new Pose(),
            ViewParameters.Borders,
            Array.Empty<ProductionLine>(),
            Array.Empty<ProductionLine>())
    {
    }


    // Erstellt einen Beweger mit Position, Grenzen und Linien.
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


    // Erstellt einen Beweger mit Pose und Bewegungsgrenzen.
    public Beweger(Pose pose, Rectangle borders)
        : this(
            pose,
            borders,
            Array.Empty<ProductionLine>(),
            Array.Empty<ProductionLine>())
    {
    }


    // Hauptkonstruktor mit allen benötigten Einstellungen.
    public Beweger(
        Pose pose,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines,
        IReadOnlyList<ProductionLine> dockLines)
    {
        this.borders = borders;

        this.productionLines =
            productionLines ?? throw new ArgumentNullException(nameof(productionLines));

        this.dockLines =
            dockLines ?? throw new ArgumentNullException(nameof(dockLines));

        SetzePose(pose);
    }


    // --------------------------------------------------------
    // Bewegung
    // --------------------------------------------------------

    // Bewegt das Objekt abhängig von seiner Ausrichtung.
    public virtual void Bewegen(Direction direction)
    {
        // Drehbefehle werden separat verarbeitet.
        if (direction == Direction.turnLeft ||
            direction == Direction.turnRight ||
            direction == Direction.zeroDegrees)
        {
            Drehen(direction);
            return;
        }

        double winkel = Angle * Math.PI / 180.0;

        double deltaX;
        double deltaY;

        // Verschiebung für die gewünschte Richtung berechnen.
        switch (direction)
        {
            case Direction.up:
                deltaX = Math.Sin(winkel) * Parameters.step;
                deltaY = -Math.Cos(winkel) * Parameters.step;
                break;

            case Direction.down:
                deltaX = -Math.Sin(winkel) * Parameters.step;
                deltaY = Math.Cos(winkel) * Parameters.step;
                break;

            case Direction.left:
                deltaX = -Math.Cos(winkel) * Parameters.step;
                deltaY = -Math.Sin(winkel) * Parameters.step;
                break;

            case Direction.right:
                deltaX = Math.Cos(winkel) * Parameters.step;
                deltaY = Math.Sin(winkel) * Parameters.step;
                break;

            default:
                return;
        }

        Point neuePosition = new(
            Location.X + (int)Math.Round(deltaX),
            Location.Y + (int)Math.Round(deltaY));

        Pose neuePose = new(
            neuePosition.X,
            neuePosition.Y,
            Angle);

        // Beim Roboter zusätzlich Hindernisse und
        // den möglicherweise getragenen Stein berücksichtigen.
        if (this is Robot)
        {
            if (!IstRoboterPoseGueltig(neuePose))
                return;
        }
        else if (!borders.Contains(neuePosition))
        {
            return;
        }

        SetzePose(neuePose);
    }


    // Bewegt das Objekt um eine frei angegebene Strecke.
    //
    // Wird insbesondere von ProductionLine verwendet,
    // um LEGO-Steine auf dem Förderband zu bewegen.
    public virtual void Bewegen(double deltaX, double deltaY)
    {
        SetzePose(
            new Pose(
                x + (int)Math.Round(deltaX),
                y + (int)Math.Round(deltaY),
                angle));
    }


    // --------------------------------------------------------
    // Drehung
    // --------------------------------------------------------

    // Verändert den Drehwinkel des Objekts.
    private void Drehen(Direction direction)
    {
        int neuerWinkel = Angle;

        switch (direction)
        {
            case Direction.turnLeft:
                neuerWinkel -= Parameters.rotationStep;

                if (neuerWinkel <= -360)
                    neuerWinkel += 360;

                break;

            case Direction.turnRight:
                neuerWinkel += Parameters.rotationStep;

                if (neuerWinkel >= 360)
                    neuerWinkel -= 360;

                break;

            case Direction.zeroDegrees:
                neuerWinkel = 0;
                break;

            default:
                return;
        }

        Pose neuePose = new(X, Y, neuerWinkel);

        // Vor der Drehung prüfen, ob die neue Ausrichtung
        // für den Roboter und einen getragenen Stein erlaubt ist.
        if (this is Robot && !IstRoboterPoseGueltig(neuePose))
            return;

        SetzePose(neuePose);
    }


    // --------------------------------------------------------
    // Greifpunkt und Andocken
    // --------------------------------------------------------

    // Berechnet den Greifpunkt an der Vorderseite des Roboters.
    private Point Greifpunkt(Point robotLocation, int robotAngle)
    {
        double winkel = robotAngle * Math.PI / 180.0;

        double mitteX =
            robotLocation.X + RobotWidth / 2.0;

        double mitteY =
            robotLocation.Y + RobotHeight / 2.0;

        double tcpX =
            mitteX + RobotHeight / 2.0 * Math.Sin(winkel);

        double tcpY =
            mitteY - RobotHeight / 2.0 * Math.Cos(winkel);

        return new Point(
            (int)Math.Round(tcpX),
            (int)Math.Round(tcpY));
    }


    // Ermittelt die Abmessungen eines LEGO-Steins.
    private static Size ErmittleSteingroesse(LegoBrick brick)
    {
        if (brick.Size == LegoSize.large)
        {
            return new Size(
                ViewParameters.BigBrickWidth,
                ViewParameters.BigBrickHeight);
        }

        return new Size(
            ViewParameters.SmallBrickWidth,
            ViewParameters.SmallBrickHeight);
    }


    // Aktualisiert die Position eines angedockten LEGO-Steins.
    private void DockobjektNachziehen()
    {
        if (dockobjekt is not LegoBrick brick)
            return;

        Size steingroesse = ErmittleSteingroesse(brick);

        Point tcp = Greifpunkt(Location, Angle);

        // Den Mittelpunkt des Steins auf den Greifpunkt setzen.
        brick.Location = new Point(
            tcp.X - steingroesse.Width / 2,
            tcp.Y - steingroesse.Height / 2);

        // Den Winkelunterschied zwischen Roboter und Stein beachten.
        brick.Angle = Angle + dockWinkelOffset;
    }


    // Versucht, einen LEGO-Stein aufzunehmen.
    // Gibt true zurück, wenn das Andocken erfolgreich war.
    public bool Kontakt(Beweger objekt)
    {
        // Nur LEGO-Steine dürfen aufgenommen werden.
        if (objekt is not LegoBrick brick)
            return false;

        // Der Stein muss beweglich sein.
        // Außerdem darf kein anderes Objekt angedockt sein.
        if (brick.Status != BrickStatus.Moving ||
            dockobjekt != null)
        {
            return false;
        }

        // Der Greifpunkt muss sich in der Nähe einer Docklinie befinden.
        if (!AufDockLinie())
            return false;

        Point tcp = Greifpunkt(Location, Angle);

        Size steingroesse = ErmittleSteingroesse(brick);

        // Mittelpunkt des Steins berechnen.
        Point steinMitte = new(
            brick.Location.X + steingroesse.Width / 2,
            brick.Location.Y + steingroesse.Height / 2);

        // Abstand zum Greifpunkt prüfen.
        bool nahGenug =
            Math.Abs(tcp.X - steinMitte.X) <= GreifToleranz &&
            Math.Abs(tcp.Y - steinMitte.Y) <= GreifToleranz;

        if (!nahGenug)
            return false;

        // Winkelunterschied speichern.
        dockWinkelOffset = brick.Angle - Angle;

        brick.Andocken();

        dockobjekt = brick;

        DockobjektNachziehen();

        return true;
    }


    // --------------------------------------------------------
    // Docklinien
    // --------------------------------------------------------

    // Prüft, ob ein Punkt innerhalb oder nahe einer Linie liegt.
    //
    // Die Ausrichtung der Linie wird berücksichtigt.
    private static bool AufLinie(
        Point point,
        Pose linePose,
        double lineWidth,
        double lineHeight,
        double tolerance)
    {
        double winkel = linePose.Angle * Math.PI / 180.0;

        double mitteX = linePose.X + lineWidth / 2.0;
        double mitteY = linePose.Y + lineHeight / 2.0;

        double deltaX = point.X - mitteX;
        double deltaY = point.Y - mitteY;

        // Punkt in das lokale Koordinatensystem der Linie umrechnen.
        double localX =
            deltaX * Math.Cos(winkel) +
            deltaY * Math.Sin(winkel);

        double localY =
            -deltaX * Math.Sin(winkel) +
            deltaY * Math.Cos(winkel);

        return
            Math.Abs(localX) <= lineWidth / 2.0 + tolerance &&
            Math.Abs(localY) <= lineHeight / 2.0 + tolerance;
    }


    // Gibt die Größe einer Produktionslinie zurück.
    // Die Delivery Line hat eigene Abmessungen.
    private static Size ErmittleLiniengroesse(ProductionLine line)
    {
        if (IstDeliveryLine(line))
        {
            return new Size(
                ViewParameters.ProductionDeliveryWidth,
                ViewParameters.ProductionDeliveryHeight);
        }

        return new Size(
            ViewParameters.ProductionLineWidth,
            ViewParameters.ProductionLineHeight);
    }


    // Prüft, ob eine Linie die Delivery Line ist.
    private static bool IstDeliveryLine(ProductionLine line)
    {
        return
            line.Pose.X == ViewParameters.RectDeliveryPose.X &&
            line.Pose.Y == ViewParameters.RectDeliveryPose.Y;
    }


    // Prüft, ob der Greifpunkt in der Nähe einer erlaubten Docklinie liegt.
    private bool AufDockLinie()
    {
        Point tcp = Greifpunkt(Location, Angle);

        foreach (ProductionLine line in dockLines)
        {
            Size liniengroesse = ErmittleLiniengroesse(line);

            if (AufLinie(
                tcp,
                line.Pose,
                liniengroesse.Width,
                liniengroesse.Height,
                LinienToleranz))
            {
                return true;
            }
        }

        return false;
    }


    // Prüft, ob der Greifpunkt auf der Delivery Line liegt.
    public bool AufDeliveryLinie()
    {
        return AufLinie(
            Greifpunkt(Location, Angle),
            ViewParameters.RectDeliveryPose,
            ViewParameters.ProductionDeliveryWidth,
            ViewParameters.ProductionDeliveryHeight,
            0);
    }


    // --------------------------------------------------------
    // Bewegungsgrenzen
    // --------------------------------------------------------

    // Prüft, ob alle Ecken eines Rechtecks innerhalb
    // der Fabrikgrenzen liegen.
    private bool EckenInnerhalbGrenzen(PointF[] ecken)
    {
        foreach (PointF ecke in ecken)
        {
            if (!borders.Contains(
                (int)Math.Floor(ecke.X),
                (int)Math.Floor(ecke.Y)))
            {
                return false;
            }
        }

        return true;
    }


    // Prüft, ob ein getragener LEGO-Stein innerhalb
    // der Fabrikgrenzen bleibt.
    private bool DockobjektInnerhalbGrenzen(
        Point robotPosition,
        int robotAngle)
    {
        if (dockobjekt is not LegoBrick brick)
            return true;

        Size steingroesse = ErmittleSteingroesse(brick);

        Point tcp = Greifpunkt(robotPosition, robotAngle);

        Point steinPosition = new(
            tcp.X - steingroesse.Width / 2,
            tcp.Y - steingroesse.Height / 2);

        // Eckpunkte des gedrehten Steins berechnen.
        PointF[] steinEcken = RotatedCorners(
            steinPosition,
            robotAngle + dockWinkelOffset,
            steingroesse.Width,
            steingroesse.Height);

        return EckenInnerhalbGrenzen(steinEcken);
    }


    // --------------------------------------------------------
    // Undocken
    // --------------------------------------------------------

    // Löst die Verbindung zwischen Roboter und LEGO-Stein.
    // Gibt den Stein zurück oder null, wenn keiner angedockt ist.
    public LegoBrick? Undock()
    {
        if (dockobjekt is not LegoBrick brick)
            return null;

        brick.Status = BrickStatus.Moving;

        dockobjekt = null;
        dockWinkelOffset = 0;

        return brick;
    }


    // --------------------------------------------------------
    // Pose setzen
    // --------------------------------------------------------

    // Setzt Position und Ausrichtung gleichzeitig.
    //
    // Die einzelnen Koordinaten werden dabei synchronisiert.
    // Ein angedockter Stein wird anschließend nachgezogen.
    public void SetzePose(Pose newPose)
    {
        pose = newPose;

        x = newPose.X;
        y = newPose.Y;
        angle = newPose.Angle;

        DockobjektNachziehen();
    }


    // --------------------------------------------------------
    // Roboter-Kollisionen
    // --------------------------------------------------------

    // Prüft, ob eine geplante Roboterposition erlaubt ist.
    private bool IstRoboterPoseGueltig(Pose geplantePose)
    {
        PointF[] roboterEcken = RotatedCorners(
            geplantePose.Location,
            geplantePose.Angle,
            RobotWidth,
            RobotHeight);

        // Der gesamte Roboter muss innerhalb der Fabrik liegen.
        if (!EckenInnerhalbGrenzen(roboterEcken))
            return false;

        // Der Roboter darf keine Produktionslinie berühren.
        if (KollidiertMitProduktionslinie(geplantePose))
            return false;

        // Auch ein getragener Stein muss innerhalb der Grenzen bleiben.
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
        PointF[] roboterEcken = RotatedCorners(
            robotPose.Location,
            robotPose.Angle,
            RobotWidth,
            RobotHeight);

        foreach (ProductionLine line in productionLines)
        {
            Size liniengroesse = ErmittleLiniengroesse(line);

            PointF[] linienEcken = RotatedCorners(
                line.Pose.Location,
                line.Pose.Angle,
                liniengroesse.Width,
                liniengroesse.Height);

            if (RechteckeUeberlappen(roboterEcken, linienEcken))
                return true;
        }

        return false;
    }


    // --------------------------------------------------------
    // Geometrische Hilfsmethoden
    // --------------------------------------------------------

    // Berechnet die vier Eckpunkte eines gedrehten Rechtecks.
    // Die Drehung erfolgt um den Mittelpunkt des Rechtecks.
    private static PointF[] RotatedCorners(
        Point topLeft,
        double angle,
        double width,
        double height)
    {
        double winkel = angle * Math.PI / 180.0;

        double mitteX = topLeft.X + width / 2.0;
        double mitteY = topLeft.Y + height / 2.0;

        double halbeBreite = width / 2.0;
        double halbeHoehe = height / 2.0;

        double cos = Math.Cos(winkel);
        double sin = Math.Sin(winkel);

        PointF DrehePunkt(double x, double y)
        {
            return new PointF(
                (float)(mitteX + x * cos - y * sin),
                (float)(mitteY + x * sin + y * cos));
        }

        return new[]
        {
            DrehePunkt(-halbeBreite, -halbeHoehe),
            DrehePunkt(halbeBreite, -halbeHoehe),
            DrehePunkt(halbeBreite, halbeHoehe),
            DrehePunkt(-halbeBreite, halbeHoehe)
        };
    }


    // Prüft, ob sich zwei gedrehte Rechtecke überschneiden.
    //
    // Verwendet wird das Separating Axis Theorem (SAT).
    private static bool RechteckeUeberlappen(
        PointF[] rechteckA,
        PointF[] rechteckB)
    {
        return
            !GibtEsTrennachse(rechteckA, rechteckB) &&
            !GibtEsTrennachse(rechteckB, rechteckA);
    }


    // Prüft, ob zwischen zwei Rechtecken eine Trennachse existiert.
    private static bool GibtEsTrennachse(
        PointF[] polygon,
        PointF[] anderesPolygon)
    {
        for (int i = 0; i < polygon.Length; i++)
        {
            PointF ersterPunkt = polygon[i];
            PointF zweiterPunkt = polygon[(i + 1) % polygon.Length];

            // Senkrechte zur aktuellen Rechteckkante berechnen.
            double achseX = -(zweiterPunkt.Y - ersterPunkt.Y);
            double achseY = zweiterPunkt.X - ersterPunkt.X;

            double minA = double.PositiveInfinity;
            double maxA = double.NegativeInfinity;

            double minB = double.PositiveInfinity;
            double maxB = double.NegativeInfinity;

            // Erstes Rechteck auf die Achse projizieren.
            foreach (PointF punkt in polygon)
            {
                double projektion =
                    punkt.X * achseX + punkt.Y * achseY;

                minA = Math.Min(minA, projektion);
                maxA = Math.Max(maxA, projektion);
            }

            // Zweites Rechteck auf dieselbe Achse projizieren.
            foreach (PointF punkt in anderesPolygon)
            {
                double projektion =
                    punkt.X * achseX + punkt.Y * achseY;

                minB = Math.Min(minB, projektion);
                maxB = Math.Max(maxB, projektion);
            }

            // Getrennte Projektionsbereiche bedeuten,
            // dass sich die Rechtecke nicht überschneiden.
            if (maxA <= minB || maxB <= minA)
                return true;
        }

        return false;
    }
}