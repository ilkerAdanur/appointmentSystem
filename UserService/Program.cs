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
using Microsoft.IdentityModel.Tokens;
using UserService.Models;
using UserService.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/user-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddHealthChecks();

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

builder.Host.UseSerilog();

builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

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
})
.RequireAuthorization();

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

app.MapPost("/auth/login", async (
    LoginUserDto loggedInUser,
    IUserService userService) =>
{
    var loginResult = await userService.LoginAsync(loggedInUser);

    return Results.Ok(loginResult);
})
.AddEndpointFilter<ValidationFilter<LoginUserDto>>();

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

