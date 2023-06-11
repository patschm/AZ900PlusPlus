namespace ACME.Frontend.Web.Mvc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var url = builder.Configuration.GetSection("DataService:Url").Get<string>();

        builder.Services.AddHttpClient("data", opts => { 
            opts.BaseAddress = new Uri(url);
        });
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Product/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Product}/{action=Productgroups}/{id?}");

        app.Run();
    }
}