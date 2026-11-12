using System;
using System.Collections.Generic;
using System.Drawing;
using static MyAvaloniaApp.Models.ViewParameters;

namespace MyAvaloniaApp.Models;

public class Robot : Beweger
{
    private Parameters.Direction robotDirection = Parameters.Direction.stop;

    // Abstand zwischen dem Greifpunkt und dem Brick beim Positionieren.
    private readonly int abstandZurLinie = 30;

    public Parameters.Direction RobotDirection
    {
        get => robotDirection;
        private set => robotDirection = value;
    }

    public Robot(
        Point location,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines)
        : base(location, borders, productionLines)
    {
    }

    public override void Bewegen(Parameters.Direction direction)
    {
        RobotDirection = direction;
        base.Bewegen(direction);
    }

    // Positioniert den Roboter vor einem Brick.
    // Positioniert den Roboter vor einer Produktionslinie.
    // Positioniert den Roboter vor einer Produktionslinie.
    public void GeheZuLinie(
        Pose linienPose,
        double linienBreite,
        double linienHoehe,
        double zielwinkel)
    {
        double angleRad = zielwinkel * Math.PI / 180.0;

        // Bewegungsrichtung des Roboters berechnen.
        double forwardX = Math.Sin(angleRad);
        double forwardY = -Math.Cos(angleRad);

        // Mittelpunkt der Linie berechnen.
        double centerX = linienPose.X + linienBreite / 2.0;
        double centerY = linienPose.Y + linienHoehe / 2.0;

        // Orientierung der Produktionslinie berücksichtigen.
        double lineAngleRad = linienPose.Angle * Math.PI / 180.0;

        double axisXX = Math.Cos(lineAngleRad);
        double axisXY = Math.Sin(lineAngleRad);

        double axisYX = -Math.Sin(lineAngleRad);
        double axisYY = Math.Cos(lineAngleRad);

        // Ausdehnung der Linie in Anfahrtrichtung berechnen.
        double halfExtent =
            Math.Abs(forwardX * axisXX + forwardY * axisXY)
                * linienBreite / 2.0
            +
            Math.Abs(forwardX * axisYX + forwardY * axisYY)
                * linienHoehe / 2.0;

        // Abstand vom Mittelpunkt bis zur Außenkante
        // plus zusätzlicher Sicherheitsabstand.
        double distance = halfExtent + abstandZurLinie;

        // Greifpunkt vor der Linie positionieren.
        double tcpX = centerX - forwardX * distance;
        double tcpY = centerY - forwardY * distance;

        // Linke obere Ecke des Roboters berechnen.
        double robotX =
            tcpX - RobotWidth / 2.0 - RobotHeight / 2.0 * forwardX;

        double robotY =
            tcpY - RobotHeight / 2.0 - RobotHeight / 2.0 * forwardY;

        Pose zielPose = new Pose(
            (int)Math.Round(robotX),
            (int)Math.Round(robotY),
            (int)Math.Round(zielwinkel));

        SetzePose(zielPose);

        RobotDirection = Parameters.Direction.stop;
    }
}