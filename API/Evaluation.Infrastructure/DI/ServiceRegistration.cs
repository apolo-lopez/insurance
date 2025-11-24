using Evaluation.Application.Interfaces;
using Evaluation.Infrastructure.Data;
using Evaluation.Infrastructure.Repositories;
using Evaluation.Infrastructure.UOF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evaluation.Infrastructure.DI
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register AppDbContext with PostgreSQL provider
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No DefaultConnection has been found");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            // Register repositories
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IPolicyRepository, PolicyRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
