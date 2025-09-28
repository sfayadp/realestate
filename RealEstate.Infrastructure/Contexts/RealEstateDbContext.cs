using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Models.RealEstate;

namespace RealEstate.Infrastructure.Contexts;

public partial class RealEstateDbContext : DbContext
{
    public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Owner> Owner { get; set; }

    public virtual DbSet<Property> Property { get; set; }

    public virtual DbSet<PropertyImage> PropertyImage { get; set; }

    public virtual DbSet<PropertyTrace> PropertyTrace { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.IdOwner);

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.IdProperty);

            entity.HasIndex(e => e.IdOwner, "IX_Property_IdOwner");

            entity.HasIndex(e => e.CodeInternal, "UQ_Property_CodeInternal").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CodeInternal).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdOwnerNavigation).WithMany(p => p.Property)
                .HasForeignKey(d => d.IdOwner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Property_Owner");
        });

        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(e => e.IdPropertyImage);

            entity.HasIndex(e => e.IdProperty, "IX_PropertyImage_IdProperty");

            entity.Property(e => e.Enabled).HasDefaultValue(true);

            entity.HasOne(d => d.IdPropertyNavigation).WithMany(p => p.PropertyImage)
                .HasForeignKey(d => d.IdProperty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyImage_Property");
        });

        modelBuilder.Entity<PropertyTrace>(entity =>
        {
            entity.HasKey(e => e.IdPropertyTrace);

            entity.HasIndex(e => new { e.IdProperty, e.DateSale }, "IX_PropertyTrace_IdProperty_Date");

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdPropertyNavigation).WithMany(p => p.PropertyTrace)
                .HasForeignKey(d => d.IdProperty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PropertyTrace_Property");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
