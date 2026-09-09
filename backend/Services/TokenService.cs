using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class TokenService
{
    private readonly string _secret;

    public TokenService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"]
            ?? configuration["JWT_SECRET"]
            ?? throw new InvalidOperationException("Jwt:Secret não configurado.");
    }

    public (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim("organizacao_id", usuario.OrganizacaoId.ToString()),
            new Claim(ClaimTypes.Role, usuario.Role),
            new Claim(ClaimTypes.Email, usuario.Email),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(claims: claims, expires: expiraEm, signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
