using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using PayPro.Main.Local.ViewModels;
using PayPro.Main.Themes.Views;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp;
using PayPro.Contracts.Interfaces;
using PayPro.Services;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace PayPro
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
            .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/payproapp-.log", rollingInterval: RollingInterval.Day))
            .ConfigureServices((hostContext, services) =>
            {
                // View, ViewModels
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                // Services
                services.AddHttpClient<IPaymentService, PaymentService>((serviceProvider, client) =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
                    client.BaseAddress = new Uri(baseUrl);
                });
                services.AddHostedService<POSCommService>();
            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startup = AppHost.Services.GetRequiredService<MainWindow>();
            startup.Show();

            base.OnStartup(e);

            LiveCharts.Configure(config=>
                config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('헬')));
            
            Log.Information("Application started");
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            Log.Information("Application stopping");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
