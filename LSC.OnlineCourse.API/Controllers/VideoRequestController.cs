using LSC.OnlineCourse.API.Common;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing video requests, including retrieving, creating, updating, and deleting video
    /// requests.
    /// </summary>
    /// <remarks>This controller is secured with authorization and requires specific scopes for accessing its
    /// endpoints.  It supports operations for both administrators and regular users, with role-based access to certain
    /// data.</remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VideoRequestController : ControllerBase
    {
        private readonly IVideoRequestService _videoRequestService;
        private readonly IUserClaims userClaims;

        public VideoRequestController(IVideoRequestService videoRequestService, IUserClaims userClaims)
        {
            _videoRequestService = videoRequestService;
            this.userClaims = userClaims;
        }

        /// <summary>
        /// Retrieves a collection of video requests based on the user's roles and permissions.
        /// </summary>
        /// <remarks>If the user has the "Admin" role, all video requests are returned. Otherwise, only
        /// the video requests associated with the current user's ID are retrieved.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing an <see cref="IEnumerable{T}"/> of <see
        /// cref="VideoRequestModel"/> objects. The collection will include all video requests for administrators, or
        /// only the user's video requests for non-administrators.</returns>
        [HttpGet]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetAll()
        {
            List<VideoRequestModel> videoRequests;
            var userRoles = userClaims.GetUserRoles();
            if (userRoles.Contains("Admin"))
            {
                videoRequests = await _videoRequestService.GetAllAsync();
            }
            else
            {
                var videoRequest = await _videoRequestService.GetByUserIdAsync(userClaims.GetUserId());
                videoRequests = videoRequest.ToList();
            }

            return Ok(videoRequests);
        }

        /// <summary>
        /// Retrieves a video request by its unique identifier.
        /// </summary>
        /// <remarks>This method requires the caller to have the appropriate Azure AD B2C scope specified
        /// in the configuration key <c>AzureAdB2C:Scopes:Read</c>.</remarks>
        /// <param name="id">The unique identifier of the video request to retrieve.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the <see cref="VideoRequestModel"/> if found; otherwise, a <see
        /// cref="NotFoundResult"/> if no video request exists with the specified identifier.</returns>
        [HttpGet("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<VideoRequestModel>> GetById(int id)
        {
            var videoRequest = await _videoRequestService.GetByIdAsync(id);
            if (videoRequest == null)
            {
                return NotFound();
            }
            return Ok(videoRequest);
        }

        /// <summary>
        /// Retrieves a collection of video requests associated with the specified user ID.
        /// </summary>
        /// <remarks>This method requires the caller to have the scope specified by the 
        /// "AzureAdB2C:Scopes:Read" configuration key. If the user ID does not exist or the user has  no associated
        /// video requests, an empty collection is returned.</remarks>
        /// <param name="userId">The unique identifier of the user whose video requests are to be retrieved.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing an <see cref="IEnumerable{T}"/> of  <see
        /// cref="VideoRequestModel"/> objects representing the video requests for the specified user.</returns>
        [HttpGet("user/{userId}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<IEnumerable<VideoRequestModel>>> GetByUserId(int userId)
        {
            var videoRequests = await _videoRequestService.GetByUserIdAsync(userId);
            return Ok(videoRequests);
        }

        /// <summary>
        /// Creates a new video request and returns the created resource.
        /// </summary>
        /// <remarks>This method requires the caller to have the appropriate scope defined in the 
        /// "AzureAdB2C:Scopes:Write" configuration key. The created resource is returned with a  201 Created status
        /// code and includes a location header for retrieving the resource by ID.</remarks>
        /// <param name="model">The <see cref="VideoRequestModel"/> containing the details of the video request to create.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the created <see cref="VideoRequestModel"/>  and a location
        /// header pointing to the resource.</returns>
        [HttpPost]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<ActionResult<VideoRequestModel>> Create(VideoRequestModel model)
        {
            var createdVideoRequest = await _videoRequestService.CreateAsync(model);
            //await _videoRequestService.SendVideoRequestAckEmail(model); // we are using SQLTrigger to send email automatically
            return CreatedAtAction(nameof(GetById), new { id = createdVideoRequest.VideoRequestId }, createdVideoRequest);
        }

        /// <summary>
        /// Updates an existing video request with the specified ID using the provided data.
        /// </summary>
        /// <remarks>This method requires the caller to have the appropriate scope defined in the 
        /// "AzureAdB2C:Scopes:Write" configuration key. Ensure that the backend is properly secured  to prevent
        /// unauthorized access.</remarks>
        /// <param name="id">The unique identifier of the video request to update.</param>
        /// <param name="model">The updated data for the video request.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.  Returns <see
        /// cref="OkObjectResult"/> with the updated video request if the operation succeeds,  or <see
        /// cref="NotFoundResult"/> if no video request with the specified ID exists.</returns>
        [HttpPut("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Update(int id, VideoRequestModel model)
        {
            try
            {
                //if a user who knows this end point and not having role as admin can directly hit. this is a 
                //security issue. we will see how we can fix this in security video in this series.
                // we have now restricted from UI app but backend is not protected.
                var updatedVideoRequest = await _videoRequestService.UpdateAsync(id, model);
                //await _videoRequestService.SendVideoRequestAckEmail(model); // we are using SQLTrigger to send email automatically
                return Ok(updatedVideoRequest);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes the video request with the specified identifier.
        /// </summary>
        /// <remarks>This operation requires the caller to have the appropriate scope specified  in the
        /// configuration key <c>AzureAdB2C:Scopes:Write</c>.</remarks>
        /// <param name="id">The unique identifier of the video request to delete.</param>
        /// <returns>A <see cref="NoContentResult"/> indicating that the deletion was successful.</returns>
        [HttpDelete("{id}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> Delete(int id)
        {
            await _videoRequestService.DeleteAsync(id);
            return NoContent();
        }
    }

}
