using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.SenhaHash).IsRequired();
        builder.Property(u => u.Plano).HasConversion<string>().HasMaxLength(20);

        builder.Property(u => u.ZApiInstanceId).HasMaxLength(100);
        builder.Property(u => u.ZApiToken).HasMaxLength(200);
        builder.Property(u => u.ZApiClientToken).HasMaxLength(200);
        builder.Property(u => u.WhatsAppMensagemBoasVindas).HasMaxLength(1000);
        builder.Property(u => u.WhatsAppRespostasAutomaticasAtivas).HasDefaultValue(true);
        builder.Property(u => u.WhatsAppHorariosDisponiveisAtivo).HasDefaultValue(true);
        builder.Property(u => u.WhatsAppConfirmarAgendamentosAtivo).HasDefaultValue(true);
        builder.Property(u => u.WhatsAppLembretesAutomaticosAtivos).HasDefaultValue(true);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.ZApiInstanceId).IsUnique().HasFilter("\"ZApiInstanceId\" IS NOT NULL");
    }
}
