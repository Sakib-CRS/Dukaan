using Microsoft.AspNetCore.Mvc;

namespace Dukaan.Host.Controllers;

/// <summary>
/// Controller for managing Auth for merchant
/// </summary>
/// <remarks>
/// This controller serves as the entry point for auth-related API calls.
/// It delegates business logic to the 
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Endpoint for login a new merchant and store.
    /// </summary>
    /// <param name="request">The login data</param>
    /// <returns>The login response.</returns>
    /// <response code="200">Returns the token informations.</response>
    public async Task<ActionResult> Login()
    {
        // TODO: Get response from AuthService for login
        return Ok();
    }
}
