using SinglesourceApp.Models;

namespace SinglesourceApp.Services;

/// <summary>
/// Backend contract for property details. Replace <see cref="MockPropertyDetailsService"/> with an HTTP implementation
/// (same <see cref="PropertyDetailsDto"/> shape) when the API is available.
/// </summary>
public interface IPropertyDetailsService
{
    /// <summary>Intended: GET /properties/{propertyId}/details</summary>
    Task<PropertyDetailsDto> GetPropertyDetailsAsync(string propertyId, CancellationToken cancellationToken = default);
}
