using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nvisia.Profile.Service.WriteStore.Context;

namespace Nvisia.Profile.Service.WriteStore;

public static class StartupExtensions
{
    public static IServiceCollection AddWriteStoreContext(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlBuilder = new SqlConnectionStringBuilder
        {
            DataSource = configuration["DataSource:Server"],
            InitialCatalog = configuration["DataSource:Database"],
            UserID = configuration["DataSource:User"],
            Password = configuration["DataSource:Password"],
            TrustServerCertificate = true,
        };
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(sqlBuilder.ConnectionString));
        return services;
    }
}