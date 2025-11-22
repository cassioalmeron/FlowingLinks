using Microsoft.AspNetCore.Mvc;
using FlowingLinks.Core;

namespace FlowingLinks.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;
        private readonly FlowingLinksDbContext _dbContext;

        public HealthController(ILogger<HealthController> logger, FlowingLinksDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Basic health check endpoint
        /// Returns 200 OK if the service is running
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Detailed health check endpoint including database connectivity
        /// Returns 200 OK if the service and database are healthy
        /// Returns 503 Service Unavailable if the database is not accessible
        /// </summary>
        [HttpGet("detailed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetDetailed()
        {
            try
            {
                // Test database connectivity
                var canConnect = await _dbContext.Database.CanConnectAsync();

                if (!canConnect)
                {
                    _logger.LogWarning("Health check: Database connection failed");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                    {
                        status = "unhealthy",
                        database = "disconnected",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new
                {
                    status = "healthy",
                    database = "connected",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check error");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "unhealthy",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Liveness probe for Kubernetes and container orchestration
        /// Returns 200 OK if the service is alive
        /// </summary>
        [HttpGet("live")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetLive()
        {
            return Ok();
        }

        /// <summary>
        /// Readiness probe for Kubernetes and container orchestration
        /// Returns 200 OK if the service is ready to accept traffic
        /// Returns 503 Service Unavailable if not ready
        /// </summary>
        [HttpGet("ready")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetReady()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable);
                }

                return Ok();
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
