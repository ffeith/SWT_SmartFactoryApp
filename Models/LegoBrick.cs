using static MyAvaloniaApp.Models.Parameters;

namespace MyAvaloniaApp.Models;

// Repräsentiert einen LEGO-Stein in der Fabrik.
public class LegoBrick : Beweger
{
    // Ursprüngliche Position auf der Produktionslinie.
    public Pose LegoPose { get; }

    public LegoColor Color { get; }
    public LegoSize Size { get; }

    public BrickStatus Status { get; set; } = BrickStatus.Moving;

    // -1 bedeutet, dass der Stein keinen Recyclingplatz belegt.
    public int RecyclePlatz { get; private set; } = -1;

    // Abstand zur ursprünglichen Position entlang der Produktionslinie.
    private int versatz;

    public bool VersatzIstNull => versatz == 0;

    public LegoBrick(Pose pose, LegoColor color, LegoSize size)
        : base(pose, ViewParameters.Borders)
    {
        LegoPose = pose;
        Color = color;
        Size = size;
    }

    // Wird aufgerufen, wenn der Roboter den Stein aufnimmt.
    public void Andocken()
    {
        Status = BrickStatus.Docked;
        RecyclePlatz = -1;
    }

    // Normale manuelle Bewegung eines Steins.
    public override void Bewegen(Direction direction)
    {
        if (Status == BrickStatus.Docked || RecyclePlatz >= 0)
            return;

        base.Bewegen(direction);
    }

    // Verfolgt die Bewegung der ursprünglichen Produktionslinie.
    public override void Bewegen(double deltaX, double deltaY)
    {
        double lineAngle =
            LegoPose.Angle * System.Math.PI / 180.0;

        double movementAlongLine =
            deltaX * System.Math.Cos(lineAngle) +
            deltaY * System.Math.Sin(lineAngle);

        if (movementAlongLine > 0)
            versatz++;
        else if (movementAlongLine < 0)
            versatz--;

        // Nur Steine auf ihrer normalen Produktionslinie bewegen sich räumlich.
        if (Status != BrickStatus.Moving || RecyclePlatz >= 0)
            return;

        base.Bewegen(deltaX, deltaY);
    }

    public void ZumKunden()
    {
        Status = BrickStatus.ToCustomer;
    }

    public void ZumRecycling()
    {
        Status = BrickStatus.ToRecycle;
    }

    // Transportiert den Stein horizontal in Richtung Kunde.
    public void BewegenZumKunden()
    {
        if (Status == BrickStatus.ToCustomer)
        {
            base.Bewegen(Parameters.step, 0);
        }
    }

    // Transportiert den Stein horizontal in Richtung Recyclinglinie.
    public void BewegenZumRecycling()
    {
        if (Status == BrickStatus.ToRecycle)
        {
            base.Bewegen(-Parameters.step, 0);
        }
    }

    // Setzt den Stein an seine ursprüngliche Position zurück.
    public void ZurueckZurProduktionslinie()
    {
        base.Bewegen(
            LegoPose.X - X,
            LegoPose.Y - Y);

        Pose = LegoPose;

        RecyclePlatz = -1;
        versatz = 0;
        Status = BrickStatus.Moving;
    }

    // Legt einen abgelehnten Stein auf einem Recyclingplatz ab.
    public void AufRecyclePlatz(Pose recyclePose, int platz)
    {
        base.Bewegen(
            recyclePose.X - X,
            recyclePose.Y - Y);

        Pose = recyclePose;

        RecyclePlatz = platz;
        Status = BrickStatus.Moving;
    }

    public override string ToString()
    {
        return $"{Color} {Size}";
    }
}