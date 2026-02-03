using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Base controller with shared functionality
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    /// <returns>The authenticated user's ID</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID cannot be extracted from token</exception>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
}
