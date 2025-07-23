namespace MeAndMyDog.API.Models.DTOs.Provider;

/// <summary>
/// DTO for provider upgrade request - mirrors registration wizard
/// </summary>
public class ProviderUpgradeRequestDto
{
    /// <summary>
    /// Business name (required)
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;
    
    /// <summary>
    /// Business type (Individual/Company)
    /// </summary>
    public string? BusinessType { get; set; }
    
    /// <summary>
    /// Company number (optional)
    /// </summary>
    public string? CompanyNumber { get; set; }
    
    /// <summary>
    /// VAT number (optional)
    /// </summary>
    public string? VatNumber { get; set; }
    
    /// <summary>
    /// Business address line 1
    /// </summary>
    public string? AddressLine1 { get; set; }
    
    /// <summary>
    /// Business address line 2
    /// </summary>
    public string? AddressLine2 { get; set; }
    
    /// <summary>
    /// Business city
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Business county
    /// </summary>
    public string? County { get; set; }
    
    /// <summary>
    /// Business postcode
    /// </summary>
    public string PostCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Services to offer - same structure as registration
    /// </summary>
    public List<ServiceProviderUpgradeDto> Services { get; set; } = new();
    
    /// <summary>
    /// User agrees to provider terms and conditions
    /// </summary>
    public bool AgreeToProviderTerms { get; set; }
}