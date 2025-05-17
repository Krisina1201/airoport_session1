using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Airport;


public partial class UserMainWindow : Window
{
    private DispatcherTimer _timer;
    private TimeSpan _elapsedTime;
    private Stopwatch _stopwatch;

    public class Info
    {
        public DateOnly? Date { get; set; }
        public TimeOnly LoginTime { get; set; }
        public TimeOnly LogoutTime { get; set; }
        public string TimeInSystem { get; set; }
        public string? Error { get; set; }
    }

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
        using var context = new User23Context();
        InitializeComponent();
        _stopwatch = new Stopwatch();
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100); 
        _timer.Tick += Timer_Tick;

        Loaded += (sender, e) => StartTimer();

        helloTextBlock.Text = $"Hi {user.Firstname}. Welcome to AMONIC Airlines.";
        

        var employee = context.UserInfos.Where(e => e.UserId == user.Id);

        var loginList = employee.Where(e => e.Id % 2 == 1).ToList();
        var logoutList = employee.Where(e => e.Id % 2 == 0).ToList();

        List<Info> listInfo = new List<Info>();

        for (int i = 0; i < loginList.Count(); i++)
        {
            if (loginList[i].Entrance.HasValue && logoutList[i].Exit.HasValue)
            {
                listInfo.Add(new Info
                {
                    Date = DateOnly.FromDateTime(loginList[i].Entrance.Value.Date),
                    LoginTime = TimeOnly.FromDateTime(loginList[i].Entrance.Value),
                    LogoutTime = TimeOnly.FromDateTime(logoutList[i].Exit.Value),
                    TimeInSystem = (logoutList[i].Exit.Value - loginList[i].Entrance.Value).ToString(@"hh\:mm\:ss"),
                    Error = logoutList[i].Error
                });
            }
        }

        EmployeeDataGrid.ItemsSource = listInfo.ToList();

        int countError = listInfo.Count(e => e.Error != null);
        crashCountTextBlock.Text = $"Number of crashes:     {countError}";
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