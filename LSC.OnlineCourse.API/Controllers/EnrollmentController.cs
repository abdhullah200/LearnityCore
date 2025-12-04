using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing course enrollments, including enrolling in courses, retrieving enrollment
    /// details, and fetching user-specific enrollments.
    /// </summary>
    /// <remarks>This controller handles operations related to course enrollments. It requires authorization
    /// for all actions and provides endpoints for creating new enrollments, retrieving specific enrollment details, and
    /// listing all enrollments for a user.</remarks>
    [Route("api/[controller]")]
    [ApiController]   
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _service;

        public EnrollmentController(IEnrollmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Enrolls a user in a course based on the provided enrollment data.
        /// </summary>
        /// <remarks>This method checks if the user is already enrolled in the specified course before
        /// attempting to enroll them.  If the user is already enrolled, a <see cref="StatusCodes.Status400BadRequest"/>
        /// response is returned.</remarks>
        /// <param name="model">The enrollment data containing the user ID and course ID. This parameter must not be <see langword="null"/>.</param>
        /// <returns>An <see cref="IActionResult"/> containing the result of the enrollment operation.  Returns a <see
        /// cref="StatusCodes.Status200OK"/> response with the enrolled course details if the operation is successful.
        /// Returns a <see cref="StatusCodes.Status400BadRequest"/> response if the enrollment data is invalid or the
        /// user is already enrolled in the course. Returns a <see cref="StatusCodes.Status401Unauthorized"/> response
        /// if the user is not authenticated. Returns a <see cref="StatusCodes.Status403Forbidden"/> response if the
        /// user does not have permission to enroll in the course. Returns a <see
        /// cref="StatusCodes.Status500InternalServerError"/> response if an unexpected error occurs.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnrollCourse([FromBody] CourseEnrollmentModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid enrollment data.");
            }

            var enrollment = await _service.GetUserEnrollmentsAsync(model.UserId);
            if (enrollment != null && enrollment.FirstOrDefault(f => f.CourseId == model.CourseId) != null)
            {
                return BadRequest("Enrollment to this course is already exists!");
            }
            var enrolledCourse = await _service.EnrollCourseAsync(model);
            return Ok(enrolledCourse);
        }

        /// <summary>
        /// Retrieves the enrollment details for a specific course by its identifier.
        /// </summary>
        /// <remarks>This method requires the caller to be authenticated and authorized to access the
        /// specified enrollment.</remarks>
        /// <param name="id">The unique identifier of the course enrollment to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the enrollment details as a <see cref="CourseEnrollmentModel"/> 
        /// if the enrollment is found. Returns a status code indicating the result of the operation: <list
        /// type="bullet"> <item><description><see cref="StatusCodes.Status200OK"/> if the enrollment is successfully
        /// retrieved.</description></item> <item><description><see cref="StatusCodes.Status404NotFound"/> if no
        /// enrollment is found with the specified identifier.</description></item> <item><description><see
        /// cref="StatusCodes.Status401Unauthorized"/> if the caller is not authenticated.</description></item>
        /// <item><description><see cref="StatusCodes.Status403Forbidden"/> if the caller does not have permission to
        /// access the enrollment.</description></item> <item><description><see
        /// cref="StatusCodes.Status500InternalServerError"/> if an unexpected error occurs.</description></item>
        /// </list></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseEnrollmentModel))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEnrollment(int id)
        {
            var enrollment = await _service.GetEnrollmentAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            return Ok(enrollment);
        }

        /// <summary>
        /// Retrieves the list of course enrollments for a specific user.
        /// </summary>
        /// <remarks>This method returns a 200 OK response with the user's enrollments if the request is
        /// successful.  Possible HTTP status codes include: <list type="bullet"> <item><description>200 OK: The
        /// enrollments were successfully retrieved.</description></item> <item><description>401 Unauthorized: The
        /// caller is not authenticated.</description></item> <item><description>403 Forbidden: The caller does not have
        /// permission to access this resource.</description></item> <item><description>500 Internal Server Error: An
        /// unexpected error occurred on the server.</description></item> </list></remarks>
        /// <param name="id">The unique identifier of the user whose enrollments are to be retrieved.</param>
        /// <returns>An <see cref="IActionResult"/> containing a list of <see cref="CourseEnrollmentModel"/> objects 
        /// representing the user's course enrollments if the operation is successful.</returns>
        [HttpGet("user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CourseEnrollmentModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserEnrollments(int id)
        {
            var enrollment = await _service.GetUserEnrollmentsAsync(id);

            return Ok(enrollment);
        }
    }

}
