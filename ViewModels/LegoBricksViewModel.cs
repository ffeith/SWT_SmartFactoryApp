using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaApp.Models;
using Avalonia.Media;

namespace MyAvaloniaApp.ViewModels;

public partial class LegoBricksViewModel : ViewModelBase
{
    private LegoBrick? brick;

    [ObservableProperty]
    private double x;

    [ObservableProperty]
    private double y;

    [ObservableProperty]
    private double angle;

    // Größe des Steins für die GUI.
    public double Width =>
        brick == null ? 0 :
        brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickWidth
            : ViewParameters.SmallBrickWidth;

    public double Height =>
        brick == null ? 0 :
        brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickHeight
            : ViewParameters.SmallBrickHeight;

    // Farbe dynamisch aus dem tatsächlichen Brick übernehmen.
    public IBrush Fill
    {
        get
        {
            if (brick == null)
                return Brushes.Transparent;

            return brick.Color switch
            {
                Parameters.LegoColor.red => Brushes.Red,
                Parameters.LegoColor.blue => Brushes.Blue,
                Parameters.LegoColor.green => Brushes.Green,
                Parameters.LegoColor.yellow => Brushes.Yellow,
                _ => Brushes.Transparent
            };
        }
    }

    // Ein leerer Anzeigeplatz soll nicht sichtbar sein.
    public bool IsVisible => brick != null;

    public LegoBricksViewModel()
    {
        // Zunächst existiert noch kein Brick.
    }

    public LegoBricksViewModel(LegoBrick brick)
    {
        SetBrick(brick);
    }

    // Verbindet das ViewModel mit einem echten LEGO-Stein.
    public void SetBrick(LegoBrick newBrick)
    {
        brick = newBrick;

        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(Fill));
        OnPropertyChanged(nameof(IsVisible));

        RefreshProperties();
    }

    // Aktualisiert Position und Winkel des Steins.
    public void RefreshProperties()
    {
        if (brick == null)
            return;

        X = brick.Location.X;
        Y = brick.Location.Y;
        Angle = brick.Angle;
    }
}