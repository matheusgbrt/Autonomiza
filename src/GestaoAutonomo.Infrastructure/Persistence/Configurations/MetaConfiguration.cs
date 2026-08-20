using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class MetaConfiguration : IEntityTypeConfiguration<Meta>
{
    public void Configure(EntityTypeBuilder<Meta> builder)
    {
        builder.ToTable("Metas");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(m => m.ValorAlvo).HasPrecision(18, 2);

        builder.HasIndex(m => m.UsuarioId);
    }
}
