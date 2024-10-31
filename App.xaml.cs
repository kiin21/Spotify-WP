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
        public ShellWindow ShellWindow { get; private set; }
        public IServiceProvider Services { get; }

        public App()
        {
            // Get the base directory of the application
            string baseDirectory = AppContext.BaseDirectory;
            
            DotNetEnv.Env.Load($"{baseDirectory}/.env");
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");


            Services = ConfigureServices();
            this.InitializeComponent();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            // Register DAOs
            services.AddSingleton<ISongDAO, MockSongDAO>();
            services.AddSingleton<IPlaylistDAO, PlaylistDAO>();
            services.AddSingleton<IPlaylistSongDAO, PlaylistSongDAO>();

            // Register Services
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaylistService>();
            services.AddSingleton<PlaylistSongService>();
            services.AddSingleton<PlaybackControlService>();

            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();  // Add this line




            // Register ViewModels
            services.AddTransient<HeaderViewModel>();
            services.AddTransient<MainPanelViewModel>();

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ShellWindow = new ShellWindow();
            ShellWindow.Activate();
        }

    }
}
