using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class LancamentoFinanceiroConfiguration : IEntityTypeConfiguration<LancamentoFinanceiro>
{
    public void Configure(EntityTypeBuilder<LancamentoFinanceiro> builder)
    {
        builder.ToTable("LancamentosFinanceiros");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Categoria).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Valor).HasPrecision(18, 2);
        builder.Property(l => l.Descricao).HasMaxLength(2000);

        builder.HasIndex(l => l.UsuarioId);
        builder.HasIndex(l => new { l.UsuarioId, l.Data });

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(l => l.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Agendamento>()
            .WithMany()
            .HasForeignKey(l => l.AgendamentoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
