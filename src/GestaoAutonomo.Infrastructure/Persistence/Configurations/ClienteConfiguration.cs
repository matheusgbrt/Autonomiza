using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.Telefone).HasMaxLength(20);
        builder.Property(c => c.Observacoes).HasMaxLength(2000);

        builder.HasIndex(c => c.UsuarioId);
    }
}
