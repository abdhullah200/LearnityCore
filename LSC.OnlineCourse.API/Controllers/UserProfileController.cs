using LSC.OnlineCourse.API.Common.LSC.OnlineCourse.API.Common;
using LSC.OnlineCourse.API.Model;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user profiles, including retrieving and updating user profile information.
    /// </summary>
    /// <remarks>This controller handles operations related to user profiles, such as fetching user details
    /// and updating profile information,  including profile pictures and bios. It integrates with services for user
    /// profile management and Azure Blob Storage for handling  profile picture uploads.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserProfileController : ControllerBase
    {
        private readonly IAzureBlobStorageService _blobStorageService;
        private readonly IUserProfileService _userProfileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileController"/> class.
        /// </summary>
        /// <param name="blobStorageService">The service used for interacting with Azure Blob Storage. This is typically used for managing user
        /// profile-related files.</param>
        /// <param name="userProfileService">The service responsible for managing user profile data and operations.</param>
        public UserProfileController(IAzureBlobStorageService blobStorageService, IUserProfileService userProfileService)
        {
            _blobStorageService = blobStorageService;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// this method retrieves the profile information of a user based on their unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile([FromRoute] int id)
        {
            var userInfo = await _userProfileService.GetUserInfoAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }
            return Ok(userInfo);
        }

        /// <summary>
        /// Updates the user's profile information, including their profile picture and bio.
        /// </summary>
        /// <remarks>This method allows users to update their profile picture and/or bio. If a profile
        /// picture is provided, it is uploaded to Azure Blob Storage, and the corresponding URL is updated in the
        /// database. If a bio is provided, it is updated in the database. Both updates are optional, and the method
        /// processes only the fields that are provided in the request.</remarks>
        /// <param name="model">An instance of <see cref="UpdateUserProfileModel"/> containing the user's profile data to update. The model
        /// includes the user's ID, an optional profile picture, and an optional bio.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see cref="OkObjectResult"/>
        /// with the updated model if the operation is successful.</returns>
        [HttpPost("updateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileModel model)
        {
            string pictureUrl = null;

            if (model.Picture != null)
            {
                using (var stream = new MemoryStream())
                {
                    await model.Picture.CopyToAsync(stream);

                    // Upload the byte array or stream to Azure Blob Storage
                    pictureUrl = await _blobStorageService.UploadAsync(stream.ToArray(),
                        $"{model.UserId}_profile_picture.{model.Picture.FileName.Split('.').LastOrDefault()}");
                }

                // Update the profile picture URL in the database
                await _userProfileService.UpdateUserProfilePicture(model.UserId, pictureUrl);
            }

            // Update bio
            if (model.Bio != null)
            {
                await _userProfileService.UpdateUserBio(model.UserId, model.Bio);
            }

            return Ok(model);
        }
    }


}
