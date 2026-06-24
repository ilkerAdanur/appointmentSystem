using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Data;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(
        DbContextOptions<AppointmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments => Set<Appointment>();
}