using EmployeesCollaborationTracker.Application.Interfaces;
using EmployeesCollaborationTracker.Application.Services;
using EmployeesCollaborationTracker.Infrastructure.FileReaders;
using EmployeesCollaborationTracker.Infrastructure.Parsing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IDateParser, DateParser>();
builder.Services.AddScoped<IEmployeeFileReader, CsvEmployeeFileReader>();
builder.Services.AddScoped<ICollaborationService, CollaborationService>();

var clientOrigin = "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("EmployeesCollaborationTrackerClient", policy =>
    {
        policy.WithOrigins(clientOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("EmployeesCollaborationTrackerClient");

app.MapControllers();

app.Run();