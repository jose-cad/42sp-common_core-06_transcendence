namespace Backend.Models;

public class HistoricoKm
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CarroId { get; set; }
    public int KmRegistrado { get; set; }
    public required string Fonte { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Carro? Carro { get; set; }
}
