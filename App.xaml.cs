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
        public ShellWindow ShellWindow { get; private set; }
        public IServiceProvider Services { get; }

        public App()
        {
            // Get the base directory of the application
            string baseDirectory = AppContext.BaseDirectory;
            
            DotNetEnv.Env.Load($"{baseDirectory}/.env");

            Services = ConfigureServices();
            this.InitializeComponent();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            // Register DAOs
            services.AddSingleton<ISongDAO, MockSongDAO>();
            services.AddSingleton<IPlaylistDAO, PlaylistDAO>();
            services.AddSingleton<IPlaylistSongDAO, PlaylistSongDAO>();

            // Register Services
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaylistService>();
            // Register PlaybackControl services
            //services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();
            services.AddSingleton<IPlaybackControlService, PlaybackControlService>();

            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();  
            services.AddSingleton<PlaylistSongService>();

            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();  



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
