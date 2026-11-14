using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;


// Stellt einen einzelnen LEGO-Stein in der Oberfläche dar.
public partial class LegoBrickViewModel : ViewModelBase
{
    // Der LEGO-Stein, dessen Darstellung dieses ViewModel übernimmt.
    private LegoBrick? brick;

    // Aktuelle Position und Ausrichtung für die grafische Darstellung.
    [ObservableProperty]
    private double x;

    [ObservableProperty]
    private double y;

    [ObservableProperty]
    private double angle;


    // --------------------------------------------------------
    // Eigenschaften für die grafische Darstellung
    // --------------------------------------------------------

    // Breite abhängig von der Größe des LEGO-Steins.
    public double Width =>
        brick == null
            ? 0
            : brick.Size == LegoSize.large
                ? ViewParameters.BigBrickWidth
                : ViewParameters.SmallBrickWidth;


    // Höhe abhängig von der Größe des LEGO-Steins.
    public double Height =>
        brick == null
            ? 0
            : brick.Size == LegoSize.large
                ? ViewParameters.BigBrickHeight
                : ViewParameters.SmallBrickHeight;


    // Farbe abhängig von der LEGO-Farbe.
    public IBrush Fill => brick?.Color switch
    {
        LegoColor.red => Brushes.Red,
        LegoColor.blue => Brushes.Blue,
        LegoColor.green => Brushes.Green,
        LegoColor.yellow => Brushes.Yellow,
        _ => Brushes.Transparent
    };


    // Ein Stein ist sichtbar, sobald ein echtes Brick-Objekt zugewiesen wurde.
    public bool IsVisible => brick != null;


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public LegoBrickViewModel()
    {
    }


    // --------------------------------------------------------
    // LEGO-Stein zuweisen
    // --------------------------------------------------------

    // Verbindet das ViewModel mit einem echten LEGO-Stein.
    public void SetBrick(LegoBrick? newBrick)
    {
        brick = newBrick;

        // Die grafischen Eigenschaften neu berechnen.
        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        //OnPropertyChanged(nameof(Fill));
        OnPropertyChanged(nameof(IsVisible));

        Refresh();
    }


    // --------------------------------------------------------
    // Darstellung aktualisieren
    // --------------------------------------------------------

    // Übernimmt die aktuelle Position und Ausrichtung
    // des LEGO-Steins in das ViewModel.
    public void Refresh()
    {
        if (brick == null)
        {
            X = 0;
            Y = 0;
            Angle = 0;
            OnPropertyChanged(nameof(Fill));

            return;
        }

        X = brick.X;
        Y = brick.Y;
        Angle = brick.Angle;
        OnPropertyChanged(nameof(Fill));
    }
}