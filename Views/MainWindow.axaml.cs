using Avalonia.Controls;
using Avalonia.Interactivity;
using MyAvaloniaApp.ViewModels;

namespace MyAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;
    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveUp();
    }
    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveDown();
    }
    private void MoveLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveLeft();
    }
    private void MoveRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveRight();
    }
    private void MoveUpLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveUpLeft();
    }
    private void MoveUpRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveUpRight();
    }
    private void MoveDownLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveDownLeft();
    }
    private void MoveDownRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveDownRight();
    }
    private void TurnLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TurnLeft();
    }
    private void TurnRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TurnRight();
    }
    private void TurnToZeroDegrees_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TurnToZeroDegrees();
    }
    private void TimerStart_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TimerStart();
    }
    private void TimerStop_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TimerStop();
    }
}