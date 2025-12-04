using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;

namespace LSC.OnlineCourse.Service
{
    public interface ICourseCategoryService
    {
        /// <summary>
        /// compute course category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CourseCategoryModel?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a list of all available course categories.
        /// </summary>
        /// <remarks>The returned list may be empty if no course categories are available. Ensure that the
        /// caller  awaits the task to retrieve the result.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
        /// cref="CourseCategoryModel"/> objects representing the available course categories.</returns>
        Task<List<CourseCategoryModel>> GetCourseCategories();
    }

    public class CourseCategoryService : ICourseCategoryService
    {
        /// <summary>
        /// Represents the repository used to manage course categories.
        /// </summary>
        /// <remarks>This field is read-only and is intended to provide access to the data source for
        /// course category operations.</remarks>
        private readonly ICourseCategoryRepository categoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseCategoryService"/> class.
        /// </summary>
        /// <param name="categoryRepository">The repository used to manage course category data. This parameter cannot be null.</param>
        public CourseCategoryService(ICourseCategoryRepository  categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Retrieves a course category by its unique identifier.
        /// </summary>
        /// <remarks>This method queries the repository for a course category with the specified
        /// identifier.  If a matching category is found, it is mapped to a <see cref="CourseCategoryModel"/> and
        /// returned.</remarks>
        /// <param name="id">The unique identifier of the course category to retrieve.</param>
        /// <returns>A <see cref="CourseCategoryModel"/> representing the course category if found; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<CourseCategoryModel?> GetByIdAsync(int id)
        {
            var category = await categoryRepository.GetById(id);
            if (category != null)
            {
                return new CourseCategoryModel()
                {
                    CategoryName = category.CategoryName,
                    CategoryId = category.CategoryId,
                    Description = category.Description
                };
            }
            return null;
        }

        /// <summary>
        /// Retrieves a list of course categories.
        /// </summary>
        /// <remarks>This method asynchronously fetches course categories from the data source and maps
        /// them to a list of  <see cref="CourseCategoryModel"/> objects. Each category includes its name, ID, and
        /// description.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
        /// cref="CourseCategoryModel"/> objects, where each object represents a course category.</returns>
        public async Task<List<CourseCategoryModel>> GetCourseCategories()
        {
            var categories = await categoryRepository.GetCourseCategories();
            return categories.Select(c => new CourseCategoryModel()
            {
                CategoryName = c.CategoryName,
                CategoryId = c.CategoryId,
                Description = c.Description
            }).ToList();
        }
    }
}
