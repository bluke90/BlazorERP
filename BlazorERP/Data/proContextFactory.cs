using System;
using BlazorERP.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BlazorERP.Data;

/// <summary>
/// Factory for creating <see cref="proContext"/> instances.
/// Implements both <see cref="IDbContextFactory{TContext}"/> for
/// runtime usage and <see cref="IDesignTimeDbContextFactory{TContext}"/>
/// for design-time operations such as migrations.
/// </summary>
public class proContextFactory : IDbContextFactory<proContext>, IDesignTimeDbContextFactory<proContext>
{
    public proContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<proContext>();
        var configuration = BuildConfiguration();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        return new proContext(optionsBuilder.Options);
    }

    proContext IDesignTimeDbContextFactory<proContext>.CreateDbContext(string[] args)
    {
        return CreateDbContext();
    }

    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
}