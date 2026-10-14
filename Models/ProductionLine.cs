using System;
using System.Collections.Generic;
using static MyAvaloniaApp.Models.ViewParameters;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

public class ProductionLine
{
    public Pose ProductionLinePose { get; }

    private List<LegoBrick> bricks = new List<LegoBrick>();

    public List<LegoBrick> Bricks => bricks;

    public ProductionLine(Pose pose)
    {
        ProductionLinePose = pose;
        // Dieser Konstruktor erzeugt keine Steine.
    }

    public ProductionLine(Pose pose, Parameters.LegoColor color)
    {
        ProductionLinePose = pose;
        CreateBricks(color);
    }

    private void CreateBricks(Parameters.LegoColor color)
    {
        const double productionLineWidth = 100;
        const double productionLineHeight = 25;

        const double insideOffset = 25;
        const double brickSpacing = 24;

        double angleRad = ProductionLinePose.Angle * Math.PI / 180.0;

        double dx = Math.Cos(angleRad);                                     // Richtung entlang der Produktionslinie
        double dy = Math.Sin(angleRad);

        double lineCenterX = ProductionLinePose.X + productionLineWidth / 2.0;
        double lineCenterY = ProductionLinePose.Y + productionLineHeight / 2.0;

        double factoryCenterX = ViewParameters.Borders.Left + ViewParameters.Borders.Width / 2.0;
        double factoryCenterY = ViewParameters.Borders.Top + ViewParameters.Borders.Height / 2.0;

        double nx = -dy;                                            // Senkrechte Richtung zur Produktionslinie
        double ny = dx;

        double toCenterX = factoryCenterX - lineCenterX;
        double toCenterY = factoryCenterY - lineCenterY;

        if (nx * toCenterX + ny * toCenterY < 0)                    // Innenseite der Produktionslinie bestimmen
        {
            nx = -nx;
            ny = -ny;
        }

        double rowCenterX = lineCenterX + insideOffset * nx;
        double rowCenterY = lineCenterY + insideOffset * ny;

        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            double distance =
                (i - (Parameters.bricksPerLine - 1) / 2.0) * brickSpacing;

            double brickCenterX = rowCenterX + distance * dx;
            double brickCenterY = rowCenterY + distance * dy;

            LegoSize brickSize = i < 2                                              //Erste zwei Steine gross, letzte zwei klein
                ? LegoSize.large
                : LegoSize.small;

            double displayWidth = brickSize == LegoSize.large ? 20 : 12;
            const double displayHeight = 12;

            double x = brickCenterX - displayWidth / 2.0;
            double y = brickCenterY - displayHeight / 2.0;

            Pose brickPose = new Pose(
                (int)Math.Round(x),
                (int)Math.Round(y),
                ProductionLinePose.Angle);

            bricks.Add(new LegoBrick(brickPose, color, brickSize));
        }
    }
}