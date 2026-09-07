using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organizacao> Organizacoes => Set<Organizacao>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Carro> Carros => Set<Carro>();
    public DbSet<Arquivo> Arquivos => Set<Arquivo>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<HistoricoKm> HistoricosKm => Set<HistoricoKm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Motorista>().HasIndex(m => m.Cpf).IsUnique();
        modelBuilder.Entity<Carro>().HasIndex(c => c.Placa).IsUnique();
        modelBuilder.Entity<Arquivo>().HasIndex(a => a.Hash).IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Organizacao).WithMany()
            .HasForeignKey(u => u.OrganizacaoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Motorista>()
            .HasOne(m => m.Organizacao).WithMany()
            .HasForeignKey(m => m.OrganizacaoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Carro>()
            .HasOne(c => c.Organizacao).WithMany()
            .HasForeignKey(c => c.OrganizacaoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Carro>()
            .HasOne(c => c.Motorista).WithMany()
            .HasForeignKey(c => c.MotoristaId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Arquivo>()
            .HasOne(a => a.Motorista).WithMany()
            .HasForeignKey(a => a.MotoristaId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Arquivo>()
            .HasOne(a => a.Carro).WithMany()
            .HasForeignKey(a => a.CarroId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.Organizacao).WithMany()
            .HasForeignKey(a => a.OrganizacaoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.Motorista).WithMany()
            .HasForeignKey(a => a.MotoristaId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.Carro).WithMany()
            .HasForeignKey(a => a.CarroId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Alerta>()
            .HasOne(a => a.UsuarioTratou).WithMany()
            .HasForeignKey(a => a.TratadoPor).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HistoricoKm>()
            .HasOne(h => h.Carro).WithMany()
            .HasForeignKey(h => h.CarroId).OnDelete(DeleteBehavior.Restrict);
    }
}
