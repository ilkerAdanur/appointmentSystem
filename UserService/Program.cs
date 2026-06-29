using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Services;
using FluentValidation;
using UserService.Filters;
using UserService.Validators;
using UserService.ExceptionHandlers;
using Serilog;
using Microsoft.AspNetCore.Identity;
using UserService.Models;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/user-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHealthChecks();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

// app.UseHttpsRedirection();

app.MapGet("/users", async (IUserService service) => {
    return await service.GetAll(); 
});

app.MapGet("/users/{id}", async (int id, IUserService service) =>
{
    return await service.GetById(id);
});

app.MapPost("/auth/register", async (
    RegisterUserDto registerUserDto,
    IUserService userService)=>
{
    var registeredUser = await userService.RegisterAsync(registerUserDto);
    return Results.Created(
        $"/users/{registeredUser.Id}",
        registeredUser);
}).AddEndpointFilter<ValidationFilter<RegisterUserDto>>();

// app.MapPost("/users", async (CreateUserDto dto, UserDbContext db) =>
// {
//     var user = new User
//     {
//         Name = dto.Name,
//         Email = dto.Email
//     };
//     db.Users.Add(user);
//     await db.SaveChangesAsync();

//     return Results.Created(
//         $"/users/{user.Id}",
//         new UserResponseDto
//         {
//         Id = user.Id,
//         Name = user.Name,
//         Email = user.Email
//         }
//     );

//     // db.Users.Add(user);
//     // await db.SaveChangesAsync();
//     // return Results.Created($"/users/{user.Id}", user);
// });


// app.MapPost("/users", async (
//     CreateUserDto dto,
//     IUserService service) =>
// {
//     var result = await service.Create(dto);

//     return Results.Created($"/users/{result.Id}", result);
// })
// .AddEndpointFilter<ValidationFilter<CreateUserDto>>();

app.Run();

