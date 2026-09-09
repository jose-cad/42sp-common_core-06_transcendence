using System.Security.Claims;
using System.Text.RegularExpressions;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<UsuarioResponse>> Registrar(RegistrarRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { erro = "Nome é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.Email) || !EmailRegex.IsMatch(request.Email))
            return BadRequest(new { erro = "E-mail inválido." });

        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 8)
            return BadRequest(new { erro = "Senha deve ter no mínimo 8 caracteres." });

        var organizacaoExiste = await _db.Organizacoes.AnyAsync(o => o.Id == request.OrganizacaoId);
        if (!organizacaoExiste)
            return BadRequest(new { erro = "Organização não encontrada." });

        if (await _db.Usuarios.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { erro = "E-mail já cadastrado." });

        var usuario = new Usuario
        {
            OrganizacaoId = request.OrganizacaoId,
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return Ok(new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Role, usuario.OrganizacaoId));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return Unauthorized(new { erro = "E-mail ou senha incorretos." });

        var (token, expiraEm) = _tokenService.GerarToken(usuario);
        return Ok(new AuthResponse(token, expiraEm));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioResponse>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null || !Guid.TryParse(userId, out var id))
            return Unauthorized();

        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound();

        return Ok(new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Role, usuario.OrganizacaoId));
    }
}
