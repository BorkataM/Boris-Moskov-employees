using EmployeesCollaborationTracker.Application.DTOs;
using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Application.Interfaces
{
    public interface ICollaborationService
    {
        List<CollaborationResultDto> GetCollaboratingPairs(IEnumerable<EmployeeProject> employeeProjects);
    }
}
