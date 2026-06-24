using Scalar.AspNetCore;
using UserService.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

// app.UseHttpsRedirection();

var users = new List<User>
{
    new ( 1, "Alice", "alice@example.com" )
};

app.MapGet("/users", () => users);

app.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users", (User user) =>
{
    users.Add(user);
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();

