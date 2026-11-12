using System;
using System.Collections.Generic;
using System.Drawing;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

// Verwaltet die Steine und ihre Bewegung auf einer Produktionslinie.
public class ProductionLine
{
    private const int StepsPerDirection = 10;
    private const int BrickSpacing = 24;
    private const int InsideOffset = 25;

    private readonly List<LegoBrick> bricks = new();

    private readonly LegoColor[] alleFarben =
    {
        LegoColor.red,
        LegoColor.green,
        LegoColor.blue,
        LegoColor.yellow
    };

    private readonly Random random = new();

    private int tickCount;
    private bool initialOffsetApplied;

    public Pose ProductionLinePose { get; }

    public Pose Pose => ProductionLinePose;

    public Point Location => ProductionLinePose.Location;

    public int Angle => ProductionLinePose.Angle;

    public List<LegoBrick> Bricks => bricks;

    public ProductionLine(Pose pose)
    {
        ProductionLinePose = pose;
    }

    // Erzeugt vier Steine mit unabhängig zufällig gewählter Farbe.
    public void CreateBricks()
    {
        if (bricks.Count > 0)
            return;

        List<LegoColor> colors = new();

        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            int colorIndex = random.Next(alleFarben.Length);

            colors.Add(alleFarben[colorIndex]);
        }

        CreateBricks(colors);
    }

    // Erstellt die Steine an ihren vorgesehenen Positionen.
    private void CreateBricks(IReadOnlyList<LegoColor> colors)
    {
        if (colors.Count != Parameters.bricksPerLine)
        {
            throw new ArgumentException(
                $"Expected {Parameters.bricksPerLine} brick colors.");
        }

        double lineAngle =
            ProductionLinePose.Angle * Math.PI / 180.0;

        double directionX = Math.Cos(lineAngle);
        double directionY = Math.Sin(lineAngle);

        double normalX = -directionY;
        double normalY = directionX;

        double lineCenterX =
            ProductionLinePose.X +
            ViewParameters.ProductionLineWidth / 2.0;

        double lineCenterY =
            ProductionLinePose.Y +
            ViewParameters.ProductionLineHeight / 2.0;

        double factoryCenterX =
            ViewParameters.Borders.Left +
            ViewParameters.Borders.Width / 2.0;

        double factoryCenterY =
            ViewParameters.Borders.Top +
            ViewParameters.Borders.Height / 2.0;

        double toFactoryX = factoryCenterX - lineCenterX;
        double toFactoryY = factoryCenterY - lineCenterY;

        if (normalX * toFactoryX + normalY * toFactoryY < 0)
        {
            normalX = -normalX;
            normalY = -normalY;
        }

        double rowCenterX =
            lineCenterX + InsideOffset * normalX;

        double rowCenterY =
            lineCenterY + InsideOffset * normalY;

        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            double distance =
                (i - (Parameters.bricksPerLine - 1) / 2.0)
                * BrickSpacing;

            double brickCenterX =
                rowCenterX + distance * directionX;

            double brickCenterY =
                rowCenterY + distance * directionY;

            int x = (int)Math.Round(
                brickCenterX - ViewParameters.SmallBrickWidth / 2.0);

            int y = (int)Math.Round(
                brickCenterY - ViewParameters.SmallBrickHeight / 2.0);

            Pose brickPose = new(
                x,
                y,
                ProductionLinePose.Angle);

            bricks.Add(new LegoBrick(
                brickPose,
                colors[i],
                LegoSize.small));
        }
    }

    // Bewegt die Steine gleichmäßig vor und zurück.
    public void Bewegen(int directionSign)
    {
        if (bricks.Count == 0)
            return;

        double angleRad =
            ProductionLinePose.Angle * Math.PI / 180.0;

        double timeScale =
            Parameters.timerInterval / 50.0;

        double distance =
            Parameters.step * Math.Sqrt(2) * timeScale;

        double deltaX =
            Math.Cos(angleRad) * directionSign * distance;

        double deltaY =
            Math.Sin(angleRad) * directionSign * distance;

        // Anfangsposition der Steine leicht versetzen.
        if (!initialOffsetApplied)
        {
            int initialSteps = StepsPerDirection / 2;

            for (int i = 0; i < initialSteps; i++)
            {
                foreach (LegoBrick brick in bricks)
                {
                    brick.Bewegen(-deltaX, -deltaY);
                }
            }

            initialOffsetApplied = true;
            return;
        }

        bool forward = tickCount < StepsPerDirection;

        tickCount =
            (tickCount + 1) % (StepsPerDirection * 2);

        if (!forward)
        {
            deltaX = -deltaX;
            deltaY = -deltaY;
        }

        foreach (LegoBrick brick in bricks)
        {
            brick.Bewegen(deltaX, deltaY);
        }
    }
}