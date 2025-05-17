using Airport.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Airport;

public partial class AddUser : Window
{
    private readonly User23Context context;

    public AddUser()
    {
        context = new User23Context();
        InitializeComponent();
        officeComboBox.ItemsSource = context.Offices.Select(e => e.Title).ToList();

    }

    private void cancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //AdminMainWindow adminMainWindow = new AdminMainWindow();
        //adminMainWindow.Show();
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

    private void saveClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        string inputEmail = emailBox.Text;
        string inputFirstName = firstNameBox.Text;
        string inputLastName = lastNameBox.Text;
        string inputPassword = passwordBox.Text;
        string inputBirthday = birthdayBox.Text;
        var inputOffice = officeComboBox.SelectedItem;

        string checkDate = @"^\d{4}-\d{2}-\d{2}";

        if (inputBirthday == null || inputEmail == null || inputFirstName == null ||
            inputLastName == null || inputPassword == null || inputOffice == null)
        {
            ShowErrorDialog("Ошибка", "Все поля должны быть заполнены");
        }
        else if (!Regex.IsMatch(inputBirthday, checkDate, RegexOptions.IgnoreCase))
        {
            ShowErrorDialog("Ошибка", "Не верный формат даты!\nНеобходимый формат 'ГГГГ-ММ-ДД'");
        }
        else
        {
            try
            {
                int officeId = context.Offices.FirstOrDefault(e => e.Title == inputOffice.ToString()).Id;
                int numMax = context.Users.Max(u => (int?)u.Id) ?? 0;
                context.Users.Add(new User
                {
                    Id = numMax+1,
                    Birthdate = DateOnly.Parse(inputBirthday),
                    Email = inputEmail,
                    Lastname = inputLastName,
                    Firstname = inputFirstName,
                    Roleid = 2,
                    Officeid = officeId,
                    Active = true,
                    Password = inputPassword
                });
                context.SaveChanges();
                //this.Close();
            }catch
            {
                ShowErrorDialog("Ошибка", "При сохранении произошла ошибка, попробуйте снова!");
            }
        }
    }
}