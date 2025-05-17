using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Airport;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateOnly? date { get; set; }
    public string UserRole { get; set; }
    public string EmailAddress { get; set; }
    public string Office { get; set; }
    public int RoleId { get; set; }
    public bool? Active { get; set; }
    public string Password { get; set; }
}

public partial class AdminMainWindow : Window
{
    private ObservableCollection<Employee> _originalData;
    private ObservableCollection<Employee> _filteredData;
    public List<Employee> employees;
    private readonly User23Context context;

    public AdminMainWindow()
    {
        context = new User23Context();
        InitializeComponent();
        var itemsComboxOffice = context.Offices.Select(e => e.Title).ToList();
        itemsComboxOffice.Add("Всё");
        officeCombobox.ItemsSource = itemsComboxOffice;

        employees = context.Users.Select(e => new Employee
        {
            Id = e.Id,
            FirstName = e.Firstname,
            LastName = e.Lastname,
            Age = 40,
            UserRole = e.Role.Title,
            EmailAddress = e.Email,
            date = e.Birthdate,
            Office = e.Office.Title,
            Active = e.Active,
            Password = e.Password
        }).ToList();
       

        _originalData = new ObservableCollection<Employee>(employees);
        _filteredData = new ObservableCollection<Employee>(_originalData);

        EmployeeDataGrid.ItemsSource = _filteredData;

        officeCombobox.SelectionChanged += filterComboboxByOffice;
    }


    public void filterComboboxByOffice(object sender, SelectionChangedEventArgs e)
    {
        string officeTitle = officeCombobox.SelectedItem.ToString();
        if (officeTitle != "Всё")
        {
            if (officeTitle == null) return;

            _filteredData.Clear();

            var filter = _originalData.Where(e => e.Office == officeTitle);

            foreach (var item in filter)
            {
                _filteredData.Add(item);
            }
        } else
        {
            foreach (var item in employees)
            {
                _filteredData.Add(item);
            }
        }
    }

    private async void AddUserClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AddUser addUser = new AddUser();
        await addUser.ShowDialog(this);
        RefreshData();
    }

    private void exitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }

    private void logoutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void changeRoleClock(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var selectUser = EmployeeDataGrid.SelectedItem as Employee;
        if (selectUser == null)
        {
            ShowDialog("Ошибка!", "Выберите пользователя!");
            return;
        }
        int roleId;

        if (selectUser.UserRole == "Administrator") 
        {
            selectUser.UserRole = "User";
            roleId = 2;
        }
        else { selectUser.UserRole = "Administrator"; roleId = 1; }

        var officeId = context.Offices.FirstOrDefault(e => e.Title == selectUser.Office).Id;

        var updUser = new User
        {
            Id = selectUser.Id,
            Birthdate = selectUser.date,
            Email = selectUser.EmailAddress,
            Firstname = selectUser.FirstName,
            Lastname = selectUser.LastName,
            Active = selectUser.Active,
            Roleid = roleId,
            Officeid = officeId,
            Password = selectUser.Password
        };

        try
        {
            context.Users.Update(updUser);
            context.SaveChanges();
            ShowDialog("Успешно!", $"Роль пользователя {selectUser.FirstName} {selectUser.LastName}\nизменена на {selectUser.UserRole}");
            RefreshData(); 
        }
        catch
        {
            ShowDialog("Ошибка!", "Не удалось изменить роль\nПопробуйте снова!");
        }
    }

    private void ShowDialog(string title, string message)
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

    private void RefreshData()
    {
        employees = context.Users.Select(e => new Employee
        {
            Id = e.Id,
            FirstName = e.Firstname,
            LastName = e.Lastname,
            Age = 40,
            UserRole = e.Role.Title,
            EmailAddress = e.Email,
            date = e.Birthdate,
            Office = e.Office.Title,
            Active = e.Active,
            Password = e.Password
        }).ToList();

        _originalData = new ObservableCollection<Employee>(employees);
        _filteredData = new ObservableCollection<Employee>(_originalData);

        EmployeeDataGrid.ItemsSource = null; 
        EmployeeDataGrid.ItemsSource = _filteredData; 

        if (officeCombobox.SelectedItem != null)
        {
            filterComboboxByOffice(officeCombobox, null);
        }
    }

}