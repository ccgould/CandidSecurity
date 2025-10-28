using System.Windows.Input;

namespace CandidBritishAirways.Controls;

public partial class AccordionSection : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(AccordionSection), default(string));

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(AccordionSection), true);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(AccordionSection), default(View));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand ToggleCommand { get; }

    public AccordionSection()
    {
        InitializeComponent();
        ToggleCommand = new Command(() => IsExpanded = !IsExpanded);
        BindingContext = this;
    }
}