using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using MockPOS.Main.Local.ViewModels;
using MockPOS.Main.Themes.Views;
using Microsoft.Extensions.Configuration;

namespace MockPOS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // View, ViewModels
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var configuration = AppHost.Services.GetRequiredService<IConfiguration>();
            var viewModel = AppHost.Services.GetRequiredService<MainWindowViewModel>();
            viewModel.Initialize(configuration);

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = viewModel;
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }

}
