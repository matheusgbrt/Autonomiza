using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.ToTable("Servicos");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Descricao).HasMaxLength(2000);
        builder.Property(s => s.ValorPadrao).HasPrecision(18, 2);

        builder.HasIndex(s => s.UsuarioId);
    }
}
