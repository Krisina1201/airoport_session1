using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Airport;

public partial class EditFlight : Window
{
    public User23Context context;
    public Item selectItem;
    public User loginUser;
    public EditFlight()
    {
        InitializeComponent();
    }

    public EditFlight(Item selectedUser)
    {
        InitializeComponent();
        //context = new User23Context();

        //selectItem = selectedUser;

        //fromTextBlock.Text = selectedUser.From;
        //toTextBlock.Text = selectedUser.To;
        //aircraftTextBlock.Text = selectedUser.Aircraft;

        //dateTextBox.Text = selectedUser.Date.Date.ToString();
        //timeTextBox.Text = selectedUser.Time.TimeOfDay.ToString();
        //priceTextBox.Text = selectedUser.EconomyPrice.ToString();

    }

    public EditFlight(Item selectedUser, User localUser)
    {
        InitializeComponent();
        context = new User23Context();

        selectItem = selectedUser;

        loginUser = localUser;

        fromTextBlock.Text = selectedUser.From;
        toTextBlock.Text = selectedUser.To;
        aircraftTextBlock.Text = selectedUser.Aircraft;

        dateTextBox.Text = selectedUser.Date.Date.ToString();
        timeTextBox.Text = selectedUser.Time.TimeOfDay.ToString();
        priceTextBox.Text = selectedUser.EconomyPrice.ToString();

    }

    private void UpdateClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            string date = dateTextBox.Text as String;
            string time = timeTextBox.Text as String;
            string price = priceTextBox.Text as String;

            string checkDate = @"^\d{4}-\d{2}-\d{2}";
            string checkTime= @"^\d{2}:\d{2}:\d{2}";

            if (date == null || time == null || price == null)
            {
                ShowErrorDialog("Ошибка", "Заполните все поля");
            }
            else if (!Regex.IsMatch(date, checkDate, RegexOptions.IgnoreCase))
            {
                ShowErrorDialog("Ошибка!", "Ошибка в формате даты\nВерный формат: ГГГГ-ММ-ДД");
            }
            else if (!Regex.IsMatch(time, checkTime, RegexOptions.IgnoreCase))
            {
                ShowErrorDialog("Ошибка!", "Ошибка в формате времени\nВерный формат: чччч-мм-сс");
            }
            else
            {
                var objSchedule = context.Schedules.FirstOrDefault(e => e.Id == selectItem.Id);

                objSchedule.Time = TimeOnly.Parse(time);
                objSchedule.Date = DateOnly.Parse(date);
                objSchedule.Economyprice = decimal.Parse(price);

                try
                {
                    context.SaveChanges();
                    ShowErrorDialog("Успех", "Данные успешно сохранены!");
                    System.Threading.Thread.Sleep(5);
                    FlightWindow flightWindow = new FlightWindow(loginUser);
                    flightWindow.Show();
                    this.Close();
                } catch (Exception ex)
                {
                    ShowErrorDialog("Ошибка", $"Проблема с схранеием данных\n{ex}");
                }
            }

        } catch(Exception ex)
        {
            ShowErrorDialog("Ошибка", $"Произошла ошибка: {ex}");
        }
    }
    
    private void BackClick(object? sender, RoutedEventArgs e)
    {
        FlightWindow flightWindow = new FlightWindow(loginUser);
        flightWindow.Show();
        this.Close();
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