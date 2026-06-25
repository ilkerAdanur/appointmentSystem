using AppointmentService.Models;
using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using FluentValidation;
using AppointmentService.Validators;
using AppointmentService.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointmentDtoValidator>();

builder.Services.AddScoped<IAppointmentService, AppointmentService.Services.AppointmentService>();

var app = builder.Build();

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
    try
    {
        var result = await service.Create(dto);

        return Results.Created($"/appointments/{result.Id}", result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.AddEndpointFilter<ValidationFilter<CreateAppointmentDto>>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();