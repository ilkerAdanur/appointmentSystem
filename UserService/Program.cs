using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

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

app.MapPost("/users", async (CreateUserDto dto, UserDbContext db) =>
{
    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created(
        $"/users/{user.Id}",
        new UserResponseDto
        {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email
        }
    );

    // db.Users.Add(user);
    // await db.SaveChangesAsync();
    // return Results.Created($"/users/{user.Id}", user);
});

app.Run();

