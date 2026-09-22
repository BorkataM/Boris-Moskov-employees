using EmployeesCollaborationTracker.Application.Interfaces;
using EmployeesCollaborationTracker.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EmployeesCollaborationTracker.Infrastructure.FileReaders
{
    public class CsvEmployeeFileReader : IEmployeeFileReader
    {
        private readonly IDateParser _dateParser;
        private readonly ILogger<CsvEmployeeFileReader> _logger;

        public CsvEmployeeFileReader(IDateParser dateParser, ILogger<CsvEmployeeFileReader> logger)
        {
            _dateParser = dateParser;
            _logger = logger;
        }

        public async Task<List<EmployeeProject>> ReadAsync(Stream fileStream, CancellationToken cancellationToken = default)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            var result = new List<EmployeeProject>();

            using (var reader = new StreamReader(fileStream))
            {
                string? line;
                var isFirstLine = true;

                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    try
                    {
                        var record = ParseLine(line);
                        if (record != null)
                        {
                            result.Add(record);
                        }
                        else if (!isFirstLine)
                        {
                            _logger.LogWarning("Skipped malformed CSV line: '{Line}'.", line);
                        }
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning(ex, "Skipped CSV line due to an unparsable date: '{Line}'.", line);
                    }
                    finally
                    {
                        isFirstLine = false;
                    }
                }
            }

            return result;
        }

        private EmployeeProject? ParseLine(string line)
        {
            var parts = line.Split(',');

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

            if (dateFrom > dateTo)
            {
                return null;
            }

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
