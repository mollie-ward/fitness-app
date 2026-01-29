using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FitnessApp.IntegrationTests.Helpers;

/// <summary>
/// Helper class to generate JWT tokens for integration tests
/// </summary>
public static class JwtTokenHelper
{
    private const string Secret = "ThisIsATestSecretKeyForJWTTokenGenerationPurposes123456"; // At least 32 characters
    private const string Issuer = "FitnessApp.TestServer";
    private const string Audience = "FitnessApp.TestClient";

    /// <summary>
    /// Generates a JWT token for a test user
    /// </summary>
    /// <param name="userId">The user ID to include in the token</param>
    /// <returns>JWT token string</returns>
    public static string GenerateToken(Guid userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("sub", userId.ToString()),
                new Claim("userId", userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
