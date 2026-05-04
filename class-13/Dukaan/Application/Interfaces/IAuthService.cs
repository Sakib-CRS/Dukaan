using Dukaan.Application.Dtos;

namespace Dukaan.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);
}
