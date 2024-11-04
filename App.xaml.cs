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
using Spotify.DAOs;

namespace Spotify
{
    public partial class App : Application
    { 
        private ShellWindow _shellWindow;
        private LoginSignupWindow _loginSignupWindow;
        private readonly IServiceProvider _services;

        public IServiceProvider Services => _services;
        public ShellWindow ShellWindow => _shellWindow;
        public LoginSignupWindow LoginSignupWindow => _loginSignupWindow;
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
            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();
            services.AddSingleton<IPlaylistDAO, PlaylistDAO>();
            services.AddSingleton<IPlaylistSongDAO, PlaylistSongDAO>();

            // Register Services
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaylistService>();
            services.AddSingleton<PlaylistSongService>();
            services.AddSingleton<PlaybackControlService>();
            services.AddSingleton<LocalStorageService>();

            // Register ViewModels
            services.AddTransient<HeaderViewModel>();
            services.AddTransient<MainPanelViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                //// Create ShellWindow first
                //_shellWindow = new ShellWindow();
                //_shellWindow.Activate();

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
}
