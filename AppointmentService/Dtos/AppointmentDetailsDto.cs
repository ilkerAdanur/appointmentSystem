namespace AppointmentService.Dtos;
public class AppointmentDetailsDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; } = "";
    public int UserId { get; set; }
    public object? User { get; set; }
}