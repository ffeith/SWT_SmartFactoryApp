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

    public ProductionLine(Pose pose, Parameters.LegoColor color, Parameters.LegoSize size)
    {
        ProductionLinePose = pose;
        CreateBricks(color, size);
    }

    private void CreateBricks(Parameters.LegoColor color, Parameters.LegoSize size)
        {
        // Groesse der Produktionslinien-Rechtecke aus der AXAML-Datei
        const double productionLineWidth = 100;
        const double productionLineHeight = 25;

        // Abstand der Steine zur Produktionslinie und zueinander
        const double insideOffset = 25;
        const double brickSpacing = 24;

        // Winkel der Produktionslinie in Bogenmass umrechnen
        double angleRad = ProductionLinePose.Angle * Math.PI / 180.0;

        // Richtung entlang der Produktionslinie
        double dx = Math.Cos(angleRad);
        double dy = Math.Sin(angleRad);

        // Mittelpunkt der Produktionslinie berechnen
        double lineCenterX = ProductionLinePose.X + productionLineWidth / 2.0;
        double lineCenterY = ProductionLinePose.Y + productionLineHeight / 2.0;

        // Mittelpunkt des vorhandenen Fabrikbereichs
        double factoryCenterX = ViewParameters.Borders.Left
                            + ViewParameters.Borders.Width / 2.0;

        double factoryCenterY = ViewParameters.Borders.Top
                            + ViewParameters.Borders.Height / 2.0;

        // Senkrechte Richtung zur Produktionslinie
        double nx = -dy;
        double ny = dx;

        // Prüfen, welche Seite zur Mitte der Fabrik zeigt
        double toCenterX = factoryCenterX - lineCenterX;
        double toCenterY = factoryCenterY - lineCenterY;

        if (nx * toCenterX + ny * toCenterY < 0)
        {
            nx = -nx;
            ny = -ny;
        }

        // Mittelpunkt der Steinreihe auf die Innenseite verschieben
        double rowCenterX = lineCenterX + insideOffset * nx;
        double rowCenterY = lineCenterY + insideOffset * ny;

        // Die uebergebene Groesse gilt fuer die ersten beiden Steine.
        // Die beiden anderen erhalten die jeweils andere Groesse.
        LegoSize otherSize = size == LegoSize.small
            ? LegoSize.large
            : LegoSize.small;

        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            // Vier gleichmaessig verteilte Positionen um die Reihenmitte
            double distance = (i - (Parameters.bricksPerLine - 1) / 2.0)
                            * brickSpacing;

            double brickCenterX = rowCenterX + distance * dx;
            double brickCenterY = rowCenterY + distance * dy;

            LegoSize brickSize = i < 2 ? size : otherSize;

            // Darstellungsmasse: kleine Steine 12x12, grosse Steine 20x20
            double displaySize = brickSize == LegoSize.small ? 12 : 20;

            // Canvas.Left und Canvas.Top bezeichnen die linke obere Ecke.
            double x = brickCenterX - displaySize / 2.0;
            double y = brickCenterY - displaySize / 2.0;

            Pose brickPose = new Pose(
                (int)Math.Round(x),
                (int)Math.Round(y),
                ProductionLinePose.Angle);

            bricks.Add(new LegoBrick(brickPose, color, brickSize));
        }
    }
}