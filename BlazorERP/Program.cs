using BlazorERP.Data.Context;
using BlazorERP.Modules.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using BlazorERP.Components;
using BlazorERP.Services;

namespace BlazorERP;

public class Program
{
    public static IConfiguration Configuration;
    
    private const string LogPre = "|[Startup]|>  ";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddUserSecrets<Program>();
        builder.Configuration.AddEnvironmentVariables();
        Configuration = builder.Configuration;
        
        // Logging - Serilog
        
        // ======= Initialize Base Services ========
        Console.WriteLine($"{LogPre}Initializing Base Services...");
        
        // Add MudBlazor
        builder.Services.AddMudServices(options =>
        {
            options.PopoverOptions.ThrowOnDuplicateProvider = false;
        });
        
        // .NET and other critical services
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        

        // Factory for contexts
        builder.Services.AddDbContextFactory<proContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<proContext>>().CreateDbContext());
        
        // ===== Add DI Services =====
        Console.WriteLine($"{LogPre}Initializing Dependency Injection Services...");
        // - Database
        
        // - Scoped
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<ImsService>();
        // - Singleton
        
        // - Transient
        
        
        
        
        
        
        // ======== Build App =========
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}