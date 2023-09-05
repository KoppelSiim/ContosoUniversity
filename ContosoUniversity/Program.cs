using ContosoUniversity.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add configuration.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure services: register the SchoolContext as a service, specifying that it should use SQL Server as the database provider.
            builder.Services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Building the Application.
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Enable HTTPS redirection and serve static files like CSS, JavaScript, and image
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Enable routing and authorization middleware.
            // Routing is responsible for determining which controller and action should handle a given request,
            // while authorization ensures that users have the necessary permissions to access certain resources.
            app.UseRouting();
            app.UseAuthorization();

            // Map default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}