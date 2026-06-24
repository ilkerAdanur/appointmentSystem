using AppointmentService.Models;
using AppointmentService.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using AppointmentService.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));


var app = builder.Build();

app.MapGet("/appointments/{id}/details", async(
int id,
AppointmentDbContext db,
IHttpClientFactory httpClientFactory,
IConfiguration configuration) =>
{
    var appointment = await db.Appointments.FindAsync(id);
    if(appointment is null)
        return Results.NotFound("Appointment not found");
    var userServiceUrl = configuration["Services:UserService"];
    var client = httpClientFactory.CreateClient();

    var user = await client.GetFromJsonAsync<object>(
        $"{userServiceUrl}/users/{appointment.UserId}");
    return Results.Ok(new
    {
       appointment.Id,
       appointment.AppointmentDate,
       appointment.Description,
       User = user 
    });
});

app.MapGet("/appointments", async (AppointmentDbContext db) =>
{
    return await db.Appointments.ToListAsync();
});

app.MapGet("/appointments/{id}", async (int id, AppointmentDbContext db) =>
{
    var appointment = await db.Appointments.FindAsync(id);
    return appointment is null
    ? Results.NotFound()
    : Results.Ok(appointment);
});

app.MapPost("/appointments", async (
    CreateAppointmentDto dto,
    AppointmentDbContext db,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) =>
{
    var appointment = new Appointment
    {
        AppointmentDate = dto.AppointmentDate,
        Description = dto.Description,
        UserId = dto.UserId
    };
    db.Appointments.Add(appointment);
    await db.SaveChangesAsync();

    return Results.Created(
         $"/appointments/{appointment.Id}",
         new AppointmentResponseDto
         {
             Id = appointment.Id,
             AppointmentDate = appointment.AppointmentDate,
             Description = appointment.Description,
             UserId = appointment.UserId
         }
    );
    // var userServiceUrl = configuration["Services:UserService"];
    // var client = httpClientFactory.CreateClient();
    // var response = await client.GetAsync($"{userServiceUrl}/users/{appointment.UserId}");
    // if (!response.IsSuccessStatusCode)
    //     return Results.BadRequest("User not Found");

    // db.Appointments.Add(appointment);
    // await db.SaveChangesAsync();
    // return Results.Created($"/appointments/{appointment.Id}",appointment);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.Run();

