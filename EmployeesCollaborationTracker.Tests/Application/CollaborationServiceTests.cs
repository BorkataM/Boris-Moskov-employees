using EmployeesCollaborationTracker.Application.Services;
using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Tests.Application
{
    public class CollaborationServiceTests
    {
        private readonly CollaborationService _sut = new();

        [Fact]
        public void GetCollaboratingPairs_WithOverlappingAssignments_ReturnsPairWithCorrectDayCount()
        {
            var employeeProjects = new List<EmployeeProject>
            {
                new() { EmployeeId = 143, ProjectId = 12, DateFrom = new DateOnly(2013, 11, 1), DateTo = new DateOnly(2014, 1, 5) },
                new() { EmployeeId = 218, ProjectId = 12, DateFrom = new DateOnly(2013, 11, 15), DateTo = new DateOnly(2014, 1, 5) },
            };

            var result = _sut.GetCollaboratingPairs(employeeProjects);

            var pair = Assert.Single(result);
            Assert.Equal(143, pair.EmployeeId1);
            Assert.Equal(218, pair.EmployeeId2);
            Assert.Equal(52, pair.TotalDaysWorked);
        }

        [Fact]
        public void GetCollaboratingPairs_WithNonOverlappingAssignments_ReturnsNoPairs()
        {
            var employeeProjects = new List<EmployeeProject>
            {
                new() { EmployeeId = 143, ProjectId = 12, DateFrom = new DateOnly(2013, 1, 1), DateTo = new DateOnly(2013, 6, 1) },
                new() { EmployeeId = 218, ProjectId = 12, DateFrom = new DateOnly(2013, 7, 1), DateTo = new DateOnly(2013, 12, 1) },
            };

            var result = _sut.GetCollaboratingPairs(employeeProjects);

            Assert.Empty(result);
        }

        [Fact]
        public void GetCollaboratingPairs_SameEmployeeOnSameProject_IsNotPairedWithItself()
        {
            var employeeProjects = new List<EmployeeProject>
            {
                new() { EmployeeId = 1, ProjectId = 1, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 6, 1) },
                new() { EmployeeId = 1, ProjectId = 1, DateFrom = new DateOnly(2020, 2, 1), DateTo = new DateOnly(2020, 7, 1) },
            };

            var result = _sut.GetCollaboratingPairs(employeeProjects);

            Assert.Empty(result);
        }

        [Fact]
        public void GetCollaboratingPairs_AcrossMultipleProjects_SumsTotalDaysAndListsEachProject()
        {
            var employeeProjects = new List<EmployeeProject>
            {
                new() { EmployeeId = 1, ProjectId = 10, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 1, 11) },
                new() { EmployeeId = 2, ProjectId = 10, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 1, 11) },
                new() { EmployeeId = 1, ProjectId = 20, DateFrom = new DateOnly(2020, 2, 1), DateTo = new DateOnly(2020, 2, 6) },
                new() { EmployeeId = 2, ProjectId = 20, DateFrom = new DateOnly(2020, 2, 1), DateTo = new DateOnly(2020, 2, 6) },
            };

            var result = _sut.GetCollaboratingPairs(employeeProjects);

            var pair = Assert.Single(result);
            Assert.Equal(11 + 6, pair.TotalDaysWorked);
            Assert.Equal(2, pair.Projects.Count);
        }

        [Fact]
        public void GetCollaboratingPairs_OrdersResultsByTotalDaysWorkedDescending()
        {
            var employeeProjects = new List<EmployeeProject>
            {
                new() { EmployeeId = 1, ProjectId = 1, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 1, 6) },
                new() { EmployeeId = 2, ProjectId = 1, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 1, 6) },
                new() { EmployeeId = 3, ProjectId = 2, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 2, 1) },
                new() { EmployeeId = 4, ProjectId = 2, DateFrom = new DateOnly(2020, 1, 1), DateTo = new DateOnly(2020, 2, 1) },
            };

            var result = _sut.GetCollaboratingPairs(employeeProjects);

            Assert.Equal(2, result.Count);
            Assert.True(result[0].TotalDaysWorked > result[1].TotalDaysWorked);
        }

        [Fact]
        public void GetCollaboratingPairs_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetCollaboratingPairs(null!));
        }
    }
}
