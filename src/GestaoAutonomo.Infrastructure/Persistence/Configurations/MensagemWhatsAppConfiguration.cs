using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class MensagemWhatsAppConfiguration : IEntityTypeConfiguration<MensagemWhatsApp>
{
    public void Configure(EntityTypeBuilder<MensagemWhatsApp> builder)
    {
        builder.ToTable("MensagensWhatsApp");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Telefone).IsRequired().HasMaxLength(30);
        builder.Property(m => m.Direcao).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Conteudo).IsRequired().HasMaxLength(4000);

        builder.HasIndex(m => new { m.UsuarioId, m.CreatedAt });
        builder.HasIndex(m => new { m.UsuarioId, m.Telefone });

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(m => m.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
