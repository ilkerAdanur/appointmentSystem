using UserService.Dtos;

namespace UserService.Services;

public interface IUserService 
{
    Task<List<UserResponseDto>> GetAll();
    Task<UserResponseDto> Create(CreateUserDto dto);
    Task<UserResponseDto?> GetById(int id);
    Task<UserResponseDto> RegisterAsync(RegisterUserDto registerUserDto);
    Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task RevokeRefreshTokenAsync(RevokeRefreshTokenDto dto);
}
