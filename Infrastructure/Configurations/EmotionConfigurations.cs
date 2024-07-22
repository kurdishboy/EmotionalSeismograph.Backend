using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EmotionConfigurations : IEntityTypeConfiguration<Emotion>
{
    public void Configure(EntityTypeBuilder<Emotion> builder)
    {
        builder.ToTable("Emotions");
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();
    }
}