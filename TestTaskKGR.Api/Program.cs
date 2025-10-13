using Serilog;
using Microsoft.EntityFrameworkCore;
using TestTaskKGR.DAL;
using TestTaskKGR.ViewModel;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

try
{
    Log.Information("Application started");
    builder.Services
        .AddSingleton(Log.Logger)
        .AddScoped<MapsterConfig>()
        .AddScoped<IRepository<TypeViewModel>, TypeRepository>()
        .AddScoped<IRepository<RoleViewModel>, RoleRepository>()
        .AddScoped<IRepository<ViolationViewModel>, ViolationRepository>()
        .AddDbContext<ApplicationContext>(options => options.UseNpgsql("Host=localhost;Port=5432;Database=violationdb;Username=postgres;Password=12345",
            providerOptions => providerOptions.EnableRetryOnFailure()), ServiceLifetime.Transient)
        .AddControllers();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        dataContext.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllers();
    app.Run();

}
catch (Exception ex)
{  
    Log.Error(ex, "The Application failed to start.");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
