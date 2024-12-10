// App.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Contracts.Services;
using Spotify.Services;
using Spotify.Contracts.DAO;
using DotNetEnv;
using Spotify.Views;
using System.Diagnostics;
using System.IO;
using Spotify.DAOs;
using Spotify.Models.DTOs;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Spotify
{
    public partial class App : Application
    {
        public  UserDTO CurrentUser { get; set; } = new UserDTO();
        internal ShellWindow _shellWindow;
        internal LoginSignupWindow _loginSignupWindow;
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
            services.AddSingleton<IAdsDAO, AdsDAO>();
            services.AddSingleton<ISongDAO, SongDAO>();
            services.AddSingleton<IQueueDAO, QueueDAO>();
            services.AddSingleton<IPlaylistDAO, PlaylistDAO>();
            services.AddSingleton<IPlaylistSongDAO, PlaylistSongDAO>();
            services.AddSingleton<IPlaylistSongDetailDAO, PlaylistSongDetailDAO>();
            services.AddSingleton<IArtistDAO, ArtistDAO>();
            services.AddSingleton<IPlayHistoryDAO, PlayHistoryDAO>();
            
            // Why AddScoped???????????
            services.AddScoped<IUserDAO, UserDAO>();

            // Register Services
            services.AddSingleton<Spotify.Services.WindowManager>();
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaylistService>();
            services.AddSingleton<LeftSidebarPageViewModel>();
            services.AddSingleton<PlaylistSongDetailService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<LocalStorageService>();
            services.AddSingleton<PlayHistoryService>();
            services.AddSingleton<AdsService>();
            services.AddSingleton<PlaylistSongService>();
            services.AddSingleton<PlaylistSongDetailService>();
            services.AddSingleton<QueueService>();
            services.AddSingleton<ArtistService>();
            services.AddSingleton<PlaylistSongDetailService>();
            services.AddSingleton<ArtistService>();


            // FIX_LATTER
            services.AddSingleton<ShellWindow>();




            // Register ViewModels
            services.AddSingleton<PlaybackControlViewModel>();

            services.AddTransient<HeaderViewModel>();
            services.AddTransient<ArtistViewModel>();
            //services.AddTransient<MainPanelViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            try
            {
                ////// Create ShellWindow first
                //_shellWindow = new ShellWindow();
                //_shellWindow.Activate();

                // Sử dụng WindowManager để chuyển đổi sang ShellWindow
                //Spotify.Services.WindowManager.Instance.SwitchToShellWindow();

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
