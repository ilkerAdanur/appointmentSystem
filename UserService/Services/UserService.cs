using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Dtos;
using UserService.Exceptions;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    readonly UserDbContext _db;

    public UserService(UserDbContext db)
    {
        _db = db;
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
}