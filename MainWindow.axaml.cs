using Airport.Models;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Linq;

namespace Airport;

public partial class MainWindow : Window
{
    public int count;
    private DispatcherTimer _lockTimer;
    private int _remainingLockTime = 10;
    private bool _isLocked = false;

    public MainWindow()
    {
        InitializeComponent();

        _lockTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _lockTimer.Tick += LockTimer_Tick;
    }

    private void exitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void LoginClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_isLocked)
        {
            ShowErrorDialog("Система заблокирована",
                $"Попробуйте снова через {_remainingLockTime} сек.");
            return;
        }

        using var context = new User23Context();

        string userName = userNameBox.Text;
        string password = passwordBox.Text;

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            ShowErrorDialog("Ошибка ввода", "Необходимо заполнить все поля!");
            return;
        }

        try
        {
            var user = context.Users.FirstOrDefault(u => u.Email == userName);

            if (user == null)
            {
                HandleFailedAttempt();
                ShowErrorDialog("Ошибка", "Пользователь не найден!");
                return;
            }

            if (user.Password != password)
            {
                HandleFailedAttempt();
                ShowErrorDialog("Неверный пароль",
                    $"Попробуйте еще раз. Осталось попыток: {3 - count}");
                return;
            }

            context.UserInfos.Add(new UserInfo
            {
                UserId = user.Id,
                Entrance = DateTime.Now
            });

            Program.userId = user.Id;

            context.SaveChanges();

            if (user.Roleid == 1)
            {
                new AdminMainWindow().Show();
            }
            else
            {
                new UserMainWindow(user).Show();
            }

            this.Close();
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Ошибка", $"Произошла ошибка: {ex.Message}");
        }
    }

    private void HandleFailedAttempt()
    {
        count++;

        if (count >= 3)
        {
            StartLockTimer();
        }
    }

    private void StartLockTimer()
    {
        _isLocked = true;
        _remainingLockTime = 10;
        loginButton.IsEnabled = false; 
        _lockTimer.Start();

        UpdateTimerText();
    }

    private void LockTimer_Tick(object sender, EventArgs e)
    {
        _remainingLockTime--;
        UpdateTimerText();

        if (_remainingLockTime <= 0)
        {
            _lockTimer.Stop();
            _isLocked = false;
            count = 0; 
            loginButton.IsEnabled = true; 
            timer.Text = string.Empty;
        }
    }

    private void UpdateTimerText()
    {
        if (timer != null)
        {
            timer.Text = $"Система заблокирована. Осталось: {_remainingLockTime} сек.";
        }
    }

    private void ShowErrorDialog(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Content = new TextBlock { Text = message },
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        dialog.ShowDialog(this);
    }
}