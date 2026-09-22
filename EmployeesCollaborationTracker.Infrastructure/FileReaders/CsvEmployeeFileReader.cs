using EmployeesCollaborationTracker.Application.Interfaces;
using EmployeesCollaborationTracker.Domain.Entities;

namespace EmployeesCollaborationTracker.Infrastructure.FileReaders
{
    public class CsvEmployeeFileReader : IEmployeeFileReader
    {
        private readonly IDateParser _dateParser;

        public CsvEmployeeFileReader(IDateParser dateParser)
        {
            _dateParser = dateParser;
        }

        public List<EmployeeProject> Read(Stream fileStream)
        {
            var result = new List<EmployeeProject>();

            using (var reader = new StreamReader(fileStream))
            {
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var record = ParseLine(line);
                    if (record != null)
                    {
                        result.Add(record);
                    }
                }
            }

            return result;
        }

        private EmployeeProject? ParseLine(string line)
        {
            var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 4)
            {
                return null;
            }

            if (!int.TryParse(parts[0].Trim(), out var employeeId))
            {
                return null;
            }

            if (!int.TryParse(parts[1].Trim(), out var projectId))
            {
                return null;
            }

            var dateFrom = _dateParser.Parse(parts[2]);
            var dateTo = _dateParser.Parse(parts[3]);

            return new EmployeeProject
            {
                EmployeeId = employeeId,
                ProjectId = projectId,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }
    }
}