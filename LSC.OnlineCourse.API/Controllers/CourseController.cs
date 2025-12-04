using LSC.OnlineCourse.API.Common.LSC.OnlineCourse.API.Common;
using LSC.OnlineCourse.API.Model;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.Resource;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// This controller handles operations related to courses, including retrieving course details,
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IAzureBlobStorageService blobStorageService;

        public CourseController(ICourseService courseService, IAzureBlobStorageService blobStorageService)
        {
            this.courseService = courseService;
            this.blobStorageService = blobStorageService;
        }

        /// <summary>
        /// This method retrieves a list of all available courses.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CourseModel>>> GetAllCoursesAsync()
        {
            var courses = await courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        /// <summary>
        /// This method retrieves a list of all available courses filtered by a specific category identifier.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("Category/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CourseModel>>> GetAllCoursesByCategoryIdAsync([FromRoute] int categoryId)
        {
            var courses = await courseService.GetAllCoursesAsync(categoryId);
            return Ok(courses);
        }

        /// <summary>
        /// This method retrieves detailed information about a specific course based on its unique identifier.
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("Detail/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDetailModel>> GetCourseDetailAsync(int courseId)
        {
            var courseDetail = await courseService.GetCourseDetailAsync(courseId);
            if (courseDetail == null)
            {
                return NotFound();
            }
            return Ok(courseDetail);
        }

        /// <summary>
        /// This method retrieves detailed information about a specific course based on its unique identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await courseService.GetCourseDetailAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        /// <summary>
        /// This method adds a new course to the system.
        /// </summary>
        /// <param name="courseModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> AddCourse([FromBody] CourseDetailModel courseModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await courseService.AddCourseAsync(courseModel);
            return Ok();
        }

        /// <summary>
        /// This method updates an existing course in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="courseModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDetailModel courseModel)
        {
            if (id != courseModel.CourseId)
            {
                return BadRequest("Course ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await courseService.UpdateCourseAsync(courseModel);
            return NoContent();
        }


        /// <summary>
        /// Deletes the course with the specified identifier.
        /// </summary>
        /// <remarks>This operation requires the caller to have administrative privileges and the
        /// appropriate authorization scope.</remarks>
        /// <param name="id">The unique identifier of the course to delete.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="NoContentResult"/> if the deletion is successful.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [AdminRole]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Write")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of all instructors.
        /// </summary>
        /// <remarks>This method returns a collection of instructors available in the system.  The caller
        /// must have the required scope specified in the configuration to access this endpoint.</remarks>
        /// <returns>An <see cref="ActionResult{T}"/> containing a list of <see cref="InstructorModel"/> objects. Returns an
        /// empty list if no instructors are found.</returns>
        [HttpGet("Instructors")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAdB2C:Scopes:Read")]
        public async Task<ActionResult<List<InstructorModel>>> GetInstructors()
        {
            var instructors = await courseService.GetAllInstructorsAsync();
            return Ok(instructors);
        }

        /// <summary>
        /// Handles the upload of a course thumbnail image and updates the course with the new thumbnail URL.
        /// </summary>
        /// <remarks>This method requires the caller to be authenticated and have the "Admin" role.  The
        /// course ID must be provided in the request form data under the key "courseId".  The uploaded file is stored
        /// in Azure Blob Storage, and the course's thumbnail URL is updated in the database.</remarks>
        /// <param name="file">The thumbnail image file to upload. The file must not be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="BadRequestObjectResult"/> if the file is null or empty,  <see cref="NotFoundObjectResult"/> if the
        /// course does not exist, or  <see cref="OkObjectResult"/> with the thumbnail URL upon successful upload.</returns>
        [HttpPost("upload-thumbnail")]
        [Authorize]
        [AdminRole]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            var courseId = Convert.ToInt32(Request.Form["courseId"]);
            string thumbnailUrl = null;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var course = await courseService.GetCourseDetailAsync(courseId);
            if (course == null)
                return NotFound("Course not found");

            if (file != null)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    // Upload the byte array or stream to Azure Blob Storage
                    thumbnailUrl = await blobStorageService.UploadAsync(
                        stream.ToArray(), $"{courseId}_{course.Title.Trim().Replace(' ', '_')}.{file.FileName.Split('.').LastOrDefault()}", "course-preview");
                }

                // Update the profile picture URL in the database
                await courseService.UpdateCourseThumbnail(thumbnailUrl, courseId);
            }



            return Ok(new { message = "Thumbnail uploaded successfully", thumbnailUrl });
        }
    }

}
