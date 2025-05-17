using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Airport;

public partial class UserMainWindow : Window
{
    public UserMainWindow()
    {
        InitializeComponent();
    }

    private void exitClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}