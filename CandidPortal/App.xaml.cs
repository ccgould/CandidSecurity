﻿using CandidPortal.Configuration;
using CandidPortal.Repositories;
using CandidPortal.Services;
using CandidPortal.ViewModels.Pages;
using CandidPortal.ViewModels.Windows;
using CandidPortal.Views.Pages;
using CandidPortal.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;
namespace CandidPortal;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static System.Configuration.Configuration AppConfig { get; set; } = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)); })
        .ConfigureServices((context, services) =>
        {
            services.AddNavigationViewPageProvider();

            services.AddHostedService<ApplicationHostService>();

            // Theme manipulation
            services.AddSingleton<IThemeService, ThemeService>();

            // TaskBar manipulation
            services.AddSingleton<ITaskBarService, TaskBarService>();

            // Service containing navigation, same as INavigationWindow... but without window
            services.AddSingleton<INavigationService, NavigationService>();

            // Main window with navigation
            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<DataPage>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<EmployeesPage>();
            services.AddSingleton<EmployeesPageViewModel>();
            services.AddSingleton<EmployeeRepository>();
            services.AddSingleton<TrainingRecordRepository>();
            services.AddSingleton<TrainingTypeRepository>();
        }).Build();

    /// <summary>
    /// Gets services.
    /// </summary>
    public static IServiceProvider Services
    {
        get { return _host.Services; }
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        if (AppConfig.Sections["AppSettings"] is null)
        {
            AppConfig.Sections.Add("AppSettings", new AppSettings());
        }
        await _host.StartAsync();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
