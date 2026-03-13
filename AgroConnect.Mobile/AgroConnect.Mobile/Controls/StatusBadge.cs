using AgroConnect.Mobile.Helpers;

namespace AgroConnect.Mobile.Controls;

public class StatusBadge : Border
{
    public static readonly BindableProperty StatusProperty =
        BindableProperty.Create(nameof(Status), typeof(string), typeof(StatusBadge), propertyChanged: OnStatusChanged);

    public string? Status { get => (string?)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }

    private readonly Label _label;

    public StatusBadge()
    {
        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 };
        Padding = new Thickness(10, 4);
        StrokeThickness = 0;
        _label = new Label { TextColor = Colors.White, FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
        Content = _label;
    }

    private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StatusBadge badge && newValue is string status)
        {
            badge._label.Text = StatusHelper.GetStatusLabel(status);
            badge.BackgroundColor = StatusHelper.GetStatusColor(status);
        }
    }
}
