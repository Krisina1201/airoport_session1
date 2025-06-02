using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace Airport;


public class Item
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime Time { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string FlightNumber { get; set; }
    public string Aircraft { get; set; }
    public decimal EconomyPrice { get; set; }
}
public partial class FlightWindow : Window
{
    public User23Context context;
    public ObservableCollection<Item> _originalData;
    public ObservableCollection<Item> _filteredData;
    public List<Item> employees;
    public List<Item> filter;
    public User user;


    public FlightWindow()
    {
        InitializeComponent();
        //context = new User23Context();

        //_originalData = new ObservableCollection<Item>();
        //_filteredData = new ObservableCollection<Item>();
        //filter = new List<Item>();

        //this.Loaded += (s, e) => RefreshData();
    }
    
    public FlightWindow(User loginUser)
    {
        InitializeComponent();
        context = new User23Context();

        _originalData = new ObservableCollection<Item>();
        _filteredData = new ObservableCollection<Item>();
        filter = new List<Item>();

        user = loginUser;

        this.Loaded += (s, e) => RefreshData();
    }

    public void RefreshData()
    {
        employees = context.Schedules.Select(e => new Item
        {
            Id = e.Id,
            Date = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day),
            Time = new DateTime(1, 1, 1, e.Time.Hour, e.Time.Minute, e.Time.Second),
            From = e.Route.Arrivalairport.Iatacode,
            To = e.Route.Departureairport.Iatacode,
            FlightNumber = e.Flightnumber,
            Aircraft = e.Aircraft.Makemodel,
            EconomyPrice = e.Economyprice
        }).ToList();

        _originalData.Clear();
        foreach (var item in employees)
        {
            _originalData.Add(item);
        }
        _filteredData.Clear();
        foreach (var item in _originalData)
        {
            _filteredData.Add(item);
        }

        fromCombobox.ItemsSource = context.Airports.Select(e => e.Iatacode).ToList();
        toCombobox.ItemsSource = context.Airports.Select(e => e.Iatacode).ToList();

        string[] items = ["Утро", "День", "Вечер", "Ночь"];
        dateTimeCombobox.ItemsSource = items;

        EmployeeDataGrid.ItemsSource = _originalData;
    }

    private void CancelFlightClick(object? sender, RoutedEventArgs e)
    {
        var selectedItem = EmployeeDataGrid.SelectedItem as Item;
        if (selectedItem == null)
        {
            ShowErrorDialog("Ошибка", "Выберите рейс для удаления");
            return;
        }

        try
        {
            var scheduleToDelete = context.Schedules.FirstOrDefault(s => s.Id == selectedItem.Id);
            if (scheduleToDelete != null)
            {
                context.Schedules.Remove(scheduleToDelete);
                context.SaveChanges();
                RefreshData();
                ShowErrorDialog("Успех", "Рейс успешно удален");
            }
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Ошибка", $"Не удалось удалить рейс: {ex.Message}");
        }
    }


    private void EditClick(object sender, RoutedEventArgs e)
    {
        Item selectedItemForEdit = EmployeeDataGrid.SelectedItem as Item;

        if (selectedItemForEdit == null) { ShowErrorDialog("Ошибка!", "Выберите полет который хотите обновить"); return; }

        EditFlight editFlight = new EditFlight(selectedItemForEdit, user);
        editFlight.Show();
        this.Close();
    }

    private void backClick(object? sender, RoutedEventArgs e)
    {
        AdminMainWindow adminMainWindow = new AdminMainWindow(user);
        adminMainWindow.Show();
    }

    public void FilterClick(object? sender, RoutedEventArgs e)
    {
        string outbound = OutboundTextBox.Text;
        string flightNumber = flifhtNumberTextBox.Text;

        string from = fromCombobox.SelectedItem as string;
        string to = toCombobox.SelectedItem as string;
        string time = dateTimeCombobox.SelectedItem as string;

        var filtered = _originalData.AsEnumerable();

        if (!string.IsNullOrEmpty(from))
            filtered = filtered.Where(item => item.From == from);

        if (!string.IsNullOrEmpty(to))
            filtered = filtered.Where(item => item.To == to);

        if (!string.IsNullOrEmpty(time))
        {
            if (time == "Утро")
                filtered = filtered.Where(e => e.Time.Hour < 12 && e.Time.Hour >= 6);
            else if (time == "День")
                filtered = filtered.Where(e => e.Time.Hour < 18 && e.Time.Hour >= 12);
            else if (time == "Вечер")
                filtered = filtered.Where(e => e.Time.Hour < 24 && e.Time.Hour >= 18); // Исправлено условие (было < 00)
            else if (time == "Ночь")
                filtered = filtered.Where(e => e.Time.Hour < 6 || e.Time.Hour >= 0); // Исправлено условие для ночи
        }

        string checkDate = @"^\d{4}-\d{2}-\d{2}";

        if (!string.IsNullOrEmpty(outbound))
        {
            if (!Regex.IsMatch(outbound, checkDate, RegexOptions.IgnoreCase))
            {
                ShowErrorDialog("Ошибка!", "Неверный формат даты");
            }
            else
            {
                filtered = filtered.Where(e => e.Date == DateTime.Parse(outbound));
            }
        }

        if (!string.IsNullOrEmpty(flightNumber))
        {
            filtered = filtered.Where(e => e.FlightNumber == flightNumber); // Исправлено: было e.Aircraft == flightNumber
        }

        if (!filtered.Any())
        {
            ShowErrorDialog("Ошибка!", "Под ваши настройки ничего не найдено");
        }
        else
        {
            _filteredData.Clear();
            foreach (var item in filtered)
            {
                _filteredData.Add(item);
            }

            EmployeeDataGrid.ItemsSource = _filteredData;
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