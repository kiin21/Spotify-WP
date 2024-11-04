// App.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Contracts.Services;
using Spotify.Services;
using Spotify.Contracts.DAO;
using Spotify.DAO;
using DotNetEnv;
using Spotify.Views;
using System.Diagnostics;
using System.IO;
using Spotify.Views;
using Spotify.DAOs;

namespace Spotify
{
    public partial class App : Application
    {
        //public ShellWindow ShellWindow { get; private set; }
        //public IServiceProvider Services { get; }

        //public App()
        //{
        //    //Get the directory of the current file
        //    string currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //    // Construct the path to the .env file
        //    string envFilePath = Path.Combine(currentDirectory, ".env");
        //    // I only want to get the file .env inside the Spotify same position with App or .gitignore
        //    Debug.WriteLine($"Loading .env file from: {envFilePath}");

        //    if (File.Exists(envFilePath))
        //    {
        //        DotNetEnv.Env.Load(envFilePath);
        //        string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");

        //        if (string.IsNullOrEmpty(connectionString))
        //        {
        //            Debug.WriteLine("MONGODB_CONNECTION_STRING is not set or is empty.");
        //        }
        //        else
        //        {
        //            Debug.WriteLine($"MONGODB_CONNECTION_STRING: {connectionString}");
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine($".env file not found at: {envFilePath}");
        //    }

        //    Services = ConfigureServices();
        //    this.InitializeComponent();
        //}

        //private static IServiceProvider ConfigureServices()
        //{
        //    var services = new ServiceCollection();
        //    // Register DAOs
        //    services.AddSingleton<ISongDAO, MockSongDAO>();
        //    services.AddSingleton<IPlaylistDAO, MockPlaylistDAO>();
        //    services.AddSingleton<ILikedSongDAO, MockLikedSongDAO>();

        //    // Register Services
        //    services.AddSingleton<SongService>();
        //    services.AddSingleton<PlaylistService>();
        //    services.AddSingleton<LikedSongService>();

        //    services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();  // Add this line


        //    // Register  Services
        //    services.AddSingleton<SongService>();
        //    services.AddSingleton<PlaybackControlService>();

        //    // Register ViewModels
        //    services.AddTransient<HeaderViewModel>();
        //    services.AddTransient<MainPanelViewModel>();

        //    return services.BuildServiceProvider();
        //}

        //protected override void OnLaunched(LaunchActivatedEventArgs args)
        //{
        //    ShellWindow = new ShellWindow();
        //    ShellWindow.Activate();
        //}

        private ShellWindow _shellWindow;
        private readonly IServiceProvider _services;

        public IServiceProvider Services => _services;
        public ShellWindow ShellWindow => _shellWindow;

        public static new App Current => Application.Current as App;

        public App()
        {
            InitializeComponent();
            LoadEnvironmentVariables();
            _services = ConfigureServices();
        }

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


        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register DAOs
            services.AddSingleton<ISongDAO, MockSongDAO>();
            services.AddSingleton<IPlaylistDAO, MockPlaylistDAO>();
            services.AddSingleton<ILikedSongDAO, MockLikedSongDAO>();
            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();

            // Register Services
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaylistService>();
            services.AddSingleton<LikedSongService>();
            services.AddSingleton<PlaybackControlService>();

            // Register ViewModels
            services.AddTransient<HeaderViewModel>();
            services.AddTransient<MainPanelViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                // Create ShellWindow first
                _shellWindow = new ShellWindow();
                _shellWindow.Activate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnLaunched: {ex.Message}");
                throw;
            }
        }
    }
}
