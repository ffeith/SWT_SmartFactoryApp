using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;

public partial class LegoBricksViewModel : ViewModelBase
{
    private LegoBrick brick;
    [ObservableProperty] private double x;
    [ObservableProperty] private double y;
    [ObservableProperty] private double angle;
    public Parameters.LegoColor Color => brick.Color;
    public double Width =>
    brick.Size == Parameters.LegoSize.large
        ? ViewParameters.BigBrickWidth
        : ViewParameters.SmallBrickWidth;

    public double Height =>
        brick.Size == Parameters.LegoSize.large
            ? ViewParameters.BigBrickHeight
            : ViewParameters.SmallBrickHeight;
    
    public LegoBricksViewModel(LegoBrick brick)
    {
        this.brick = brick;
        RefreshProperties();
    }

    public void RefreshProperties()
    {
        X = brick.Location.X;
        Y = brick.Location.Y;
        Angle = brick.LegoPose.Angle;
    }
}