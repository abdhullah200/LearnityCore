using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LSC.OnlineCourse.API.Common
{
    public class PrivateMemoryHealthCheck : IHealthCheck
    {
        /// <summary>
        /// Represents the maximum memory threshold, in bytes, used to determine when memory usage exceeds a predefined
        /// limit.
        /// </summary>
        /// <remarks>This field is read-only and is intended to store a constant or configuration value
        /// that defines the upper limit for memory usage. It is typically used in scenarios where memory management or
        /// monitoring is required.</remarks>
        private readonly long _maxMemoryThreshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateMemoryHealthCheck"/> class with the specified memory
        /// threshold.
        /// </summary>
        /// <remarks>This health check monitors the private memory usage of the application and compares
        /// it against the specified threshold. If the private memory usage exceeds the threshold, the health check may
        /// indicate an unhealthy state.</remarks>
        /// <param name="maxMemoryThreshold">The maximum private memory threshold, in bytes, that the health check will use to determine if the system is
        /// healthy. Must be a positive value.</param>
        public PrivateMemoryHealthCheck(long maxMemoryThreshold)
        {
            _maxMemoryThreshold = maxMemoryThreshold;
        }

        /// <summary>
        /// Performs a health check to evaluate the application's memory usage.
        /// </summary>
        /// <remarks>This health check evaluates the current memory usage of the application against a
        /// predefined threshold. If the memory usage is below the threshold, the result is <see
        /// cref="HealthCheckResult.Healthy"/>; otherwise, the result is <see
        /// cref="HealthCheckResult.Unhealthy"/>.</remarks>
        /// <param name="context">The context in which the health check is being executed. This parameter provides additional information
        /// about the health check operation.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the health check operation. The default value is <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains a <see
        /// cref="HealthCheckResult"/> indicating whether the application's memory usage is within acceptable limits.</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            long memoryUsage = GC.GetTotalMemory(false);

            if (memoryUsage < _maxMemoryThreshold)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Memory usage is under control: {memoryUsage / 1024 / 1024} MB."));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high: {memoryUsage / 1024 / 1024} MB."));
        }
    }

}
