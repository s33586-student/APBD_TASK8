using APBD_TASK8.Models;

namespace APBD_TASK8.DTOs
{
    public class AssignmentDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = null!;
        public DateTime DueDate { get; set; }
        public int MaxPoints { get; set; }
        public bool IsPublished { get; set; }
        public int SubmissionCount { get; set; }
    }
}
