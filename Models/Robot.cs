using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyAvaloniaApp.Models;

// Der Roboter ist ein Beweger mit eigener Bewegungsrichtung.
public class Robot : Beweger
{
    private Parameters.Direction robotDirection =
        Parameters.Direction.stop;

    private const int AbstandZurLinie = 30;

    public Parameters.Direction RobotDirection
    {
        get => robotDirection;
        private set => robotDirection = value;
    }

    public Robot(
        Point location,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines,
        IReadOnlyList<ProductionLine> dockLines)
        : base(location, borders, productionLines, dockLines)
    {
    }

    public Robot(
        Pose pose,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines,
        IReadOnlyList<ProductionLine> dockLines)
        : base(pose, borders, productionLines, dockLines)
    {
    }

    // Speichert die Richtung und führt die Bewegung aus.
    public override void Bewegen(Parameters.Direction direction)
    {
        RobotDirection = direction;

        base.Bewegen(direction);
    }

    // Positioniert den Roboter vor einer Linie.
    public void GeheZuLinie(
        Pose linienPose,
        double linienBreite,
        double linienHoehe,
        double zielwinkel)
    {
        double angleRad = zielwinkel * Math.PI / 180.0;

        double forwardX = Math.Sin(angleRad);
        double forwardY = -Math.Cos(angleRad);

        double centerX =
            linienPose.X + linienBreite / 2.0;

        double centerY =
            linienPose.Y + linienHoehe / 2.0;

        double lineAngleRad =
            linienPose.Angle * Math.PI / 180.0;

        double axisXX = Math.Cos(lineAngleRad);
        double axisXY = Math.Sin(lineAngleRad);

        double axisYX = -Math.Sin(lineAngleRad);
        double axisYY = Math.Cos(lineAngleRad);

        double halfExtent =
            Math.Abs(forwardX * axisXX + forwardY * axisXY)
                * linienBreite / 2.0
            +
            Math.Abs(forwardX * axisYX + forwardY * axisYY)
                * linienHoehe / 2.0;

        double distance = halfExtent + AbstandZurLinie;

        double tcpX = centerX - forwardX * distance;
        double tcpY = centerY - forwardY * distance;

        double robotX =
            tcpX
            - ViewParameters.RobotWidth / 2.0
            - ViewParameters.RobotHeight / 2.0 * forwardX;

        double robotY =
            tcpY
            - ViewParameters.RobotHeight / 2.0
            - ViewParameters.RobotHeight / 2.0 * forwardY;

        SetzePose(new Pose(
            (int)Math.Round(robotX),
            (int)Math.Round(robotY),
            (int)Math.Round(zielwinkel)));

        RobotDirection = Parameters.Direction.stop;
    }
}