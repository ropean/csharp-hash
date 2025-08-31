using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CSharpHash.ViewModels;
using System.Windows.Threading;

namespace CSharpHash;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _titleTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        _titleTimer.Tick += (_, _) =>
        {
            if (DataContext is MainViewModel vm)
            {
                Title = vm.IsHashing ? $"C# Hash256 — {vm.ProgressPercent}%" : "C# Hash256";
            }
        };
        _titleTimer.Start();
    }

    private async void PathInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is MainViewModel vm && File.Exists(vm.PathInput))
        {
            await vm.StartHashAsync();
        }
    }

    private void Window_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
        {
            return;
        }

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
            {
                vm.PathInput = files[0];
                await vm.StartHashAsync();
            }
        }
    }
}