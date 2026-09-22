using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Application.Interfaces
{
    public interface IEmployeeFileReader
    {
        Task<List<EmployeeProject>> ReadAsync(Stream fileStream, CancellationToken cancellationToken = default);
    }
}
