// App.xaml.cs
using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Spotify.ViewModels;
using Spotify.Contracts.Services;
using Spotify.Services;
using Spotify.Contracts.DAO;
using Spotify.DAO;
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
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register DAOs
            services.AddSingleton<ISongDAO, MockSongDAO>();
            services.AddSingleton<IPlaybackControlDAO, MockPlaybackControlDAO>();  // Add this line


            // Register  Services
            services.AddSingleton<SongService>();
            services.AddSingleton<PlaybackControlService>();

            // Register ViewModels
            services.AddTransient<HeaderViewModel>();
            services.AddTransient<MainPanelViewModel>();
            services.AddTransient<PlaybackControlViewModel>();

            // Register Pages
        //    services.AddTransient<HeaderPage>();
        //    services.AddTransient<LeftSidebarPage>();
            //services.AddTransient<SearchResultsPage>();
            //services.AddTransient<QueuePage>();
            //services.AddTransient<PlaybackControlPage>();
     

            return services.BuildServiceProvider();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ShellWindow = new ShellWindow();
            ShellWindow.Activate();
        }

    }
}
