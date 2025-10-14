
namespace CandidQVmMulti.Controls;

public partial class ResponsiveGrid : ContentView
{
    public ResponsiveGrid()
    {
        InitializeComponent();
    }

    public Microsoft.Maui.Controls.View Header
    {
        get => HeaderContent.Content;
        set => HeaderContent.Content = value;
    }

    public Microsoft.Maui.Controls.View Main
    {
        get => MainContent.Content;
        set => MainContent.Content = value;
    }

    public Microsoft.Maui.Controls.View Footer
    {
        get => FooterContent.Content;
        set => FooterContent.Content = value;
    }

    public Microsoft.Maui.Controls.View Sidebar
    {
        get => SidebarContent.Content;
        set => SidebarContent.Content = value;
    }
}