namespace Backend.Models;

public class Arquivo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Hash { get; set; }
    public required string Tipo { get; set; }
    public required string Caminho { get; set; }
    public Guid? MotoristaId { get; set; }
    public Guid? CarroId { get; set; }
    public DateTime EnviadoEm { get; set; } = DateTime.UtcNow;

    public Motorista? Motorista { get; set; }
    public Carro? Carro { get; set; }
}
