using EmployeesCollaborationTracker.Application.DTOs;
using EmployeesCollaborationTracker.Application.Interfaces;
using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Application.Services
{
    public class CollaborationService : ICollaborationService
    {
        public List<CollaborationResultDto> GetCollaboratingPairs(IEnumerable<EmployeeProject> employeeProjects)
        {
            if (employeeProjects == null)
            {
                throw new ArgumentNullException(nameof(employeeProjects));
            }

            var projects = new List<ProjectCollaborationDto>();

            foreach (var group in employeeProjects.GroupBy(e => e.ProjectId))
            {
                var assignments = group.ToList();

                for (int i = 0; i < assignments.Count; i++)
                {
                    for (int j = i + 1; j < assignments.Count; j++)
                    {
                        var overlap = GetOverlap(assignments[i], assignments[j], group.Key);
                        if (overlap != null)
                        {
                            projects.Add(overlap);
                        }
                    }
                }
            }

            return projects
                .GroupBy(p => (p.EmployeeId1, p.EmployeeId2))
                .Select(g => new CollaborationResultDto
                {
                    EmployeeId1 = g.Key.EmployeeId1,
                    EmployeeId2 = g.Key.EmployeeId2,
                    TotalDaysWorked = g.Sum(p => p.DaysWorked),
                    Projects = g.ToList()
                })
                .OrderByDescending(r => r.TotalDaysWorked)
                .ToList();
        }

        private static ProjectCollaborationDto? GetOverlap(EmployeeProject a, EmployeeProject b, int projectId)
        {
            if (a.EmployeeId == b.EmployeeId)
            {
                return null;
            }

            var start = a.DateFrom > b.DateFrom ? a.DateFrom : b.DateFrom;
            var end = a.DateTo < b.DateTo ? a.DateTo : b.DateTo;
            var days = end.DayNumber - start.DayNumber + 1;

            if (days <= 0)
            {
                return null;
            }

            var (id1, id2) = a.EmployeeId < b.EmployeeId
                ? (a.EmployeeId, b.EmployeeId)
                : (b.EmployeeId, a.EmployeeId);

            return new ProjectCollaborationDto
            {
                EmployeeId1 = id1,
                EmployeeId2 = id2,
                ProjectId = projectId,
                DaysWorked = days
            };
        }
    }
}