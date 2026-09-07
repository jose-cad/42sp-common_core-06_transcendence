namespace Backend.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid OrganizacaoId { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string SenhaHash { get; set; }
    public string Role { get; set; } = "operador";
    public string? AvatarUrl { get; set; }
    public bool Online { get; set; } = false;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Organizacao? Organizacao { get; set; }
}
