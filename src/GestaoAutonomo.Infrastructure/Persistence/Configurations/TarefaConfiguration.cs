using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("Tarefas");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Titulo).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Descricao).HasMaxLength(2000);

        builder.HasIndex(t => t.UsuarioId);
    }
}
