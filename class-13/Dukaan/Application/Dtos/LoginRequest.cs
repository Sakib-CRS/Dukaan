namespace Dukaan.Application.Dtos;

/// <summary>
/// Data Transfer Object (DTO) for login merchant.
/// </summary>
/// <remarks>
/// DTOs are used to define the contract between the API and its consumers.
/// They help in decoupling the external API structure from the internal Domain models.
/// </remarks>
public record LoginRequestDTO(
    string Email,
    string Password
);

/// <summary>
/// Response returned after a successful login.
/// </summary>
public record AuthResponseDTO(
    string Token,
    DateTime Expiration
);