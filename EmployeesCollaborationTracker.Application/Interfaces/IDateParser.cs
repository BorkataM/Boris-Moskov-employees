using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeesCollaborationTracker.Application.Interfaces
{
    public interface IDateParser
    {
        DateOnly Parse(string? value);
    }
}
