using System;
using System.Collections.Generic;
using System.Drawing;
using static MyAvaloniaApp.Models.ViewParameters;

namespace MyAvaloniaApp.Models;

public class Robot : Beweger
{
    private Parameters.Direction robotDirection = Parameters.Direction.stop;
    public Parameters.Direction RobotDirection
    {
        get => robotDirection;
        private set => robotDirection = value;
    }

    public Robot(Point location, Rectangle borders, IReadOnlyList<ProductionLine> productionLines) : base(location, borders, productionLines)
    {
        
    }

    public override void Bewegen(Parameters.Direction direction)
    {
        RobotDirection = direction;
        base.Bewegen(direction);
    }
}