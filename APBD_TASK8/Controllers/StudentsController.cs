using APBD_TASK8.Data;
using APBD_TASK8.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD_TASK8.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityTasksDbContext _context;

        public StudentsController(UniversityTasksDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idStudent:int}/dashboard")]
        public async Task<IActionResult> GetStudentDashboard(int idStudent)
        {
            var studentDashboard = await _context.Students
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FullName,
                IsActive = s.IsActive,

                Enrollments = s.Enrollments
                    .Select(e => new StudentDashboardDto.EnrollmentDto
                    {
                        EnrollmentId = e.EnrollmentId,
                        EnrolledAt = e.EnrolledAt,
                        Status = e.Status
                    })
                    .ToList(),

                Submissions = s.Submissions
                    .Select(sub => new SubmissionDto
                    {
                        SubmissionId = sub.SubmissionId,
                        Student = s.FullName,
                        Assignment = sub.Assignment.Title,
                        RepositoryUrl = sub.RepositoryUrl,
                        Status = sub.Status,
                        Score = sub.Score,
                        Feedback = sub.Feedback
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

            if (studentDashboard == null)
            {
                return NotFound("Student was not found");
            }

            return Ok(studentDashboard);
        }
    }
}
