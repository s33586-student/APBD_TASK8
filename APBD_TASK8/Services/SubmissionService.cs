using APBD_TASK8.Data;
using APBD_TASK8.DTOs;
using APBD_TASK8.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_TASK8.Services
{
    public class SubmissionService
    {
        private readonly UniversityTasksDbContext _context;

        public SubmissionService(UniversityTasksDbContext context)
        {
            _context = context;
        }

        public async Task<(bool success, int statusCode, string? error, SubmissionDto? data)> CreateSubmissionAsync(CreateSubmissionDto request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId);
            var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.AssignmentId == request.AssignmentId);

            if (student == null)
            {
                return (false, 404, $"Student with id {request.StudentId} was not found", null);
            }else if (!student.IsActive)
            {
                return (false, 400, "Student is not active", null);
            }
            if (assignment == null)
            {
                return (false, 404, $"Assignment with id {request.AssignmentId} was not found", null);
            }else if (!assignment.IsPublished)
            {
                return (false, 400, "Assignment is not published", null);
            }

            var isEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentId == request.StudentId &&
                    e.CourseId == assignment.CourseId &&
                    (e.Status == "Active" || e.Status == "Completed"));

            if (!isEnrolled)
            {
                return (false, 400, "Student is not enrolled in the course for this assignment", null);
            }
            if (request.RepositoryUrl == null || request.RepositoryUrl == "")
            {
                return (false, 400, "RepositoryUrl cannot be blank", null);
            }else if (!request.RepositoryUrl.StartsWith("https://"))
            {
                return (false, 400, "RepositoryUrl must start with https://", null);
            }

            var alreadySubmitted = await _context.Submissions
                .AnyAsync(s =>
                    s.StudentId == request.StudentId &&
                    s.AssignmentId == request.AssignmentId);

            if (alreadySubmitted)
            {
                return (false, 409, "Student has already submitted this assignment.", null);
            }

            var submission = new Submission
            {
                AssignmentId = request.AssignmentId,
                StudentId = request.StudentId,
                RepositoryUrl = request.RepositoryUrl,
                SubmittedAt = DateTime.Now,
                Score = null,
                Feedback = null,
                Status = (DateTime.Now > assignment.DueDate) ? "Late" : "Submitted"
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            var result = new SubmissionDto
            {
                SubmissionId = submission.SubmissionId,
                Student = student.FullName,
                Assignment = assignment.Title,
                RepositoryUrl = submission.RepositoryUrl,
                Status = submission.Status,
                Score = submission.Score,
                Feedback = submission.Feedback
            };

            return (true, 201, null, result);
        }

        //TODO: add GradeSubmission and DeleteSubmission
    }
}
