using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LSC.OnlineCourse.API.Common
{
    /// <summary>
    /// Provides a custom response writer for health check results in JSON format.
    /// </summary>
    public static class HealthCheckResponseWriter
    {
        /// <summary>
        /// Writes a JSON-formatted health report to the HTTP response.
        /// </summary>
        /// <remarks>The method sets the response's content type to "application/json" and writes a JSON
        /// object containing the overall health status and detailed results for each health check entry. Each entry
        /// includes its status, description, exception message (if any), and duration.</remarks>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response.</param>
        /// <param name="report">The <see cref="HealthReport"/> containing the health status and details to be written to the response.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation of writing the JSON response.</returns>
        public static async Task WriteJsonResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var json = new
            {
                status = report.Status.ToString(),
                results = report.Entries.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        status = kvp.Value.Status.ToString(),
                        description = kvp.Value.Description,
                        exception = kvp.Value.Exception?.Message,
                        duration = kvp.Value.Duration.ToString()
                    })
            };
            await context.Response.WriteAsJsonAsync(json);
        }
    }


}
