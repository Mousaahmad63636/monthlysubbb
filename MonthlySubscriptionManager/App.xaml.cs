// MonthlySubscriptionManager/App.xaml.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonthlySubscriptionManager.Application.Events;
using MonthlySubscriptionManager.Application.Services;
using MonthlySubscriptionManager.Application.Services.Interfaces;
using MonthlySubscriptionManager.Domain.Interfaces.Repositories;
using MonthlySubscriptionManager.Infrastructure.Data;
using MonthlySubscriptionManager.Infrastructure.Repositories;
using System;
using System.Windows;
using AutoMapper;
using QuickTechSystems.WPF.ViewModels;

namespace MonthlySubscriptionManager.WPF
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
            // Register database context
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Server=(localdb)\\MSSQLLocalDB;Database=MonthlySubscriptionManager;Trusted_Connection=True;"));

            // Register AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Register repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository<CustomerSubscription>, GenericRepository<CustomerSubscription>>();

            // Register services
            services.AddScoped<ICustomerSubscriptionService, CustomerSubscriptionService>();
            services.AddScoped<IMonthlySubscriptionSettingsService, MonthlySubscriptionSettingsService>();
            services.AddScoped<ISubscriptionTypeService, SubscriptionTypeService>();
            services.AddScoped<IExpenseService, ExpenseService>();

            // Register event aggregator (singleton)
            services.AddSingleton<IEventAggregator, EventAggregator>();

            // Register main window and viewmodels
            services.AddSingleton<MainWindow>();
            services.AddTransient<MonthlySubscriptionViewModel>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Create database and apply migrations
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            // Initialize currency helper
            MonthlySubscriptionManager.Application.Helpers.CurrencyHelper.UpdateExchangeRate(90000m);

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
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