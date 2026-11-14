using System;

namespace MyAvaloniaApp.Models;


// Repräsentiert einen LEGO-Stein innerhalb der SmartFactory.
// Ein Stein kann auf einer Produktionslinie liegen,
// vom Roboter getragen oder transportiert werden.
public class LegoBrick : Beweger
{
    // Ursprüngliche Position des Steins auf seiner Produktionslinie.
    // Wird für die Rückkehr nach einer erfolgreichen Lieferung benötigt.
    private readonly Pose ursprungsPose;

    // Bewegung des Steins entlang seiner ursprünglichen Produktionslinie.
    private int versatz;

    // Nummer des belegten Recyclingplatzes.
    // -1 bedeutet, dass kein Platz belegt ist.
    private int recyclePlatz = -1;

    // Aktueller Zustand des Steins.
    private BrickStatus status = BrickStatus.Moving;


    // --------------------------------------------------------
    // Eigenschaften
    // --------------------------------------------------------

    public LegoColor Color { get; set; }

    // Farbe und Größe ändern sich während der Lebensdauer nicht.

    public LegoSize Size { get; }

    // Aktueller Zustand des Steins.
    public BrickStatus Status
    {
        get => status;
        set => status = value;
    }

    // Nummer des belegten Recyclingplatzes.
    public int RecyclePlatz => recyclePlatz;

    // Gibt an, ob der ursprüngliche Bewegungsversatz
    // wieder ausgeglichen wurde.
    public bool VersatzIstNull => versatz == 0;


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public LegoBrick(
        Pose pose,
        LegoColor color,
        LegoSize size)
        : base(pose, ViewParameters.Borders)
    {
        ursprungsPose = pose;

        Color = color;
        Size = size;
    }


    // --------------------------------------------------------
    // Bewegung
    // --------------------------------------------------------

    // Normale Bewegung des Steins.
    // Angedockte Steine und Steine auf Recyclingplätzen
    // dürfen nicht unabhängig bewegt werden.
    public override void Bewegen(Direction direction)
    {
        if (Status == BrickStatus.Docked || recyclePlatz >= 0)
            return;

        base.Bewegen(direction);
    }


    // Bewegung durch das Förderband.
    //
    // Der Versatz wird auch dann aktualisiert, wenn der Stein
    // gerade transportiert oder vom Roboter getragen wird.
    public override void Bewegen(double deltaX, double deltaY)
    {
        AktualisiereVersatz(deltaX, deltaY);

        // Räumlich bewegen sich nur Steine auf normalen
        // Produktionslinien.
        if (Status != BrickStatus.Moving || recyclePlatz >= 0)
            return;

        base.Bewegen(deltaX, deltaY);
    }


    // Berechnet die Bewegung entlang der ursprünglichen Produktionslinie.
    private void AktualisiereVersatz(double deltaX, double deltaY)
    {
        double winkel =
            ursprungsPose.Angle * Math.PI / 180.0;

        // Bewegung auf die ursprüngliche Linienrichtung projizieren.
        double bewegungEntlangDerLinie =
            deltaX * Math.Cos(winkel) +
            deltaY * Math.Sin(winkel);

        if (bewegungEntlangDerLinie > 0)
        {
            versatz++;
        }
        else if (bewegungEntlangDerLinie < 0)
        {
            versatz--;
        }
    }


    // --------------------------------------------------------
    // Transport
    // --------------------------------------------------------

    // Markiert den Stein für den Transport zum Kunden.
    public void ZumKunden()
    {
        Status = BrickStatus.ToCustomer;
    }


    // Markiert den Stein für den Transport zum Recycling.
    public void ZumRecycling()
    {
        Status = BrickStatus.ToRecycle;
    }


    // Bewegt einen akzeptierten Stein horizontal zum Kunden.
    public void BewegenZumKunden()
    {
        if (Status != BrickStatus.ToCustomer)
            return;

        base.Bewegen(Parameters.step, 0);
    }


    // Bewegt einen abgelehnten Stein horizontal zur Recyclinglinie.
    public void BewegenZumRecycling()
    {
        if (Status != BrickStatus.ToRecycle)
            return;

        base.Bewegen(-Parameters.step, 0);
    }


    // --------------------------------------------------------
    // Recyclingplatz
    // --------------------------------------------------------

    // Legt den Stein auf einem freien Recyclingplatz ab.
    public void AufRecyclePlatz(Pose pose, int platz)
    {
        if (platz < 0 || platz >= Parameters.bricksPerLine)
            return;

        SetzePose(pose);

        recyclePlatz = platz;
        Status = BrickStatus.Moving;
    }


    // --------------------------------------------------------
    // Andocken
    // --------------------------------------------------------

    // Wird aufgerufen, wenn der Roboter den Stein aufnimmt.
    public void Andocken()
    {
        Status = BrickStatus.Docked;

        // Der aufgenommene Stein belegt keinen Recyclingplatz mehr.
        recyclePlatz = -1;
    }


    // --------------------------------------------------------
    // Zurücksetzen
    // --------------------------------------------------------

    // Setzt den Stein nach erfolgreicher Lieferung
    // auf seine ursprüngliche Produktionsposition zurück.
    public void ZurueckZurProduktionslinie()
    {
        SetzePose(ursprungsPose);

        versatz = 0;
        recyclePlatz = -1;
        Status = BrickStatus.Moving;
    }
}