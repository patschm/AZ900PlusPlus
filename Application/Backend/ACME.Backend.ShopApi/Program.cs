using ACME.Backend.ShopApi.Extensions;
using ACME.DataLayer.Interfaces;
using ACME.DataLayer.Repository.SqlServer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ACME.Backend.ShopApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IConfiguration config = builder.Configuration;
        Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

        var connectionString = config.GetConnectionString("SqlServerExpress");
        if (builder.Environment.IsKubernetes())
            connectionString = config.GetConnectionString("SqlServer");
        Console.WriteLine(connectionString);
        builder.Services.AddDbContext<ShopDatabaseContext>(opts => {
            opts.UseSqlServer(connectionString);
        });
 
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddControllers().AddNewtonsoftJson(su => {
           // su.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}