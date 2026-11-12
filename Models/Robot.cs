using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyAvaloniaApp.Models;


// Der Roboter erbt die Bewegungs- und Kollisionsfunktionen
// von Beweger und ergänzt die automatische Positionierung.
public class Robot : Beweger
{
    // Abstand zwischen dem Greifpunkt des Roboters
    // und dem Mittelpunkt der angefahrenen Linie.
    private const int abstandZurLinie = 30;


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public Robot(
        Pose pose,
        Rectangle borders,
        IReadOnlyList<ProductionLine> lines)
        : base(pose, borders, lines, lines)
    {
    }


    // --------------------------------------------------------
    // Automatische Positionierung
    // --------------------------------------------------------

    // Positioniert den Roboter vor einer Produktionslinie.
    //
    // Die eigentliche Aufnahme eines LEGO-Steins erfolgt
    // anschließend durch die manuelle Bewegung des Roboters.
    public void GeheZuLinie(
        Pose linienPose,
        double linienBreite,
        double linienHoehe,
        double zielwinkel)
    {
        // Zielwinkel von Grad in Radiant umrechnen.
        double winkel = zielwinkel * Math.PI / 180.0;

        // Bewegungsrichtung des Greifpunkts berechnen.
        double richtungX = Math.Sin(winkel);
        double richtungY = -Math.Cos(winkel);


        // Mittelpunkt der Produktionslinie berechnen.
        double linienMitteX =
            linienPose.X + linienBreite / 2.0;

        double linienMitteY =
            linienPose.Y + linienHoehe / 2.0;


        // Ausrichtung der Produktionslinie berücksichtigen.
        double linienWinkel =
            linienPose.Angle * Math.PI / 180.0;

        // Richtung entlang der Produktionslinie.
        double achseX = Math.Cos(linienWinkel);
        double achseY = Math.Sin(linienWinkel);

        // Richtung senkrecht zur Produktionslinie.
        double normalX = -Math.Sin(linienWinkel);
        double normalY = Math.Cos(linienWinkel);


        // Ausdehnung der Linie in Anfahrtrichtung berechnen.
        double halbeAusdehnung =
            Math.Abs(richtungX * achseX + richtungY * achseY)
                * linienBreite / 2.0
            +
            Math.Abs(richtungX * normalX + richtungY * normalY)
                * linienHoehe / 2.0;


        // Greifpunkt mit zusätzlichem Abstand vor der Linie platzieren.
        double abstand = halbeAusdehnung + abstandZurLinie;

        double greifpunktX =
            linienMitteX - richtungX * abstand;

        double greifpunktY =
            linienMitteY - richtungY * abstand;


        // Position des Roboters anhand seines Greifpunkts berechnen.
        double robotX =
            greifpunktX
            - ViewParameters.RobotWidth / 2.0
            - ViewParameters.RobotHeight / 2.0 * richtungX;

        double robotY =
            greifpunktY
            - ViewParameters.RobotHeight / 2.0
            - ViewParameters.RobotHeight / 2.0 * richtungY;


        // Neue Position und Ausrichtung übernehmen.
        Pose neuePose = new(
            (int)Math.Round(robotX),
            (int)Math.Round(robotY),
            (int)Math.Round(zielwinkel));

        SetzePose(neuePose);
    }
}