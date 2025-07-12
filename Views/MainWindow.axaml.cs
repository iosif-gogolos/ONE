using System;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ONE.Services;
using ONE.ViewModels;

namespace ONE.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        this.KeyDown += MainWindow_KeyDown;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _viewModel = DataContext as MainWindowViewModel;
    }

    private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
    {
        // Handle keyboard shortcuts
        if (e.KeyModifiers == KeyModifiers.Meta) // Cmd on Mac
        {
            switch (e.Key)
            {
                case Key.N:
                    _viewModel?.AddPageCommand.Execute().Subscribe();
                    e.Handled = true;
                    break;
                case Key.S:
                    // Auto-save is already implemented
                    e.Handled = true;
                    break;
            }
        }
    }
}