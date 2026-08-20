using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Observacoes).HasMaxLength(2000);

        builder.HasIndex(a => a.UsuarioId);
        builder.HasIndex(a => new { a.UsuarioId, a.DataHoraInicio, a.DataHoraFim });

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Servico>()
            .WithMany()
            .HasForeignKey(a => a.ServicoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
