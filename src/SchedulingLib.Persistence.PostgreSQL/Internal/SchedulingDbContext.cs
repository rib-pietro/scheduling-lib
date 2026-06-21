using Microsoft.EntityFrameworkCore;
using SchedulingLib.Persistence.PostgreSQL.Internal.Rows;

namespace SchedulingLib.Persistence.PostgreSQL.Internal;

internal sealed class SchedulingDbContext(DbContextOptions<SchedulingDbContext> options) : DbContext(options)
{
    internal DbSet<StaffMemberRow> StaffMembers => Set<StaffMemberRow>();
    internal DbSet<ServiceAppointmentRow> ServiceAppointments => Set<ServiceAppointmentRow>();
    internal DbSet<ServiceTypeRow> ServiceTypes => Set<ServiceTypeRow>();
    internal DbSet<UserRow> Users => Set<UserRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRow>(b =>
        {
            b.ToTable("users");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Name).HasColumnName("name");
            b.Property(r => r.Email).HasColumnName("email");
            b.Property(r => r.Phone).HasColumnName("phone");
            b.Property(r => r.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<StaffMemberRow>(b =>
        {
            b.ToTable("staff_members");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Name).HasColumnName("name");
            b.Property(r => r.Email).HasColumnName("email");
            b.Property(r => r.ProfilePictureUrl).HasColumnName("profile_picture_url");
            b.Property(r => r.CreatedAt).HasColumnName("created_at");
            b.Property(r => r.ScheduleJson).HasColumnName("schedule").HasColumnType("jsonb");
            b.Property(r => r.OfferedServicesJson).HasColumnName("offered_services").HasColumnType("jsonb");
            b.Property(r => r.GalleryJson).HasColumnName("gallery").HasColumnType("jsonb");
        });

        modelBuilder.Entity<ServiceTypeRow>(b =>
        {
            b.ToTable("service_types");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Name).HasColumnName("name");
            b.Property(r => r.Price).HasColumnName("price").HasColumnType("numeric(18,4)");
            b.Property(r => r.DurationTicks).HasColumnName("duration_ticks");
        });

        modelBuilder.Entity<ServiceAppointmentRow>(b =>
        {
            b.ToTable("service_appointments");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Title).HasColumnName("title");
            b.Property(r => r.StaffMemberId).HasColumnName("staff_member_id");
            b.Property(r => r.ClientId).HasColumnName("client_id");
            b.Property(r => r.ServiceTypeId).HasColumnName("service_type_id");
            b.Property(r => r.Date).HasColumnName("date");
            b.Property(r => r.TimeSlotStart).HasColumnName("time_slot_start");
            b.Property(r => r.TimeSlotEnd).HasColumnName("time_slot_end");
            b.Property(r => r.Status).HasColumnName("status");
            b.Property(r => r.ExternalCalendarEventId).HasColumnName("external_calendar_event_id");
            b.Property(r => r.CreatedAt).HasColumnName("created_at");
        });
    }
}
