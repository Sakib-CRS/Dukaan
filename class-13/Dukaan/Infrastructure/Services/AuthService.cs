using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dukaan.Application.Dtos;
using Dukaan.Application.Interfaces;
using Dukaan.Infrastructure.Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Dukaan.Infrastructure.Services;

/// <summary>
/// Service responsible for handling auth-related business logic.
/// </summary>
/// <remarks>
/// This service help to login the merchant
/// </remarks>
public class AuthService : IAuthService
{
    private readonly UserManager<Merchant> _userManager;
    private readonly IConfiguration _configuration;
    public AuthService(UserManager<Merchant> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    /// <summary>
    /// Login a  merchant  with their store (tenant).
    /// </summary>
    /// <param name="request">The login details.</param>
    /// <returns>A response containing the token.</returns>
    /// <exception cref="Exception">Thrown when merchant login fails.</exception>
    /// <remarks>
    /// This method demonstrates a composite operation: 
    /// 1. Verify the login information
    /// 2. Generate token
    /// </remarks>
    public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invaild credentials");
        }

        var isVaild = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isVaild)
        {
            throw new UnauthorizedAccessException("Invaild credentials");
        }

        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("tenant_id", user.TenantId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256Signature
                )
        };
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        var jwt = handler.WriteToken(token);
        var readToken = handler.ReadJwtToken(jwt);
        var expiration = readToken.ValidTo;

        return new AuthResponseDTO(jwt, expiration);
    }
}
