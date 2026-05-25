using APBD_TASK8.Data;
using APBD_TASK8.DTOs;
using APBD_TASK8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace APBD_TASK8.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly UniversityTasksDbContext _context;

        public CoursesController(UniversityTasksDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] bool activeOnly = true)
        {
            var courses = _context.Courses.AsNoTracking().AsQueryable();

            if (activeOnly)
                courses = courses.Where(c => c.IsActive);

            var result = await courses
                .Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Code = c.Code,
                    Name = c.Name,
                    Credits = c.Credits,
                    AssignmentCount = c.Assignments.Count
                })
                .ToListAsync();

            return Ok(result);
        }

 
        [HttpGet("{idCourse:int}/assignments")]
        public async Task<IActionResult> GetCourseAssignments(int idCourse, [FromQuery] bool publishedOnly = true)
        {
            var courseExists = await _context.Courses
                .AsNoTracking().
                AnyAsync(c => c.CourseId == idCourse);

            if (!courseExists)
                return NotFound($"Course was not found");

            var assignments = _context.Assignments
                .AsNoTracking()
                .Where(a => a.CourseId == idCourse)
                .AsQueryable();

            if (publishedOnly)
                assignments = assignments.Where(a => a.IsPublished);

            var result = await assignments
                .Select(a => new AssignmentDto
                {
                    AssignmentId = a.AssignmentId,
                    Title = a.Title,
                    DueDate = a.DueDate,
                    MaxPoints = a.MaxPoints,
                    IsPublished = a.IsPublished,
                    SubmissionCount = a.Submissions.Count
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
