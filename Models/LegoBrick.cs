using System;
using System.Drawing;
using static MyAvaloniaApp.Models.Parameters;             //Alternativ zu den Properties Color und Size

namespace MyAvaloniaApp.Models;

public class LegoBrick : Beweger
{
    private LegoColor color;
    private LegoSize size;
    //public Parameters.LegoColor Color { get; }             //Alternative mit Properties
    //public Parameters.LegoSize Size { get; }
    public LegoColor Color { get {return color; } set {color = value;} }
    public LegoSize Size { get {return size; } set {size = value;} }
    public Pose LegoPose { get; }


    public LegoBrick(Pose pose, LegoColor color, LegoSize size) : base(pose, ViewParameters.Borders)
    {
        Color = color;
        Size = size;
        LegoPose = pose;
    }
}