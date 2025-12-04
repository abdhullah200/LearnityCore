using System;
using System.Collections.Generic;

namespace LSC.OnlineCourse.Core.Entities;

public partial class CourseCategory
{
    /// <summary>
    /// category of courses
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// category name
    /// </summary>
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// courses under this category
    /// </summary>
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
