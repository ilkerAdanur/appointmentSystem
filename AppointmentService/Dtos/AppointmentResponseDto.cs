namespace AppointmentService.Dtos;
public class AppointmentResponseDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; } ="";
}