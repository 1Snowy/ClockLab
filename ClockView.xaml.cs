namespace ClockLab.Controls;

public partial class ClockView : ContentView
{
    private TimeZoneInfo _zone = TimeZoneInfo.Local;
    private readonly IDispatcherTimer _timer;

    public static readonly BindableProperty TimeZoneIdProperty =
        BindableProperty.Create(nameof(TimeZoneId), typeof(string), typeof(ClockView),
            defaultValue: null,
            propertyChanged: OnTimeZoneIdChanged);

    public string TimeZoneId
    {
        get => (string)GetValue(TimeZoneIdProperty);
        set => SetValue(TimeZoneIdProperty, value);
    }

    public ClockView()
    {
        InitializeComponent();

    
        for (int i = 0; i < 12; i++)
        {
            var tick = new Line
            {
                X1 = 100,
                Y1 = 8,
                X2 = 100,
                Y2 = i % 3 == 0 ? 22 : 16,
                Stroke = i % 3 == 0 ? Colors.Black : Colors.Gray,
                StrokeThickness = i % 3 == 0 ? 3 : 2
            };
            var holder = new Grid { Rotation = i * 30 };
            holder.Add(tick);
            Ticks.Add(holder);
        }

        
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => UpdateHands();
        _timer.Start();

        UpdateHands(); 
    }

    private static void OnTimeZoneIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var clock = (ClockView)bindable;
        try
        {
            clock._zone = newValue is string id && !string.IsNullOrWhiteSpace(id)
                ? TimeZoneInfo.FindSystemTimeZoneById(id)
                : TimeZoneInfo.Local;
        }
        catch (TimeZoneNotFoundException)
        {
            clock._zone = TimeZoneInfo.Local;
        }

    
        if (clock.SecondHand != null)
            clock.UpdateHands();
    }

    private void UpdateHands()
    {
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _zone);

    
        SecondHand.Rotation = now.Second * 6;
        MinuteHand.Rotation = now.Minute * 6 + now.Second * 0.1;
        HourHand.Rotation = (now.Hour % 12) * 30 + now.Minute * 0.5;
    }
}
