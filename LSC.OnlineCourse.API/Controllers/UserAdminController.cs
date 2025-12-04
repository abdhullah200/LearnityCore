using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides API endpoints for managing user-related administrative operations.
    /// </summary>
    /// <remarks>This controller is secured with authorization and requires the caller to have the appropriate
    /// permissions. All endpoints are prefixed with "api/UserAdmin" and require a valid access token with the necessary
    /// scopes.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAdminController : ControllerBase
    {
        private readonly ICourseService courseService;

        public UserAdminController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <remarks>This method requires the caller to have the appropriate scope specified in the 
        /// "AzureAdB2C:Scopes:Read" configuration key. The method returns an HTTP 200 OK response  with the list of
        /// users if successful.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing a list of <see cref="UserModel"/> objects  representing all
        /// users.</returns>
        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<List<UserModel>>> GetAllUsers()
        {
            var courses = await courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

    }

}
