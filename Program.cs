using Airport.Models;
using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Airport;

class Program
{
    private static bool _isCrashing = false;
    public static User23Context? _context;
    public static int userId;
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    { 

        _context = new User23Context();
        _context.Database.EnsureCreated();

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);

             LogExit(wasCrash: false);
        } catch(Exception ex)
        {
            LogExit(wasCrash: true, error: ex.ToString());
            throw;
        }
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        _isCrashing = true;
        var exception = (Exception)e.ExceptionObject;
        LogExit(wasCrash: true, error: exception.ToString());
    }

    private static void LogExit(bool wasCrash, string? error = null)
    {
        try
        {
            int numMax = _context.UserInfos.Max(u => (int?)u.Id) ?? 0;

            _context.UserInfos.Add(new UserInfo
            {
                UserId = userId,
                Exit = DateTime.Now,
                Error = wasCrash ? error : null 
            });

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
