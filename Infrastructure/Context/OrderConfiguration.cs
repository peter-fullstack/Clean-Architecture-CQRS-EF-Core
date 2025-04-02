using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders"); // Sets the table name

            // Primary key
            builder.HasKey(o => o.Id);

            // Properties configuration
            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd(); // Auto-generate IDs

            builder.Property(o => o.CustomerId)
                .IsRequired();

            builder.Property(o => o.OrderDate)
                .IsRequired()
                .HasColumnType("datetime2"); // For SQL Server

            // Navigation property (one-to-many with OrderItem)
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId") // Shadow property for FK
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade); // Delete items when order is deleted

            // Ignore computed properties that shouldn't be mapped
            builder.Ignore(o => o.OrderTotal);
        }
    }
}