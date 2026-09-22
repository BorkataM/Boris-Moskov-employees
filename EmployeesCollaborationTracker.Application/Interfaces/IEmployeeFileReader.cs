using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Application.Interfaces
{
    public interface IEmployeeFileReader
    {
        List<EmployeeProject> Read(Stream fileStream);
    }
}
