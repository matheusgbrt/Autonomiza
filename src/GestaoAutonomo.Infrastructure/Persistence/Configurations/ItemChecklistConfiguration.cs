using GestaoAutonomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAutonomo.Infrastructure.Persistence.Configurations;

public class ItemChecklistConfiguration : IEntityTypeConfiguration<ItemChecklist>
{
    public void Configure(EntityTypeBuilder<ItemChecklist> builder)
    {
        builder.ToTable("ItensChecklist");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Descricao).IsRequired().HasMaxLength(500);

        builder.HasIndex(i => i.TarefaId);

        builder.HasOne<Tarefa>()
            .WithMany()
            .HasForeignKey(i => i.TarefaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
