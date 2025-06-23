using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Airport;

public partial class FlightUserWindow : Window
{
    public User23Context context;
    public ObservableCollection<Item> _originalData;
    public ObservableCollection<Item> _filteredData;
    public List<Item> employees;
    public List<Item> filter;
    public FlightUserWindow()
    {
        InitializeComponent();
        context = new User23Context();

        _originalData = new ObservableCollection<Item>();
        _filteredData = new ObservableCollection<Item>();
        filter = new List<Item>();
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

    private void backClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
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

    private async void ImportChangesClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Создаем диалог выбора папки
            var topLevel = TopLevel.GetTopLevel(this);
            var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select folder to save Excel file",
                AllowMultiple = false
            });

            if (folder.Count == 0) return; // Пользователь отменил выбор

            var selectedFolder = folder[0];

            // Создаем текстовое поле для имени файла
            var fileNameTextBox = new TextBox
            {
                Text = $"FlightsExport_{DateTime.Now:yyyyMMdd_HHmmss}",
                Margin = new Thickness(5),
                MinWidth = 200
            };

            // Создаем диалог для ввода имени файла
            var fileNameDialog = new Window
            {
                Title = "Enter file name",
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new StackPanel
                {
                    Children =
                {
                    new TextBlock { Text = "File name:", Margin = new Thickness(5) },
                    fileNameTextBox,
                    new Button
                    {
                        Content = "Save",
                        Margin = new Thickness(5),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        Command = new RelayCommand(async (param) =>
                        {
                            await SaveToExcel(selectedFolder, fileNameTextBox.Text);
                            ((Window)param).Close();
                        })
                    }
                }
                }
            };

            // Передаем диалог как параметр команды
            ((Button)((StackPanel)fileNameDialog.Content).Children[2]).CommandParameter = fileNameDialog;

            await fileNameDialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Error", $"Failed to export data: {ex.Message}");
        }
    }

    private async Task SaveToExcel(IStorageFolder folder, string fileName)
    {
        try
        {
            // Добавляем расширение .xlsx если его нет
            if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".xlsx";
            }

            // Создаем новую книгу Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Flights");

                // Заголовки столбцов
                worksheet.Cell(1, 1).Value = "Date";
                worksheet.Cell(1, 2).Value = "Time";
                worksheet.Cell(1, 3).Value = "From";
                worksheet.Cell(1, 4).Value = "To";
                worksheet.Cell(1, 5).Value = "Flight Number";
                worksheet.Cell(1, 6).Value = "Aircraft";
                worksheet.Cell(1, 7).Value = "Economy Price";

                // Данные
                var data = EmployeeDataGrid.ItemsSource as IEnumerable<Item>;
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 2).Value = item.Time.ToString("hh\\:mm");
                    worksheet.Cell(row, 3).Value = item.From;
                    worksheet.Cell(row, 4).Value = item.To;
                    worksheet.Cell(row, 5).Value = item.FlightNumber;
                    worksheet.Cell(row, 6).Value = item.Aircraft;
                    worksheet.Cell(row, 7).Value = item.EconomyPrice;
                    row++;
                }

                // Получаем путь к файлу
                var file = await folder.CreateFileAsync(fileName);

                // Сохраняем файл
                await using (var stream = await file.OpenWriteAsync())
                {
                    workbook.SaveAs(stream);
                }

                ShowErrorDialog("Success", $"Data exported successfully to:\n{file.Name}");
            }
        }
        catch (Exception ex)
        {
            ShowErrorDialog("Error", $"Failed to save Excel file: {ex.Message}");
        }
    }
}
