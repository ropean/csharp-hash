using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CSharpHash;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly string LogFilePath = Path.Combine(Path.GetTempPath(), "CSharpHash.log");

    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Log($"UnhandledException: {e.ExceptionObject}");
            SafeShowError("A fatal error occurred. See log at %TEMP%/CSharpHash.log.");
        };
        DispatcherUnhandledException += (_, e) =>
        {
            Log($"DispatcherUnhandledException: {e.Exception}");
            e.Handled = true;
            SafeShowError(e.Exception.Message);
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Log($"UnobservedTaskException: {e.Exception}");
            e.SetObserved();
        };
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Log("App starting");
        base.OnStartup(e);
    }

    public static void Log(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch { }
    }

    private static void SafeShowError(string message)
    {
        try { MessageBox.Show(message, "C# Hash256", MessageBoxButton.OK, MessageBoxImage.Error); }
        catch { }
    }
}

