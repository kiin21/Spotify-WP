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
using LiveChartsCore.Kernel.Sketches;
using System.Globalization;

namespace Spotify.ViewModels;

public class WrappedViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// The play history data.
    /// </summary>
    private List<PlayHistoryWithSongDTO> _playHistory;

    /// <summary>
    /// The bar series data for the bar chart.
    /// </summary>
    private ISeries[] _barSeries;

    /// <summary>
    /// The pie series data for the pie chart.
    /// </summary>
    private ISeries[] _pieSeries;

    /// <summary>
    /// The genre pie series data for the genre pie chart.
    /// </summary>
    private ISeries[] _genrePieSeries;

    /// <summary>
    /// The line series data for the line chart.
    /// </summary>
    private ISeries[] _lineSeries;

    /// <summary>
    /// The total time spent listening to music.
    /// </summary>
    private TimeSpan _totalTimeSpent = TimeSpan.Zero;

    /// <summary>
    /// Gets the total time spent listening to music as a display string.
    /// </summary>
    public string TotalTimeSpentDisplay
    {
        get
        {
            int hours = (int)_totalTimeSpent.TotalHours;
            int minutes = _totalTimeSpent.Minutes;

            return $"{hours} hours {minutes} minutes";
        }
    }

    /// <summary>
    /// Gets or sets the total time spent listening to music.
    /// </summary>
    public TimeSpan TotalTimeSpent
    {
        get => _totalTimeSpent;
        set
        {
            if (_totalTimeSpent != value)
            {
                _totalTimeSpent = value;
                OnPropertyChanged(nameof(TotalTimeSpent));
                OnPropertyChanged(nameof(TotalTimeSpentDisplay));
            }
        }
    }

    /// <summary>
    /// The colors used for the charts.
    /// </summary>
    private readonly SKColor[] colors = new[]
    {
        new SKColor(29, 185, 84, 255),   // Spotify Green
        new SKColor(255, 99, 71, 255),   // Tomato Red
        new SKColor(30, 144, 255, 255),  // Dodger Blue
        new SKColor(255, 215, 0, 255),   // Gold
        new SKColor(138, 43, 226, 255),  // Blue Violet
        new SKColor(255, 140, 0, 255)    // Dark Orange
    };

    /// <summary>
    /// Gets or sets the title of the bar chart.
    /// </summary>
    public string BarChartTitle { get; set; } = "Total Songs Played by Time of Day";

    /// <summary>
    /// Gets or sets the title of the pie chart.
    /// </summary>
    public string PieChartTitle { get; set; } = "Distribution of Songs by Time of Day";

    /// <summary>
    /// Gets or sets the title of the genre pie chart.
    /// </summary>
    public string GenrePieChartTitle { get; set; } = "Distribution of Songs by Genre";

    /// <summary>
    /// Gets or sets the title of the line chart.
    /// </summary>
    public string LineChartTitle { get; set; } = "Songs Played Over Time";

    /// <summary>
    /// Gets or sets the line series data for the line chart.
    /// </summary>
    public ISeries[] LineSeries
    {
        get => _lineSeries;
        set
        {
            _lineSeries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the X axes for the line chart.
    /// </summary>
    public Axis[] LineXAxes { get; set; }

    /// <summary>
    /// Gets or sets the Y axes for the line chart.
    /// </summary>
    public Axis[] LineYAxes { get; set; }

    /// <summary>
    /// Gets or sets the bar series data for the bar chart.
    /// </summary>
    public ISeries[] BarSeries
    {
        get => _barSeries;
        set
        {
            _barSeries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the pie series data for the pie chart.
    /// </summary>
    public ISeries[] PieSeries
    {
        get => _pieSeries;
        set
        {
            _pieSeries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the genre pie series data for the genre pie chart.
    /// </summary>
    public ISeries[] GenrePieSeries
    {
        get => _genrePieSeries;
        set
        {
            _genrePieSeries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the X axes for the bar chart.
    /// </summary>
    public Axis[] XAxes { get; set; }

    /// <summary>
    /// Gets or sets the Y axes for the bar chart.
    /// </summary>
    public Axis[] YAxes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedViewModel"/> class.
    /// </summary>
    public WrappedViewModel()
    {
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

        LineXAxes = new Axis[]
        {
            new Axis
            {
                Name = "",
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.Black),
                TextSize = 16,
                SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 },
                LabelsRotation = 45
            }
        };

        LineYAxes = new Axis[]
        {
            new Axis
            {
                Name = "Songs Played",
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.Black),
                TextSize = 16,
                MinStep = 1,
                SeparatorsPaint = new SolidColorPaint(SKColors.DarkGray) { StrokeThickness = 1 }
            }
        };
    }

    /// <summary>
    /// Updates the line series data for the line chart.
    /// </summary>
    /// <param name="playHistory">The play history data.</param>
    private void UpdateLineSeries(List<PlayHistoryWithSongDTO> playHistory)
    {
        var groupedData = playHistory
            .GroupBy(p => p.PlayedAt.Date)
            .Select(g => new DateTimePoint(g.Key, g.Count()))
            .OrderBy(p => p.DateTime)
            .ToList();

        LineSeries = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = groupedData,
                Fill = null,
                GeometryFill = new SolidColorPaint(colors[0]),
                GeometryStroke = new SolidColorPaint(colors[0]) { StrokeThickness = 2 },
                Stroke = new SolidColorPaint(colors[0]) { StrokeThickness = 3 },
                Name = "Songs Played",
                GeometrySize = 10,
                LineSmoothness = 0,
            }
        };

        // Update X axis labels
        LineXAxes[0].Labeler = value => new DateTime((long)value).ToString("MM/dd");
    }

    /// <summary>
    /// Updates the bar series data for the bar chart.
    /// </summary>
    /// <param name="values">The values for the bar chart.</param>
    private void UpdateBarSeries(int[] values)
    {
        BarSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = values,
                Fill = new SolidColorPaint(colors[0]),
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

    /// <summary>
    /// Updates the pie series data for the pie chart.
    /// </summary>
    /// <param name="timeSlots">The time slots data.</param>
    /// <param name="totalPlay">The total number of plays.</param>
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

    /// <summary>
    /// Updates the genre pie series data for the genre pie chart.
    /// </summary>
    /// <param name="genreCount">The genre count data.</param>
    /// <param name="totalPlay">The total number of plays.</param>
    private void UpdateGenrePieSeries(Dictionary<string, int> genreCount, int totalPlay)
    {
        var values = genreCount.Select((kvp, index) => new PieSeries<int>
        {
            Values = new[] { kvp.Value },
            Name = kvp.Key,
            Fill = new SolidColorPaint(colors[index % colors.Length]), // Use modulo to cycle through colors
            Stroke = null,
            DataLabelsSize = 16,
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsFormatter = point => $"{((double)kvp.Value / totalPlay * 100):F2}%"
        });

        GenrePieSeries = values.ToArray();
    }

    /// <summary>
    /// Updates the data for the charts based on the play history.
    /// </summary>
    /// <param name="playHistory">The play history data.</param>
    public void UpdateData(List<PlayHistoryWithSongDTO> playHistory)
    {
        _playHistory = playHistory;
        var timeSlots = new Dictionary<string, int>
        {
            { "Morning", 0 },
            { "Afternoon", 0 },
            { "Evening", 0 }
        };
        var totalSongs = _playHistory.Count();

        Dictionary<string, int> genres = new Dictionary<string, int>();
        foreach (var play in playHistory)
        {
            _totalTimeSpent += play.TotalTime;
            var genreName = play.Genre.name;
            if (!genres.ContainsKey(genreName))
            {
                genres[genreName] = 0;
            }
            genres[genreName]++;
        }

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
        UpdatePieSeries(timeSlots, totalSongs);
        UpdateGenrePieSeries(genres, totalSongs);
        UpdateLineSeries(playHistory);
    }

    /// <summary>
    /// Gets the time of day based on the provided time.
    /// </summary>
    /// <param name="time">The time to evaluate.</param>
    /// <returns>A string representing the time of day ("Morning", "Afternoon", or "Evening").</returns>
    private string GetTimeOfDay(DateTime time)
    {
        var hour = time.ToLocalTime().Hour;
        if (hour >= 5 && hour < 12)
            return "Morning";
        if (hour >= 12 && hour < 17)
            return "Afternoon";
        return "Evening";
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}