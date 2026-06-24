using AppointmentService.Models;
using AppointmentService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")
));



var app = builder.Build();

app.MapGet("/appointment", async (AppointmentDbContext db) =>
{
    return await db.Appointments.ToListAsync();
});

app.MapGet("/appointment/{id}", async (int id, AppointmentDbContext db) =>
{
    var appointment = await db.Appointments.FindAsync(id);
    return appointment is null
    ? Results.NotFound()
    : Results.Ok(appointment);
});

app.MapPost("/appointment", async (
    Appointment appointment,
    AppointmentDbContext db,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) =>
{
    var userServiceUrl = configuration["Services:UserService"];
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync($"{userServiceUrl}/users/{appointment.UserId}");
    if (!response.IsSuccessStatusCode)
        return Results.BadRequest("User not Found");

    db.Appointments.Add(appointment);
    await db.SaveChangesAsync();
    return Results.Created($"/appointment/{appointment.Id}",appointment);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.Run();

