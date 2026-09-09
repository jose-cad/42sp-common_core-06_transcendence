namespace Backend.DTOs;

public record RegistrarRequest(string Nome, string Email, string Senha, Guid OrganizacaoId);
public record LoginRequest(string Email, string Senha);
public record AuthResponse(string Token, DateTime ExpiraEm);
public record UsuarioResponse(Guid Id, string Nome, string Email, string Role, Guid OrganizacaoId);
