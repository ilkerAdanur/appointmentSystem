using AppointmentService.Configuration;
using AppointmentService.Data;
using AppointmentService.Dtos;
using AppointmentService.Exceptions;
using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AppointmentService.Services;


public class AppointmentService : IAppointmentService
{
    private readonly ServiceUrls _services;
    private readonly AppointmentDbContext _db;
    private readonly IHttpClientFactory _httpClientFactory;

    public AppointmentService(
                AppointmentDbContext db,
                IHttpClientFactory httpClientFactory, 
                IOptions<ServiceUrls> serviceOptions)
    {
        _db = db;
        _httpClientFactory = httpClientFactory;
        _services = serviceOptions.Value;
    }

    public async Task<AppointmentResponseDto> Create(CreateAppointmentDto dto)
        {
            var userServiceUrl = _services.UserService;
            var client = _httpClientFactory.CreateClient("UserServiceClient");

            try
            {
                var response = await client.GetAsync($"{userServiceUrl}/users/{dto.UserId}");

                if(!response.IsSuccessStatusCode)
                    throw new BadRequestException("User not found");
            }
            catch (HttpRequestException)
            {
                throw new ServiceUnavailableException("UserService is currently unavailable.");
            }

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

        var userServiceUrl = _services.UserService;
        var client = _httpClientFactory.CreateClient("UserServiceClient");

        try
        {
        var user = await client.GetFromJsonAsync<object>(
            $"{userServiceUrl}/users/{appointment.UserId}");
            appointment.User = user;
        }
        catch (HttpRequestException)
        {
            throw new ServiceUnavailableException("UserService is currently unavailable.");
        }

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