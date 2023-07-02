using Avalonia.Controls;

namespace studakDesktopCore;

public class Navigation
{
    private static ContentControl MainContent;

    public static void Initialize(ContentControl control)
    {
        MainContent = control;
    }

    public static void NavigateTo(UserControl page)
    {
        MainContent.Content = page;
    }
}