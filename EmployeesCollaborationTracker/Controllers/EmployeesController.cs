using EmployeesCollaborationTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesCollaborationTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeFileReader _fileReader;
        private readonly ICollaborationService _collaborationService;

        public EmployeesController(IEmployeeFileReader fileReader, ICollaborationService collaborationService)
        {
            _fileReader = fileReader;
            _collaborationService = collaborationService;
        }

        [HttpPost("collaborations")]
        public IActionResult GetCollaborations(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            List<Domain.Entities.EmployeeProject> employeeProjects;

            using (var stream = file.OpenReadStream())
            {
                employeeProjects = _fileReader.Read(stream);
            }

            var result = _collaborationService.GetCollaboratingPairs(employeeProjects);

            return Ok(result);
        }
    }
}
