using APBD_TASK8.DTOs;
using APBD_TASK8.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_TASK8.Controllers
{
    [ApiController]
    [Route("api/submissions")]
    public class SubmissionsController: ControllerBase
    {
        private readonly SubmissionService _submissionService;

        public SubmissionsController(SubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubmission(CreateSubmissionDto request)
        {
            var result = await _submissionService.CreateSubmissionAsync(request);

            if (!result.success)
            {
                return StatusCode(result.statusCode, result.error);
            }

            return Created($"/api/submissions/{result.data!.SubmissionId}", result.data);
        }
    }
}
