using BlazorSchedulerApp.Models;
using System.Net.Http.Json;

namespace BlazorSchedulerApp.Services;

public class AppointmentService
{
    private readonly HttpClient _httpClient;

    public AppointmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves all appointments from the API
    /// </summary>
    public async Task<List<AppointmentData>> GetAppointmentsAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<AppointmentData>>("/api/appointments");
            return result ?? new List<AppointmentData>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointments: {ex.Message}");
            return new List<AppointmentData>();
        }
    }

    /// <summary>
    /// Retrieves a single appointment by ID
    /// </summary>
    public async Task<AppointmentData?> GetAppointmentByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AppointmentData>($"/api/appointments/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching appointment: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Creates a new appointment via the API
    /// </summary>
    public async Task<AppointmentData?> CreateAppointmentAsync(AppointmentData appointment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/appointments", appointment);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AppointmentData>();
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating appointment: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Updates an existing appointment via the API
    /// </summary>
    public async Task<bool> UpdateAppointmentAsync(int id, AppointmentData appointment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/appointments/{id}", appointment);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating appointment: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Deletes an appointment via the API
    /// </summary>
    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/appointments/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting appointment: {ex.Message}");
            return false;
        }
    }
}