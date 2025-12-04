using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {
        /// <summary>
        /// Represents the database context used for accessing and managing online course data.
        /// </summary>
        /// <remarks>This field is read-only and is intended to be used internally within the class to
        /// interact with the database.</remarks>
        private readonly OnlineCourseDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseCategoryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context used to interact with the underlying data store. This parameter cannot be <see
        /// langword="null"/>.</param>
        public CourseCategoryRepository(OnlineCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a course category by its unique identifier.
        /// </summary>
        /// <remarks>This method queries the database for a course category with the specified identifier.
        /// If no matching course category is found, the result will be <see langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the course category to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the  <see
        /// cref="CourseCategory"/> if found; otherwise, <see langword="null"/>.</returns>
        public Task<CourseCategory?> GetById(int id)
        {
            return dbContext.CourseCategories.FindAsync(id).AsTask();
        }

        /// <summary>
        /// Retrieves a list of all course categories.
        /// </summary>
        /// <remarks>This method asynchronously fetches all course categories from the database. The
        /// returned list will be empty if no course categories are found.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see
        /// cref="CourseCategory"/> objects representing the course categories.</returns>
        public Task<List<CourseCategory>> GetCourseCategories()
        {
            return dbContext.CourseCategories.ToListAsync();
        }
    }
}
