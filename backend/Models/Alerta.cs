namespace Backend.Models;

public class Alerta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid OrganizacaoId { get; set; }
    public required string Tipo { get; set; }
    public Guid? MotoristaId { get; set; }
    public Guid? CarroId { get; set; }
    public required string Mensagem { get; set; }
    public string Status { get; set; } = "pendente";
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? TratadoEm { get; set; }
    public Guid? TratadoPor { get; set; }

    public Organizacao? Organizacao { get; set; }
    public Motorista? Motorista { get; set; }
    public Carro? Carro { get; set; }
    public Usuario? UsuarioTratou { get; set; }
}
