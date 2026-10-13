using System;
using System.Drawing;
using System.Collections.Generic;
using static MyAvaloniaApp.Models.ViewParameters;
using static MyAvaloniaApp.Models.Parameters;             //Alternativ zu den Properties Color und Size

namespace MyAvaloniaApp.Models;

public class ProductionLine
{
    public Pose ProductionLinePose { get; }
    private List<LegoBrick> bricks = new List<LegoBrick>();
    public List<LegoBrick> Bricks => bricks;
    public ProductionLine(Pose pose)
    {
        ProductionLinePose = pose;
        //CreateBricks();
    }
    public ProductionLine(Pose pose, Parameters.LegoColor color, Parameters.LegoSize size)
    {
        ProductionLinePose = pose;
        CreateBricks(color, size);
    }
    private void CreateBricks(Parameters.LegoColor color, Parameters.LegoSize size)
    {
        for (int i = 0; i < 4; i++)
        {
            Pose pose1 = new Pose(ProductionLinePose.X + 50 + 30 * i, ProductionLinePose.Y + 50 + 30 * i, ProductionLinePose.Angle);
            bricks.Add(new LegoBrick(pose1, color, size));
        }
    }
}