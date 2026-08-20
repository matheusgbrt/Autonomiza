using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class RecomendacaoIAConfiguration : IEntityTypeConfiguration<RecomendacaoIA>
{
    public void Configure(EntityTypeBuilder<RecomendacaoIA> builder)
    {
        builder.ToTable("RecomendacoesIA");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Categoria).HasConversion<string>().HasMaxLength(30);
        builder.Property(r => r.Mensagem).IsRequired().HasMaxLength(2000);

        builder.HasIndex(r => r.UsuarioId);
    }
}
