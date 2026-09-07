namespace Backend.Models;

public class Carro
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid OrganizacaoId { get; set; }
    public required string Modelo { get; set; }
    public required string Placa { get; set; }
    public Guid? MotoristaId { get; set; }
    public int KmSemanal { get; set; }
    public int KmTotal { get; set; }
    public int KmParaManutencao { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Organizacao? Organizacao { get; set; }
    public Motorista? Motorista { get; set; }
}
