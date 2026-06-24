namespace AppointmentService.Models;

public class Appointment
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; } = "";
    public int UserId { get; set; }
    
}