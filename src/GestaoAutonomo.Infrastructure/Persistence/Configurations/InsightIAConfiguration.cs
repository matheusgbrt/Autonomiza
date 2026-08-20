using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class InsightIAConfiguration : IEntityTypeConfiguration<InsightIA>
{
    public void Configure(EntityTypeBuilder<InsightIA> builder)
    {
        builder.ToTable("InsightsIA");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Categoria).HasConversion<string>().HasMaxLength(30);
        builder.Property(i => i.Mensagem).IsRequired().HasMaxLength(2000);

        builder.HasIndex(i => i.UsuarioId);
    }
}
