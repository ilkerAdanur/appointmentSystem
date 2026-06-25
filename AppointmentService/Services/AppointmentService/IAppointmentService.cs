using AppointmentService.Dtos;
using AppointmentService.Models;

namespace AppointmentService.Services;
public interface IAppointmentService
{
    Task<AppointmentDetailsDto?> GetAppointmentDetails(int id);
    Task<List<AppointmentResponseDto>> GetAll();
    Task<AppointmentResponseDto?> GetById(int id);
    Task<AppointmentResponseDto> Create(CreateAppointmentDto dto);
}