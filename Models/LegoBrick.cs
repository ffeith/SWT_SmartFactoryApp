using System;
using System.Drawing;
using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

public class LegoBrick : Beweger
{
    private LegoColor color;
    private LegoSize size;

    // Verfolgt die Bewegungsphase der ursprünglichen Produktionslinie.
    private int versatz = 0;

    public LegoColor Color
    {
        get { return color; }
        set { color = value; }
    }

    public LegoSize Size
    {
        get { return size; }
        set { size = value; }
    }

    public Pose LegoPose { get; }

    public Parameters.BrickStatus Status { get; set; }
        = Parameters.BrickStatus.Moving;

    // -1 bedeutet: Der Stein belegt keinen Recyclingplatz.
    public int RecyclePlatz { get; set; } = -1;

    public LegoBrick(Pose pose, LegoColor color, LegoSize size)
        : base(pose, ViewParameters.Borders)
    {
        Color = color;
        Size = size;
        LegoPose = pose;
    }

    public void Andocken()
    {
        Status = Parameters.BrickStatus.Docked;

        // Falls der Stein aus dem Recycling kommt, wird sein Platz frei.
        RecyclePlatz = -1;
    }

    public override void Bewegen(Direction direction)
    {
        if (Status == Parameters.BrickStatus.Docked || RecyclePlatz >= 0)
            return;

        base.Bewegen(direction);
    }

    public override void Bewegen(double deltaX, double deltaY)
    {
        // Bewegungen der ursprünglichen Produktionslinie weiterverfolgen,
        // auch wenn der Stein gerade vom Roboter transportiert wird.
        double winkel = LegoPose.Angle * Math.PI / 180.0;

        double bewegungEntlangDerLinie =
            deltaX * Math.Cos(winkel)
            + deltaY * Math.Sin(winkel);

        if (bewegungEntlangDerLinie > 0)
            versatz++;
        else if (bewegungEntlangDerLinie < 0)
            versatz--;

        // Nur Steine auf ihrer Produktionslinie bewegen sich räumlich
        // mit der Linie. Transportierte Steine behalten ihre Position.
        if (Status != Parameters.BrickStatus.Moving || RecyclePlatz >= 0)
            return;

        base.Bewegen(deltaX, deltaY);
    }

    public void ZumKunden()
    {
        Status = Parameters.BrickStatus.ToCustomer;
    }

    public void ZumRecycling()
    {
        Status = Parameters.BrickStatus.ToRecycle;
    }

    public bool VersatzIstNull => versatz == 0;

    public void BewegenZumKunden()
    {
        if (Status == Parameters.BrickStatus.ToCustomer)
            base.Bewegen(Parameters.Direction.right);
    }

    public void BewegenZumRecycling()
    {
        if (Status == Parameters.BrickStatus.ToRecycle)
            base.Bewegen(Parameters.Direction.left);
    }

    public void ZurueckZurProduktionslinie()
    {
        base.Bewegen(
            LegoPose.X - Location.X,
            LegoPose.Y - Location.Y);

        Angle = LegoPose.Angle;
        RecyclePlatz = -1;
        Status = Parameters.BrickStatus.Moving;
    }

    public void AufRecyclePlatz(Pose pose, int platz)
    {
        base.Bewegen(
            pose.X - Location.X,
            pose.Y - Location.Y);

        Angle = pose.Angle;
        RecyclePlatz = platz;
        Status = Parameters.BrickStatus.Moving;
    }
}