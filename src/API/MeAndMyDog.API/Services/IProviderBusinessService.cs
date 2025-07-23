using MeAndMyDog.API.Models.DTOs.Provider;

namespace MeAndMyDog.API.Services;

/// <summary>
/// Service interface for provider business operations and analytics
/// </summary>
public interface IProviderBusinessService
{
    /// <summary>
    /// Gets comprehensive dashboard data for a service provider
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <returns>Complete dashboard data</returns>
    Task<ProviderDashboardDto> GetDashboardDataAsync(string providerId);
    
    /// <summary>
    /// Gets current business metrics for a service provider
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <returns>Current business metrics</returns>
    Task<BusinessMetricsDto> GetBusinessMetricsAsync(string providerId);
    
    /// <summary>
    /// Gets today's bookings for a service provider
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <returns>List of today's bookings</returns>
    Task<List<TodaysBookingDto>> GetTodaysBookingsAsync(string providerId);
    
    /// <summary>
    /// Gets recent invoices for a service provider
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="limit">Maximum number of invoices to return</param>
    /// <returns>List of recent invoices</returns>
    Task<List<RecentInvoiceDto>> GetRecentInvoicesAsync(string providerId, int limit = 10);
    
    /// <summary>
    /// Gets quick statistics for the current period
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <returns>Quick statistics</returns>
    Task<QuickStatsDto> GetQuickStatsAsync(string providerId);
    
    /// <summary>
    /// Gets revenue trend data for charts
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="months">Number of months to include</param>
    /// <returns>Revenue trend data</returns>
    Task<RevenueTrendDto> GetRevenueTrendAsync(string providerId, int months = 6);
    
    /// <summary>
    /// Gets recent client messages
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="limit">Maximum number of messages to return</param>
    /// <returns>List of recent messages</returns>
    Task<List<RecentMessageDto>> GetRecentMessagesAsync(string providerId, int limit = 5);
    
    /// <summary>
    /// Creates a new invoice
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="createInvoiceDto">Invoice creation data</param>
    /// <returns>Invoice creation response</returns>
    Task<CreateInvoiceResponseDto> CreateInvoiceAsync(string providerId, CreateInvoiceDto createInvoiceDto);
    
    /// <summary>
    /// Gets detailed invoice information
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="invoiceId">The invoice ID</param>
    /// <returns>Detailed invoice information</returns>
    Task<InvoiceDetailsDto?> GetInvoiceDetailsAsync(string providerId, string invoiceId);
    
    /// <summary>
    /// Updates invoice status
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="invoiceId">The invoice ID</param>
    /// <param name="status">New status</param>
    /// <param name="paymentMethod">Payment method if paid</param>
    /// <param name="paymentReference">Payment reference if paid</param>
    /// <returns>Success status</returns>
    Task<bool> UpdateInvoiceStatusAsync(string providerId, string invoiceId, string status, string? paymentMethod = null, string? paymentReference = null);
    
    /// <summary>
    /// Calculates and updates business metrics for a service provider
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <param name="period">Period to calculate (YYYY-MM format)</param>
    /// <returns>Success status</returns>
    Task<bool> CalculateBusinessMetricsAsync(string providerId, string period);
    
    /// <summary>
    /// Gets list of clients for invoice creation
    /// </summary>
    /// <param name="providerId">The service provider ID</param>
    /// <returns>List of clients</returns>
    Task<List<ClientDto>> GetClientsAsync(string providerId);
}

/// <summary>
/// DTO for client information in invoice creation
/// </summary>
public class ClientDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}