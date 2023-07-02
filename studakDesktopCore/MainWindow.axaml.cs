using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using studakDesktopCore.Pages;

namespace studakDesktopCore;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent = this.FindControl<ContentControl>("MainContent");
        Navigation.Initialize(MainContent);
        MainContent.Content = new AuthPage();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}