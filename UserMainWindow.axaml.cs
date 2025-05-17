using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Diagnostics;
using System;

namespace Airport;

public partial class UserMainWindow : Window
{
    private DispatcherTimer _timer;
    private TimeSpan _elapsedTime;
    private Stopwatch _stopwatch;

    public UserMainWindow()
    {
        InitializeComponent();
    }

    private void exitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    public UserMainWindow(User user)
    {
        InitializeComponent();
        _stopwatch = new Stopwatch();
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100); 
        _timer.Tick += Timer_Tick;

        Loaded += (sender, e) => StartTimer();

        helloTextBlock.Text = $"Hi {user.Firstname}. Welcome to AMONIC Airlines.";
        crashCountTextBlock.Text = $"Number of crashes:     1";
    }

    private void StartTimer()
    {
        _stopwatch.Start();
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _elapsedTime = _stopwatch.Elapsed;
        timeInSession.Text = $"Time spent on system: {_elapsedTime.ToString(@"hh\:mm\:ss")}";
    }
}