namespace MoviesApp
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Services;
    using Services.Interfaces;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MoviesAppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IMoviesService, MoviesService>();
            builder.Services.AddScoped<IWatchlistService, WatchlistService>();
            builder.Services.AddScoped<IImportService, ImportService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movies}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
