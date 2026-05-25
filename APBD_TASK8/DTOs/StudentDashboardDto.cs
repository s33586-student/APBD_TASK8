using APBD_TASK8.Models;

namespace APBD_TASK8.DTOs
{
    public class StudentDashboardDto
    {
        public int StudentId { get; set; }
        public string IndexNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; }

        public List<EnrollmentDto> Enrollments { get; set; } = new();
        public List<SubmissionDto> Submissions { get; set; } = new();

        public class EnrollmentDto
        {
            public int EnrollmentId { get; set; }
            public DateOnly EnrolledAt { get; set; }
            public string Status { get; set; } = null!;
        }
    }
}
