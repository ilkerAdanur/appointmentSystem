namespace AppointmentService.Dtos;
public class CreateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; } = "";
    public int UserId { get; set; }
}