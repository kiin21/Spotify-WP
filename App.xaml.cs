// App.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Services;
using Spotify.Contracts.DAO;
using System.Diagnostics;
using System.IO;
using Spotify.DAOs;
using Spotify.Models.DTOs;

namespace Spotify;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets or sets the current user.
    /// </summary>
    public UserDTO CurrentUser { get; set; } = new UserDTO();

    internal ShellWindow _shellWindow;
    internal LoginSignupWindow _loginSignupWindow;
    private readonly IServiceProvider _services;

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public IServiceProvider Services => _services;

    /// <summary>
    /// Gets the shell window.
    /// </summary>
    public ShellWindow ShellWindow => _shellWindow;

    /// <summary>
    /// Gets the login/signup window.
    /// </summary>
    public LoginSignupWindow LoginSignupWindow => _loginSignupWindow;

    /// <summary>
    /// Gets the current application instance.
    /// </summary>
    public static new App Current => Application.Current as App;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>

    // Premium
    private bool _isPremium;
    public bool IsPremium
    {
        get => _isPremium;
        set
        {
            if (_isPremium != value)
            {
                _isPremium = value;
                OnPremiumStatusChanged();
            }
        }
    }

    public event Action<bool> PremiumStatusChanged;

    private void OnPremiumStatusChanged()
    {
        PremiumStatusChanged?.Invoke(_isPremium);
    }
    public App()
    {
        InitializeComponent();
        LoadEnvironmentVariables();
        _services = ConfigureServices();
    }


    /// <summary>
    /// Loads environment variables from the .env file.
    /// </summary>
    private void LoadEnvironmentVariables()
    {
        string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string envFilePath = Path.Combine(currentDirectory, ".env");
        Debug.WriteLine($"Loading .env file from: {envFilePath}");

        if (File.Exists(envFilePath))
        {
            DotNetEnv.Env.Load(envFilePath);
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("MONGODB_CONNECTION_STRING is not set or is empty.");
            }
            else
            {
                Debug.WriteLine($"MONGODB_CONNECTION_STRING: {connectionString}");
            }
        }
        else
        {
            Debug.WriteLine($".env file not found at: {envFilePath}");
        }
    }

    /// <summary>
    /// Configures the services for dependency injection.
    /// </summary>
    /// <returns>The service provider.</returns>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register DAOs
        services.AddSingleton<IAdsDAO, AdsDAO>();
        services.AddSingleton<ISongDAO, SongDAO>();
        services.AddSingleton<IQueueDAO, QueueDAO>();
        services.AddSingleton<IPlaylistDAO, PlaylistDAO>();
        services.AddSingleton<IPlaylistSongDAO, PlaylistSongDAO>();
        services.AddSingleton<IPlaylistSongDetailDAO, PlaylistSongDetailDAO>();
        services.AddSingleton<IArtistDAO, ArtistDAO>();
        services.AddSingleton<IPlayHistoryDAO, PlayHistoryDAO>();
        services.AddSingleton<ICommentDAO, CommentDAO>();
        services.AddScoped<IUserDAO, UserDAO>();

        // Register Services
        services.AddSingleton<Spotify.Services.WindowManager>();
        services.AddSingleton<SongService>();
        services.AddSingleton<PlaylistService>();
        services.AddSingleton<LeftSidebarPageViewModel>();
        services.AddSingleton<PlaylistSongDetailService>();
        services.AddSingleton<UserService>();
        services.AddSingleton<SongService>();
        services.AddSingleton<LocalStorageService>();
        services.AddSingleton<PlayHistoryService>();
        services.AddSingleton<AdsService>();
        services.AddSingleton<PlaylistSongService>();
        services.AddSingleton<PlaylistSongDetailService>();
        services.AddSingleton<QueueService>();
        services.AddSingleton<ArtistService>();
        services.AddSingleton<PlaylistSongDetailService>();
        services.AddSingleton<ArtistService>();
        services.AddSingleton<CommentService>();

        // Register ShellWindow
        services.AddSingleton<ShellWindow>();

        // Register ViewModels
        services.AddSingleton<PlaybackControlViewModel>();
        services.AddTransient<HeaderViewModel>();
        services.AddTransient<ArtistViewModel>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _loginSignupWindow = new LoginSignupWindow();
            _loginSignupWindow.Activate();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnLaunched: {ex.Message}");
            throw;
        }
    }
}
