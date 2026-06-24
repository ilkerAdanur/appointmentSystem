using Scalar.AspNetCore;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

// app.UseHttpsRedirection();

app.MapGet("/users", async (UserDbContext db) => {
    return await db.Users.ToListAsync(); 
});

app.MapGet("/users/{id}", async (int id, UserDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    return  user is not null 
    ? Results.Ok(user) 
    : Results.NotFound();
});

app.MapPost("/users", async (User user, UserDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();

