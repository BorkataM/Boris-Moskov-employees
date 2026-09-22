using EmployeesCollaborationTracker.Application.Interfaces;
using System.Globalization;

namespace EmployeesCollaborationTracker.Infrastructure.Parsing
{
    public class DateParser : IDateParser
    {
        private static readonly string[] SupportedFormats = new[]
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "dd-MM-yyyy",
            "dd/MM/yyyy",
            "MM-dd-yyyy",
            "MM/dd/yyyy",
            "d.M.yyyy",
            "yyyy.MM.dd"
        };

        public DateOnly Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                return DateOnly.FromDateTime(DateTime.Today);
            }

            var trimmed = value.Trim();

            if (DateOnly.TryParseExact(trimmed, SupportedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactResult))
            {
                return exactResult;
            }

            if (DateOnly.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fallbackResult))
            {
                return fallbackResult;
            }

            throw new FormatException($"Unable to parse date value: '{value}'");
        }
    }
}
