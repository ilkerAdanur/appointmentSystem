using UserService.Dtos;

namespace UserService.Services;

public interface IUserService 
{
    Task<List<UserResponseDto>> GetAll();
    Task<UserResponseDto> Create(CreateUserDto dto);
    Task<UserResponseDto?> GetById(int id);
}
