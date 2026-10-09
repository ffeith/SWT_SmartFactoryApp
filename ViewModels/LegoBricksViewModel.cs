using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaApp.Models;

namespace MyAvaloniaApp.ViewModels;

public partial class LegoBricksViewModel : ViewModelBase
{
    private LegoBrick brick;
    [ObservableProperty] private double x;
    [ObservableProperty] private double y;
    [ObservableProperty] private double angle;
    
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