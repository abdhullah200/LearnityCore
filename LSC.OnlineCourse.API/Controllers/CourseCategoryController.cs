using LSC.OnlineCourse.Data.Entities;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing course categories, including retrieving individual categories by their unique
    /// identifier and retrieving all categories.
    /// </summary>
    /// <remarks>This controller is part of the API layer and is responsible for handling HTTP requests
    /// related to course categories. It supports anonymous access and provides methods to retrieve course category
    /// data.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CourseCategoryController : ControllerBase
    {

        private readonly ILogger<CourseCategoryController> _logger;
        private readonly ICourseCategoryService categoryService;

        public CourseCategoryController(ILogger<CourseCategoryController> logger, ICourseCategoryService categoryService)
        {
            _logger = logger;
            this.categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <remarks>This method returns an HTTP 200 OK response with the category data if the category is
        /// found,  or an HTTP 404 Not Found response if no category with the specified identifier exists.</remarks>
        /// <param name="id">The unique identifier of the category to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the category data if found;  otherwise, a <see
        /// cref="NotFoundResult"/> if the category does not exist.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await categoryService.GetByIdAsync(id);
            //what if the id is not present?
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        /// <summary>
        /// Retrieves all course categories.
        /// </summary>
        /// <remarks>This method returns a collection of course categories available in the system. The
        /// result is returned as an HTTP 200 OK response containing the categories.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing an HTTP 200 OK response with the list of course categories.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await categoryService.GetCourseCategories();
            return Ok(categories);
        }
    }
}
