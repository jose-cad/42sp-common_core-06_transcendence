namespace Backend.Models;

public class Organizacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Nome { get; set; }
    public string? Cnpj { get; set; }
    public string? Endereco { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
