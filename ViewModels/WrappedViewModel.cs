using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Defaults;
using Microsoft.Extensions.DependencyInjection;
using Spotify.Models.DTOs;
using Spotify.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Security.Principal;

namespace Spotify.ViewModels;

public class WrappedViewModel : INotifyPropertyChanged
{
    private List<PlayHistoryWithSongDTO> _playHistory;
    private ISeries[] _barSeries;
    private ISeries[] _pieSeries;

    // Updated colors for better visibility
    private readonly SKColor[] colors = new[]
    {
        new SKColor(29, 185, 84, 255),  // Spotify Green
        new SKColor(255, 99, 71, 255),  // Tomato Red
        new SKColor(30, 144, 255, 255)  // Dodger Blue
    };

    public string BarChartTitle { get; set; } = "Total Songs Played by Time of Day";
    public string PieChartTitle { get; set; } = "Distribution of Songs by Time of Day";

    public ISeries[] BarSeries
    {
        get => _barSeries;
        set
        {
            _barSeries = value;
            OnPropertyChanged();
        }
    }

    public ISeries[] PieSeries
    {
        get => _pieSeries;
        set
        {
            _pieSeries = value;
            OnPropertyChanged();
        }
    }

    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public WrappedViewModel()
    {
        // Updated axis styling
        XAxes = new Axis[]
        {
            new Axis
            {
                Name = "Time of Day",
                Labels = new[] { "Morning", "Afternoon", "Evening" },
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.Black),
                TextSize = 16,
                SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 }
            }
        };

        YAxes = new Axis[]
        {
            new Axis
            {
                Name = "Number of Songs",
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.Black),
                TextSize = 16,
                MinStep = 1,
                ShowSeparatorLines = true,
                SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 }
            }
        };
    }

    private void UpdateBarSeries(int[] values)
    {
        BarSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values,
                Fill = new SolidColorPaint(colors[0]),  // Using Spotify Green
                Name = "Songs Played",
                Stroke = null,
                MaxBarWidth = 50,
                Padding = 20,
                DataLabelsSize = 16,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Model}",
                DataLabelsPaint = new SolidColorPaint(SKColors.Black)
            }
        };
    }

    private void UpdatePieSeries(Dictionary<string, int> timeSlots, int totalPlay)
    {
        var values = timeSlots.Select((kvp, index) => new PieSeries<int>
        {
            Values = new[] { kvp.Value },
            Name = kvp.Key,
            Fill = new SolidColorPaint(colors[index]),
            Stroke = null,
            DataLabelsSize = 16,
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsFormatter = point => $"{((double)kvp.Value / totalPlay * 100):F2}%"
        });

        PieSeries = values.ToArray();
    }

    public void UpdateData(List<PlayHistoryWithSongDTO> playHistory)
    {
        _playHistory = playHistory;
        var timeSlots = new Dictionary<string, int>
        {
            { "Morning", 0 },
            { "Afternoon", 0 },
            { "Evening", 0 }
        };

        foreach (var play in _playHistory ?? Enumerable.Empty<PlayHistoryWithSongDTO>())
        {
            var timeOfDay = GetTimeOfDay(play.PlayedAt);
            timeSlots[timeOfDay]++;
        }

        var values = new[]
        {
            timeSlots["Morning"],
            timeSlots["Afternoon"],
            timeSlots["Evening"]
        };

        UpdateBarSeries(values);
        UpdatePieSeries(timeSlots, _playHistory.Count());
    }

    private string GetTimeOfDay(DateTime time)
    {
        var hour = time.ToLocalTime().Hour;
        if (hour >= 5 && hour < 12)
            return "Morning";
        if (hour >= 12 && hour < 17)
            return "Afternoon";
        return "Evening";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}