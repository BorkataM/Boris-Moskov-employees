# Employees Collaboration Tracker

Identifies which pairs of employees have worked together on common projects, and for how long, given a CSV file of employee/project assignments.

Given a CSV row format of `EmpID, ProjectID, DateFrom, DateTo`, the app finds every pair of employees who were assigned to the same project during an overlapping date range, sums the overlapping days per pair across all their shared projects, and returns the pairs sorted by total days worked together (descending) — so the first result is the pair that collaborated the longest.

## Project structure

```
EmployeesCollaborationTracker.slnx
├── EmployeesCollaborationTracker/               API (ASP.NET Core, .NET 10)
├── EmployeesCollaborationTracker.Application/    Application services, DTOs, interfaces
├── EmployeesCollaborationTracker.Domain/         Domain entities
├── EmployeesCollaborationTracker.Infrastructure/ CSV file reading, date parsing
├── EmployeesCollaborationTracker.Tests/          xUnit tests
└── EmployeesCollaborationTracker.Client/         Angular 22 UI
```

## Prerequisites

- .NET 10 SDK
- Node.js 20.19+ / 22.12+ (Node 24 LTS recommended) and npm
- Angular CLI (`npm install -g @angular/cli`), or use `npx ng` instead of `ng` below

## Running the backend

```
cd EmployeesCollaborationTracker
dotnet run --urls http://localhost:5036
```

The API is now available at `http://localhost:5036`.

## Running the frontend

```
cd EmployeesCollaborationTracker.Client
npm install
ng serve
```

Open `http://localhost:4200`. The client calls the API directly at `http://localhost:5036` (hardcoded, since this app is local-only and not deployed), so the backend must be running first. CORS on the API is already scoped to allow `http://localhost:4200`.

Pick a `.csv` file — the results table shows every collaborating pair's shared projects, with a callout above it naming the pair that worked together the longest.

## Running the tests

```
dotnet test EmployeesCollaborationTracker.Tests
```

Covers the core collaboration algorithm (`CollaborationService`). There's no dedicated frontend test suite beyond Angular's default scaffold.

## API

`POST /api/employees/collaborations`

- Body: `multipart/form-data` with a single field named `file` containing the `.csv` upload.
- Response: `200 OK` with a JSON array of collaborating pairs, sorted by `totalDaysWorked` descending:

```json
[
  {
    "employeeId1": 143,
    "employeeId2": 218,
    "totalDaysWorked": 52,
    "projects": [
      { "employeeId1": 143, "employeeId2": 218, "projectId": 12, "daysWorked": 52 }
    ]
  }
]
```

- `400 Bad Request` for a missing/empty file, a non-`.csv` file, or a file with no valid records.
- `500 Internal Server Error` for unexpected failures.

## CSV format

```
EmpID, ProjectID, DateFrom, DateTo
143, 12, 2013-11-01, 2014-01-05
218, 10, 2012-05-16, NULL
143, 10, 2009-01-01, 2011-04-27
```

- `DateTo` may be `NULL` (case-insensitive) or blank, meaning the assignment is still ongoing — treated as today's date.
- A header row is optional; if the first line doesn't parse as a valid record it's silently treated as a header rather than a data-quality warning.
- Malformed lines (wrong column count, unparsable date, or `DateFrom` after `DateTo`) are skipped and logged rather than failing the whole upload.

### Supported date formats

`yyyy-MM-dd`, `yyyy/MM/dd`, `dd-MM-yyyy`, `dd/MM/yyyy`, `MM-dd-yyyy`, `MM/dd/yyyy`, `d.M.yyyy`, `yyyy.MM.dd`, plus a culture-invariant fallback parse for other common formats.
