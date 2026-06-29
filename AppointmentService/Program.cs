using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using AppointmentService.Validators;
using AppointmentService.Filters;
using AppointmentService.ExceptionHandlers;
using AppointmentService.Configuration;
using Serilog;
using Polly;
using Polly.Extensions.Http;
using Microsoft.EntityFrameworkCore.Migrations;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/appointment-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks();

builder.Services.AddOpenApi();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient("UserServiceClient")
    .AddPolicyHandler(retryPolicy);


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.Configure<ServiceUrls>(
    builder.Configuration.GetSection("Services"));

builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentDtoValidator>();

builder.Services.AddScoped<IAppointmentService, AppointmentService.Services.AppointmentService>();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
    db.Database.Migrate();
}

app.MapHealthChecks("/health");

app.MapGet("/appointments/{id}/details", async (
int id,
IAppointmentService service) =>
{
    var result = await service.GetAppointmentDetails(id);

    if (result is null)
        return Results.NotFound("Appointment not found");

    return Results.Ok(result);
});

app.MapGet("/appointments", async (IAppointmentService service) =>
{
    return await service.GetAll();
});

app.MapGet("/appointments/{id}", async (int id, IAppointmentService service) =>
{
    var appointment = await service.GetById(id);

    return appointment is null
    ? Results.NotFound()
    : Results.Ok(appointment);
});

app.MapPost("/appointments", async (
    CreateAppointmentDto dto,
    IAppointmentService service) =>
{
    var result = await service.Create(dto);

    return Results.Created($"/appointments/{result.Id}", result);
})
.AddEndpointFilter<ValidationFilter<CreateAppointmentDto>>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();