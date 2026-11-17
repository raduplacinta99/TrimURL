using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrimUrlApi.Database;

namespace TrimUrlApi.HealthChecks
{
    public class DatabaseHealthCheck(MainDbContext dbContext) : IHealthCheck
    {
        private readonly MainDbContext _dbContext = dbContext;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.Database.CanConnectAsync(cancellationToken)
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Unhealthy("Cannot connect to the database.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database check failed.", ex);
            }
        }
    }
}
