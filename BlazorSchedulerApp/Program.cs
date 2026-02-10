using BlazorSchedulerApp.Components;
using BlazorSchedulerApp.Models;
using BlazorSchedulerApp.Services;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for AppointmentService
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(navigationManager.BaseUri)
    };
});
builder.Services.AddScoped<AppointmentService>();

// Add Syncfusion Blazor service
builder.Services.AddSyncfusionBlazor();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// In-memory data storage for appointments
var appointments = new List<AppointmentData>
{
    new AppointmentData
    {
        Id = 1,
        Subject = "Team Meeting",
        Location = "Conference Room A",
        StartTime = DateTime.Today.AddHours(10),
        EndTime = DateTime.Today.AddHours(11),
        Description = "Weekly team sync-up meeting"
    },
    new AppointmentData
    {
        Id = 2,
        Subject = "Client Presentation",
        Location = "Boardroom",
        StartTime = DateTime.Today.AddDays(1).AddHours(14),
        EndTime = DateTime.Today.AddDays(1).AddHours(16),
        Description = "Quarterly review with client"
    },
    new AppointmentData
    {
        Id = 3,
        Subject = "Project Planning",
        Location = "Room 301",
        StartTime = DateTime.Today.AddDays(2).AddHours(9),
        EndTime = DateTime.Today.AddDays(2).AddHours(10).AddMinutes(30),
        Description = "Planning session for new project"
    }
};

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseCors();

// ============= Minimal API Endpoints for Appointments =============

// GET: Get all appointments
app.MapGet("/api/appointments", () =>
{
    return Results.Ok(appointments);
})
.WithName("GetAppointments");

// GET: Get appointment by ID
app.MapGet("/api/appointments/{id}", (int id) =>
{
    var appointment = appointments.FirstOrDefault(a => a.Id == id);
    return appointment is not null ? Results.Ok(appointment) : Results.NotFound();
})
.WithName("GetAppointmentById");

// POST: Create new appointment
app.MapPost("/api/appointments", (AppointmentData appointment) =>
{
    appointment.Id = appointments.Any() ? appointments.Max(a => a.Id) + 1 : 1;
    appointments.Add(appointment);
    return Results.Created($"/api/appointments/{appointment.Id}", appointment);
})
.WithName("CreateAppointment");

// PUT: Update appointment
app.MapPut("/api/appointments/{id}", (int id, AppointmentData updatedAppointment) =>
{
    var appointment = appointments.FirstOrDefault(a => a.Id == id);
    if (appointment is null)
        return Results.NotFound();

    appointment.Subject = updatedAppointment.Subject;
    appointment.Location = updatedAppointment.Location;
    appointment.StartTime = updatedAppointment.StartTime;
    appointment.EndTime = updatedAppointment.EndTime;
    appointment.Description = updatedAppointment.Description;
    appointment.IsAllDay = updatedAppointment.IsAllDay;
    appointment.RecurrenceRule = updatedAppointment.RecurrenceRule;
    appointment.RecurrenceID = updatedAppointment.RecurrenceID;
    appointment.RecurrenceException = updatedAppointment.RecurrenceException;

    return Results.Ok(appointment);
})
.WithName("UpdateAppointment");

// DELETE: Delete appointment
app.MapDelete("/api/appointments/{id}", (int id) =>
{
    var appointment = appointments.FirstOrDefault(a => a.Id == id);
    if (appointment is null)
        return Results.NotFound();

    appointments.Remove(appointment);
    return Results.NoContent();
})
.WithName("DeleteAppointment");

 // ============= End of Minimal API Endpoints =============

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();