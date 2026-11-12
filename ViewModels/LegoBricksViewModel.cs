using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;

// Stellt einen einzelnen LEGO-Stein in der grafischen Oberfläche dar.
public partial class LegoBrickViewModel : ViewModelBase
{
    private LegoBrick? brick;

    [ObservableProperty]
    private double x;

    [ObservableProperty]
    private double y;

    [ObservableProperty]
    private double angle;

    public double Width =>
        brick == null
            ? 0
            : brick.Size == Parameters.LegoSize.large
                ? ViewParameters.BigBrickWidth
                : ViewParameters.SmallBrickWidth;

    public double Height =>
        brick == null
            ? 0
            : brick.Size == Parameters.LegoSize.large
                ? ViewParameters.BigBrickHeight
                : ViewParameters.SmallBrickHeight;

    public IBrush Fill => brick?.Color switch
    {
        Parameters.LegoColor.red => Brushes.Red,
        Parameters.LegoColor.blue => Brushes.Blue,
        Parameters.LegoColor.green => Brushes.Green,
        Parameters.LegoColor.yellow => Brushes.Yellow,
        _ => Brushes.Transparent
    };

    public bool IsVisible => brick != null;

    public LegoBrickViewModel()
    {
    }

    public LegoBrickViewModel(LegoBrick brick)
    {
        SetBrick(brick);
    }

    // Verknüpft das ViewModel mit einem echten Stein.
    public void SetBrick(LegoBrick? newBrick)
    {
        brick = newBrick;

        OnPropertyChanged(nameof(Width));
        OnPropertyChanged(nameof(Height));
        OnPropertyChanged(nameof(Fill));
        OnPropertyChanged(nameof(IsVisible));

        Refresh();
    }

    // Aktualisiert Position und Ausrichtung.
    public void Refresh()
    {
        if (brick == null)
        {
            X = 0;
            Y = 0;
            Angle = 0;

            return;
        }

        X = brick.X;
        Y = brick.Y;
        Angle = brick.Angle;
    }
}