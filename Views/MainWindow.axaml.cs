using Avalonia.Controls;
using Avalonia.Interactivity;
using MyAvaloniaApp.ViewModels;

namespace MyAvaloniaApp.Views;


// Leitet Ereignisse der Benutzeroberfläche
// an das MainViewModel weiter.
public partial class MainWindow : Window
{
    // Zugriff auf das MainViewModel.
    // Falls kein passendes ViewModel vorhanden ist,
    // wird null zurückgegeben.
    private MainViewModel? ViewModel =>
        DataContext as MainViewModel;


    // --------------------------------------------------------
    // Konstruktor
    // --------------------------------------------------------

    public MainWindow()
    {
        InitializeComponent();
    }


    // --------------------------------------------------------
    // Roboterbewegung
    // --------------------------------------------------------

    private void MoveUp_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUp();

    private void MoveDown_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDown();

    private void MoveLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveLeft();

    private void MoveRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveRight();


    // Diagonale Bewegungen.
    private void MoveUpLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUpLeft();

    private void MoveUpRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveUpRight();

    private void MoveDownLeft_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDownLeft();

    private void MoveDownRight_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.MoveDownRight();


    // Stoppt die aktuelle Roboterbewegung.
    private void Stop_Click(object? sender, RoutedEventArgs e) =>
        ViewModel?.StopButton();


    // --------------------------------------------------------
    // Roboterrotation
    // --------------------------------------------------------

    private void MoveRotateLeft_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TurnLeft();


    private void MoveRotateRight_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TurnRight();


    // Setzt die Ausrichtung des Roboters auf null Grad zurück.
    private void ResetRotation_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TurnToZeroDegrees();


    // --------------------------------------------------------
    // Timer
    // --------------------------------------------------------

    private void StartTimer_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TimerStart();


    private void StopTimer_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.TimerStop();


    // --------------------------------------------------------
    // Produktionslinien und Bestellung
    // --------------------------------------------------------

    private void SetupProductionLines_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.SetupProductionLines();


    private void Order_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.CreateOrder();


    // --------------------------------------------------------
    // Transport und Undocken
    // --------------------------------------------------------

    private void GoToLine_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.GoToLine();


    private void GoToDelivery_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.GoToDelivery();


    private void UndockButton_Click(
        object? sender,
        RoutedEventArgs e) =>
        ViewModel?.Undock();
}