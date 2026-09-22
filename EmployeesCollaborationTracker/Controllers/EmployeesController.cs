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
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeFileReader fileReader, ICollaborationService collaborationService, ILogger<EmployeesController> logger)
        {
            _fileReader = fileReader;
            _collaborationService = collaborationService;
            _logger = logger;
        }

        [HttpPost("collaborations")]
        public async Task<IActionResult> GetCollaborations(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .csv files are supported.");
            }

            List<Domain.Entities.EmployeeProject> employeeProjects;

            try
            {
                using var stream = file.OpenReadStream();
                employeeProjects = await _fileReader.ReadAsync(stream, cancellationToken);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to read the uploaded employees file {FileName}.", file.FileName);
                return BadRequest("The uploaded file could not be read. Please make sure it is a valid CSV file.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while reading the uploaded employees file {FileName}.", file.FileName);
                return StatusCode(500, "An unexpected error occurred while processing the file.");
            }

            if (employeeProjects.Count == 0)
            {
                return BadRequest("The uploaded file did not contain any valid employee/project records.");
            }

            try
            {
                var result = _collaborationService.GetCollaboratingPairs(employeeProjects);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compute employee collaborations for {FileName}.", file.FileName);
                return StatusCode(500, "An unexpected error occurred while processing the file.");
            }
        }
    }
}
