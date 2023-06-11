using ACME.DataLayer.Interfaces;
using ACME.DataLayer.Repository.SqlServer;
using ACME.Frontend.WPF.Core.Interfaces;
using ACME.Frontend.WPF.UserControls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace ACME.Frontend.WPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider?  ServiceProvider { get; private set; }
    public IConfiguration?  Configuration { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true);
        Configuration = builder.Build();

        var serviceCollection = new  ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
        
        var main = ServiceProvider.GetRequiredService<MainWindow>();
           
        main.Show();
    }

    private void ConfigureServices(ServiceCollection serviceCollection)
    {
        // TODO 1: Examine the Repositories in ACME.DataLayer.Repository.SqlServer
        // Pay special attention to the class UnitOfWork.

        var connectionString = Configuration.GetConnectionString("SqlServerExpress");
        // TODO 2: Register the ShopDatabaseContext in the dependency injector
        serviceCollection.AddDbContext<ShopDatabaseContext>(opts => {
            opts.UseSqlServer(connectionString);
        }); 
        // TODO 3: Register the UnitOfWork class in the dependency injector
        serviceCollection.AddTransient<IUnitOfWork, UnitOfWork>();
        
        serviceCollection.AddSingleton<IViewMediator, ViewMediator>();
        serviceCollection.AddTransient<MainViewModel>();
        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddViewModels();
        serviceCollection.AddViewComponents();
    }
}
