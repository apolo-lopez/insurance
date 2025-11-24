using Evaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.Infrastructure.Data.Configuration
{
    /// <summary>
    /// Configures the entity mapping for the Client type in the Entity Framework model.
    /// </summary>
    /// <remarks>This configuration defines table mapping, property constraints, and relationships for the
    /// Client entity, including required fields, maximum lengths, and unique indexes.</remarks>
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");
            builder.HasKey(c => c.Id);

            // IdentificationNumber as owned entity, add max length and required constraint
            builder.OwnsOne(c => c.IdentificationNumber, id =>
            {
                id.Property(i => i.Value)
                  .HasColumnName("IdentificationNumber")
                  .HasMaxLength(10)
                  .IsRequired();
                id.HasIndex(i => i.Value).IsUnique(); // el índice va aquí
            });

            builder.Property(c => c.Name)
                   .HasMaxLength(200)
                   .IsRequired();
            builder.Property(c => c.Email)
                .HasMaxLength(250)
                     .IsRequired();
            builder.Property(c => c.PhoneNumber)
                     .HasMaxLength(15)
                     .IsRequired();
            builder.Property(c => c.Address)
                        .HasMaxLength(500);
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt);
        }
    }
}
