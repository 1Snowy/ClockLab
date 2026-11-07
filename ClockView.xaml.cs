using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace ClockLab.Controls;

public partial class ClockView : ContentView
{
    private TimeZoneInfo _zone = TimeZoneInfo.Local;
    private bool _isAnimationRunning = false;

    public static readonly BindableProperty TimeZoneIdProperty =
        BindableProperty.Create(nameof(TimeZoneId), typeof(string), typeof(ClockView), defaultValue: null, propertyChanged: OnTimeZoneIdChanged);

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

        StartSmoothClock();
    }

    private void StartSmoothClock()
    {
        if (_isAnimationRunning) return;
        _isAnimationRunning = true;

        var clockAnimation = new Animation(_ => UpdateHandsSmoothly());

        clockAnimation.Commit(this, "SmoothClockLoop", length: 1000, repeat: () => true);
    }

    private void UpdateHandsSmoothly()
    {
        if (SecondHand == null || MinuteHand == null || HourHand == null) return;
        DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _zone);
        double totalSeconds = now.Second + (now.Millisecond / 1000.0);

        double totalMinutes = now.Minute + (totalSeconds / 60.0);

        double totalHours = (now.Hour % 12) + (totalMinutes / 60.0);

        SecondHand.Rotation = totalSeconds * 6;     
        MinuteHand.Rotation = totalMinutes * 6;     
        HourHand.Rotation = totalHours * 30;         
    }

    private static void OnTimeZoneIdChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var clock = (ClockView)bindable;
        try
        {
            clock._zone = newValue is string id && !string.IsNullOrWhiteSpace(id) ? TimeZoneInfo.FindSystemTimeZoneById(id) : TimeZoneInfo.Local;
        }
        catch (TimeZoneNotFoundException)
        {
            clock._zone = TimeZoneInfo.Local;
        }

        clock.UpdateHandsSmoothly();
    }
}
