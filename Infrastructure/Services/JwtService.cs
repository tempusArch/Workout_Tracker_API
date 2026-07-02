using WorkoutTrackerApi.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;


namespace WorkoutTrackerApi.Infrastructure;

public class JwtService {
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config) {       
        _config = config;
    }

    public string Generate_JWT(User um) {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, um.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, um.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );                                                                              

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken Generate_RefreshToken(string UserId) {
        return new RefreshToken {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = UserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(14)
        };
    }
}