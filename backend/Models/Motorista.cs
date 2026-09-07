namespace Backend.Models;

public class Motorista
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid OrganizacaoId { get; set; }
    public required string Nome { get; set; }
    public required string Cpf { get; set; }
    public string? Rg { get; set; }
    public required string Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Organizacao? Organizacao { get; set; }
}
