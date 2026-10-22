using System;
using System.Collections.Generic;
using System.Drawing;
using static MyAvaloniaApp.Models.ViewParameters;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

public class ProductionLine
{
    public Pose ProductionLinePose { get; }
    public Point Location => ProductionLinePose.Location;
    public int Angle => ProductionLinePose.Angle;
    private List<LegoBrick> bricks = new List<LegoBrick>();
    public List<LegoBrick> Bricks => bricks;
    private int tickCount = 0;
    private int stepsPerDirection = 10;

    public ProductionLine(Pose pose)
    {
        ProductionLinePose = pose;
    }

    public ProductionLine(Pose pose, Parameters.LegoColor color)
    {
        ProductionLinePose = pose;
        CreateBricks(color);
    }

    public ProductionLine(Pose pose, IReadOnlyList<Parameters.LegoColor> colors, bool placeBricksAboveLine)
    {
        ProductionLinePose = pose;
        CreateBricks(colors, placeBricksAboveLine);
    }

    private void CreateBricks(Parameters.LegoColor color)
    {
        Parameters.LegoColor[] colors =
            new Parameters.LegoColor[Parameters.bricksPerLine];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }

        CreateBricks(colors, false);
    }

    private void CreateBricks(IReadOnlyList<Parameters.LegoColor> colors,bool placeBricksAboveLine)
    {
        if (colors.Count != Parameters.bricksPerLine)
            throw new ArgumentException(
                $"Expected {Parameters.bricksPerLine} brick colors.");

        double productionLineWidth = ViewParameters.ProductionLineWidth;
        double productionLineHeight = ViewParameters.ProductionLineHeight;

        const double insideOffset = 25;
        const double brickSpacing = 24;

        double angleRad = ProductionLinePose.Angle * Math.PI / 180.0;

        double dx = Math.Cos(angleRad);
        double dy = Math.Sin(angleRad);

        double lineCenterX =
            ProductionLinePose.X + productionLineWidth / 2.0;

        double lineCenterY =
            ProductionLinePose.Y + productionLineHeight / 2.0;

        double nx = -dy;
        double ny = dx;

        if (placeBricksAboveLine)
        {
            // Die Steine sollen auf der oberen Seite der Linie liegen.
            if (ny > 0)
            {
                nx = -nx;
                ny = -ny;
            }
        }
        else
        {
            // Bei den bisherigen Produktionslinien liegen die Steine
            // auf der zur Fabrikmitte gerichteten Seite.
            double factoryCenterX =
                ViewParameters.Borders.Left + ViewParameters.Borders.Width / 2.0;

            double factoryCenterY =
                ViewParameters.Borders.Top + ViewParameters.Borders.Height / 2.0;

            double toCenterX = factoryCenterX - lineCenterX;
            double toCenterY = factoryCenterY - lineCenterY;

            if (nx * toCenterX + ny * toCenterY < 0)
            {
                nx = -nx;
                ny = -ny;
            }
        }

        double rowCenterX = lineCenterX + insideOffset * nx;
        double rowCenterY = lineCenterY + insideOffset * ny;

        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            double distance =
                (i - (Parameters.bricksPerLine - 1) / 2.0) * brickSpacing;

            double brickCenterX = rowCenterX + distance * dx;
            double brickCenterY = rowCenterY + distance * dy;

            // Aktuell erstellen wir ausschließlich kleine Steine.
            LegoSize brickSize = LegoSize.small;

            double displayWidth = ViewParameters.SmallBrickWidth;
            double displayHeight = ViewParameters.SmallBrickHeight;

            double x = brickCenterX - displayWidth / 2.0;
            double y = brickCenterY - displayHeight / 2.0;

            Pose brickPose = new Pose(
                (int)Math.Round(x),
                (int)Math.Round(y),
                ProductionLinePose.Angle);

            bricks.Add(new LegoBrick(brickPose, colors[i], brickSize));
        }
    }

    public void Bewegen(int directionSign)
    {
        bool forward = tickCount < stepsPerDirection;

        tickCount =
            (tickCount + 1) % (stepsPerDirection * 2);

        double angleRad =
            ProductionLinePose.Angle * Math.PI / 180.0;

        double deltaX = Math.Cos(angleRad) * directionSign;
        double deltaY = Math.Sin(angleRad) * directionSign;

        double timeScale = Parameters.timerInterval / 50.0;
        double distance = Parameters.step * Math.Sqrt(2) * timeScale;

        deltaX *= distance;
        deltaY *= distance;

        if (!forward)
        {
            deltaX = -deltaX;
            deltaY = -deltaY;
        }

        foreach (LegoBrick brick in Bricks)
        {
            brick.Bewegen(deltaX, deltaY);
        }
    }
}