using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyAvaloniaApp.Models;


// Verwaltet die LEGO-Steine einer Produktionslinie
// und übernimmt die Bewegung des Förderbandes.
public class ProductionLine
{
    // --------------------------------------------------------
    // Felder
    // --------------------------------------------------------

    // Position und Ausrichtung der Produktionslinie.
    private readonly int x;
    private readonly int y;
    private readonly int angle;

    // Enthält alle Steine dieser Produktionslinie.
    private readonly List<LegoBrick> bricks = new();

    // Alle möglichen Farben der LEGO-Steine.
    private readonly LegoColor[] alleFarben =
    {
        LegoColor.red,
        LegoColor.green,
        LegoColor.blue,
        LegoColor.yellow
    };

    // Zufallsgenerator für die Farben der Steine.
    private readonly Random random = new();

    // Anzahl der Bewegungsschritte in eine Richtung.
    private const int stepsPerDirection = 10;

    // Abstand zwischen zwei LEGO-Steinen.
    private const int brickSpacing = 24;

    // Abstand der Steinreihe vom Mittelpunkt der Produktionslinie.
    private const int insideOffset = 25;

    // Referenzwert für die Berechnung der Bewegungsgeschwindigkeit.
    private const int referenceTimerInterval = 50;

    // Zählt die bisherigen Bewegungsschritte.
    private int tickCount;

    // Verhindert, dass die anfängliche Verschiebung mehrfach erfolgt.
    private bool initialOffsetApplied;


    // --------------------------------------------------------
    // Eigenschaften
    // --------------------------------------------------------

    // Drehwinkel der Produktionslinie.
    public int Angle => angle;

    // Steine dieser Produktionslinie.
    public IReadOnlyList<LegoBrick> Bricks => bricks;

    // Position ohne Drehwinkel.
    public Point Location => new(x, y);

    // Vollständige Position einschließlich Drehwinkel.
    public Pose Pose => new(x, y, angle);


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public ProductionLine(Pose pose)
    {
        x = pose.X;
        y = pose.Y;
        angle = pose.Angle;
    }


    // --------------------------------------------------------
    // LEGO-Steine erzeugen
    // --------------------------------------------------------

    // Erzeugt vier LEGO-Steine mit zufälligen Farben.
    public void CreateBricks()
    {
        // Eine bereits befüllte Produktionslinie
        // wird nicht erneut befüllt.
        if (bricks.Count > 0)
            return;

        double winkel = angle * Math.PI / 180.0;

        // Richtung entlang der Produktionslinie.
        double richtungX = Math.Cos(winkel);
        double richtungY = Math.Sin(winkel);

        // Senkrechte Richtung zur Produktionslinie.
        double normalX = -richtungY;
        double normalY = richtungX;


        // Mittelpunkt der Produktionslinie.
        double linienMitteX =
            x + ViewParameters.ProductionLineWidth / 2.0;

        double linienMitteY =
            y + ViewParameters.ProductionLineHeight / 2.0;


        // Mittelpunkt der Fabrik.
        double fabrikMitteX =
            ViewParameters.Borders.Left +
            ViewParameters.Borders.Width / 2.0;

        double fabrikMitteY =
            ViewParameters.Borders.Top +
            ViewParameters.Borders.Height / 2.0;


        // Bestimmen, auf welcher Seite der Produktionslinie
        // sich die Mitte der Fabrik befindet.
        double abstandX = fabrikMitteX - linienMitteX;
        double abstandY = fabrikMitteY - linienMitteY;

        if (normalX * abstandX + normalY * abstandY < 0)
        {
            normalX = -normalX;
            normalY = -normalY;
        }


        // Mittelpunkt der Reihe berechnen,
        // in der die LEGO-Steine angeordnet werden.
        double reihenMitteX =
            linienMitteX + insideOffset * normalX;

        double reihenMitteY =
            linienMitteY + insideOffset * normalY;


        // Vier Steine mit gleichmäßigem Abstand erzeugen.
        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            // Abstand des aktuellen Steins von der Reihenmitte.
            double abstand =
                (i - (Parameters.bricksPerLine - 1) / 2.0)
                * brickSpacing;

            // Mittelpunkt des aktuellen Steins.
            double steinMitteX =
                reihenMitteX + abstand * richtungX;

            double steinMitteY =
                reihenMitteY + abstand * richtungY;


            // Position der linken oberen Ecke berechnen.
            int brickX = (int)Math.Round(
                steinMitteX - ViewParameters.SmallBrickWidth / 2.0);

            int brickY = (int)Math.Round(
                steinMitteY - ViewParameters.SmallBrickHeight / 2.0);


            // Der Stein übernimmt die Ausrichtung seiner Produktionslinie.
            Pose brickPose = new(
                brickX,
                brickY,
                angle);


            // Für jeden Stein unabhängig eine zufällige Farbe auswählen.
            LegoColor farbe =
                alleFarben[random.Next(alleFarben.Length)];


            // LEGO-Stein erstellen und der Produktionslinie hinzufügen.
            bricks.Add(new LegoBrick(
                brickPose,
                farbe,
                LegoSize.small));
        }
    }


    // --------------------------------------------------------
    // Bewegung des Förderbandes
    // --------------------------------------------------------

    // Bewegt die Steine regelmäßig vor und zurück.
    //
    // directionSign bestimmt die ursprüngliche Bewegungsrichtung:
    //  1 = positive Richtung
    // -1 = negative Richtung
    public void Bewegen(int directionSign)
    {
        // Ohne Steine gibt es nichts zu bewegen.
        if (bricks.Count == 0)
            return;

        double winkel = angle * Math.PI / 180.0;

        // Bewegungsgeschwindigkeit abhängig vom Timer berechnen.
        double timeScale =
            (double)Parameters.timerInterval / referenceTimerInterval;

        double distance =
            Parameters.step * Math.Sqrt(2) * timeScale;


        // Bewegungsstrecke in X- und Y-Richtung.
        double deltaX =
            Math.Cos(winkel) * directionSign * distance;

        double deltaY =
            Math.Sin(winkel) * directionSign * distance;


        // Beim ersten Aufruf werden die Steine zunächst
        // entgegen der normalen Bewegungsrichtung verschoben.
        if (!initialOffsetApplied)
        {
            int initialSteps = stepsPerDirection / 2;

            for (int i = 0; i < initialSteps; i++)
            {
                BewegeAlleSteine(-deltaX, -deltaY);
            }

            initialOffsetApplied = true;
            return;
        }


        // Bestimmen, ob sich die Steine vorwärts oder rückwärts bewegen.
        bool vorwaerts = tickCount < stepsPerDirection;

        // Bewegungsschritt erhöhen und nach einem vollständigen
        // Bewegungszyklus wieder bei null beginnen.
        tickCount =
            (tickCount + 1) % (stepsPerDirection * 2);


        // Nach zehn Schritten wird die Bewegungsrichtung umgekehrt.
        if (!vorwaerts)
        {
            deltaX = -deltaX;
            deltaY = -deltaY;
        }

        BewegeAlleSteine(deltaX, deltaY);
    }


    // Bewegt alle Steine dieser Produktionslinie um dieselbe Strecke.
    private void BewegeAlleSteine(double deltaX, double deltaY)
    {
        foreach (LegoBrick brick in bricks)
        {
            brick.Bewegen(deltaX, deltaY);
        }
    }
}