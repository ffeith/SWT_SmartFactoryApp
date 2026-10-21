using System;
using System.Collections.Generic;
using System.Drawing;
using static MyAvaloniaApp.Models.ViewParameters;

namespace MyAvaloniaApp.Models;

public class Robot : Beweger
{
    private readonly IReadOnlyList<ProductionLine> productionLines;

    public Robot(
        Point location,
        Rectangle borders,
        IReadOnlyList<ProductionLine> productionLines)
        : base(location, borders)
    {
        this.productionLines = productionLines;
    }

    public override void Bewegen(Parameters.Direction direction)
    {
        base.Bewegen(direction);
    }

    protected override bool PositionErlaubt(
        Point candidateLocation,
        double candidateRotation)
    {
        // Zuerst prüfen, ob der Roboter innerhalb
        // der Fabrikgrenzen bleibt.
        if (!base.PositionErlaubt(
                candidateLocation,
                candidateRotation))
        {
            return false;
        }

        // Begrenzung des Roboters in der geplanten Pose.
        Rectangle robotBounds = RotatedBounds(
            candidateLocation,
            candidateRotation,
            RobotWidth,
            RobotHeight);

        // Alle sechs Produktionslinien überprüfen.
        foreach (ProductionLine line in productionLines)
        {
            Point lineLocation = new Point(line.ProductionLinePose.X, line.ProductionLinePose.Y);

            Rectangle lineBounds = RotatedBounds(
                lineLocation,
                line.ProductionLinePose.Angle,
                ProductionLineWidth,
                ProductionLineHeight);

            if (robotBounds.IntersectsWith(lineBounds))
                return false;
        }

        return true;
    }
}