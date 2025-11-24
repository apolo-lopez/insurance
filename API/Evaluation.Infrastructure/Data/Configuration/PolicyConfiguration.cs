using Evaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluation.Infrastructure.Data.Configuration
{
    public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.ToTable("Policies");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PolicyNumber).IsRequired().HasMaxLength(100);
            builder.HasIndex(p => p.PolicyNumber).IsUnique();

            // Enum properties stored as integers
            builder.Property(p => p.PolicyType).HasConversion<int>().IsRequired();
            builder.Property(p => p.PolicyStatus).HasConversion<int>().IsRequired();

            builder.Property(p => p.StartDate).IsRequired();
            builder.Property(p => p.EndDate).IsRequired();

            // Using decimal(18,2) for monetary values
            builder.Property(p => p.InsuredAmount).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);

            // Relationship with Client, delete everything if client is deleted
            builder.HasOne<Client>()
                   .WithMany()
                   .HasForeignKey(p => p.ClientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
