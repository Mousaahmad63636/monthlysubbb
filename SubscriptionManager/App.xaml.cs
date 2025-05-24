using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubscriptionManager.Data;
using SubscriptionManager.Services;
using SubscriptionManager.ViewModels;
using System.Windows;

namespace SubscriptionManager
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SubscriptionManager;Trusted_Connection=True;MultipleActiveResultSets=true"));

            // Services
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<ISettingsService, SettingsService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<SubscriptionViewModel>();
            services.AddTransient<ExpenseViewModel>();
            services.AddTransient<SettingsViewModel>();

            // Main Window
            services.AddSingleton<MainWindow>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                await _host.StartAsync();

 
                using (var scope = _host.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await context.Database.EnsureCreatedAsync();
                }

         
                var mainWindow = _host.Services.GetRequiredService<MainWindow>();


                if (mainWindow.DataContext is MainViewModel mainViewModel)
                {
                    await mainViewModel.InitializeAsync();
                }

                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application startup failed: {ex.Message}", "Startup Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}