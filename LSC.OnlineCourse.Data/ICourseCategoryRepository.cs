using LSC.OnlineCourse.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public interface ICourseCategoryRepository
    {
        /// <summary>
        /// Retrieves a course category by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation. Ensure to await the returned task to
        /// retrieve the result.</remarks>
        /// <param name="id">The unique identifier of the course category to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="CourseCategory"/> object representing the course category with the specified identifier,  or
        /// <see langword="null"/> if no course category with the given identifier exists.</returns>
        Task<CourseCategory?> GetById(int id);

        /// <summary>
        /// Retrieves a list of all available course categories.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
        /// cref="CourseCategory"/> objects representing the available course categories.</returns>
        Task<List<CourseCategory>> GetCourseCategories();
    }
}
