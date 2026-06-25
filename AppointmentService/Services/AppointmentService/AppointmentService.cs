using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppointmentDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AppointmentService(AppointmentDbContext db, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<AppointmentResponseDto> Create(CreateAppointmentDto dto)
        {
            var userServiceUrl = _configuration["Services:UserService"];
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{userServiceUrl}/users/{dto.UserId}");

            if(!response.IsSuccessStatusCode)
                throw new Exception("User not found");

            var appointment = new Appointment
            {
                AppointmentDate = dto.AppointmentDate,
                Description = dto.Description,
                UserId = dto.UserId
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                AppointmentDate = appointment.AppointmentDate,
                Description = appointment.Description,
                UserId = appointment.UserId
            };
        }

    public async Task<List<AppointmentResponseDto>> GetAll()
    {
        return await _db.Appointments
        .Select(x => new AppointmentResponseDto
        {
            Id = x.Id,
            AppointmentDate = x.AppointmentDate,
            Description = x.Description,
            UserId = x.UserId
        }).ToListAsync();
    }
    
    public async Task<AppointmentDetailsDto?> GetAppointmentDetails(int id)
    {
        var appointment = await _db.Appointments
        .Where(x => x.Id == id)
        .Select(x => new AppointmentDetailsDto
        {   
            Id = x.Id,
            AppointmentDate = x.AppointmentDate,
            Description = x.Description,
            UserId = x.UserId            
        }
        ).FirstOrDefaultAsync();

        if (appointment is null)
                return null;

        var userServiceUrl = _configuration["Services:UserService"];
        var client = _httpClientFactory.CreateClient();

        var user = await client.GetFromJsonAsync<object>(
            $"{userServiceUrl}/users/{appointment.UserId}");

        appointment.User = user;

        return appointment;
        
    }

    public async Task<AppointmentResponseDto?> GetById(int id)
    {
        return await _db.Appointments
        .Where(x => x.Id == id)
        .Select(x => new AppointmentResponseDto
        {
            Id = x.Id,
            AppointmentDate = x.AppointmentDate,
            Description = x.Description,
            UserId = x.UserId
        }).FirstOrDefaultAsync();
        
    }
}