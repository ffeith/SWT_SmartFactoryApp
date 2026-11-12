using Avalonia.Controls;
using Avalonia.Interactivity;
using MyAvaloniaApp.ViewModels;

namespace MyAvaloniaApp.Views;

// Leitet Klicks aus der Oberfläche an das MainViewModel weiter.
public partial class MainWindow : Window
{
    private MainViewModel? ViewModel =>
        DataContext as MainViewModel;

    public MainWindow()
    {
        InitializeComponent();
    }

    // Roboterbewegung
    private void MoveUp_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUp();

    private void MoveDown_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDown();

    private void MoveLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveLeft();

    private void MoveRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveRight();

    private void MoveUpLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUpLeft();

    private void MoveUpRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUpRight();

    private void MoveDownLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDownLeft();

    private void MoveDownRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDownRight();

    private void StopButton_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.StopButton();

    // Roboterrotation
    private void TurnLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.TurnLeft();

    private void TurnRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.TurnRight();

    private void TurnToZeroDegrees_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TurnToZeroDegrees();

    // Timer
    private void TimerStart_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.TimerStart();

    private void TimerStop_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.TimerStop();

    // Fabrik und Bestellung
    private void SetupProductionLines_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.SetupProductionLines();

    private void Order_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.CreateOrder();

    // Transport
    private void GoToLine_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.GoToLine();

    private void GoToDelivery_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.GoToDelivery();

    private void Undock_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.Undock();
}