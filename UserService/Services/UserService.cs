using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Exceptions;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    readonly UserDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(
        UserDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> Create(CreateUserDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            Name = dto.Name  
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

    }

    public async Task<List<UserResponseDto>> GetAll()
    {
        return await _db.Users
        .Select(x => new UserResponseDto
        {
         Id = x.Id,
         Email = x.Email,
         Name = x.Name   
        }).ToListAsync();
    }

    public async Task<UserResponseDto?> GetById(int id)
    {
        var user = await _db.Users
        .Select(x => new UserResponseDto
        {
            Id = x.Id,
            Email = x.Email,
            Name = x.Name
        })
        .FirstOrDefaultAsync(x => x.Id == id);

    if (user is null)
        throw new NotFoundException($"User with id '{id}' was not found.");

    return user;

    }

    public async Task<UserResponseDto> LoginAsync(LoginUserDto loginUserDto)
    {
        var normalizedEmail = loginUserDto.Email.Trim().ToLowerInvariant();
        var user = await _db.Users
        .FirstOrDefaultAsync(x => x.Email == normalizedEmail);

        if (user is null)
            throw new BadRequestException("Invalid email or password.");

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            loginUserDto.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid email or password.");
        }


        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var normalizedEmail = registerUserDto.Email.Trim().ToLowerInvariant();

        var emailExists = await _db.Users
            .AnyAsync(user => user.Email == normalizedEmail);
        
        if(emailExists)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = registerUserDto.Name.Trim(),
            Email = normalizedEmail
        };

        user.PasswordHash = _passwordHasher.HashPassword(
        user,
        registerUserDto.Password);

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}