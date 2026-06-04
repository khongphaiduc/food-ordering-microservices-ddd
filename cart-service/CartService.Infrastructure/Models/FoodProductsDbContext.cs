using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cart_service.CartService.Infrastructure.Models;

public partial class FoodProductsDbContext : DbContext
{
    public FoodProductsDbContext()
    {
    }

    public FoodProductsDbContext(DbContextOptions<FoodProductsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDiscount> CartDiscounts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=food_cart2;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carts_pkey");

            entity.ToTable("carts");

            entity.HasIndex(e => e.UserId, "uq_cart_user_active")
                .IsUnique()
                .HasFilter("((status)::text = 'ACTIVE'::text)");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'ACTIVE'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<CartDiscount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_discounts_pkey");

            entity.ToTable("cart_discounts");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AppliedAmount)
                .HasPrecision(12, 2)
                .HasColumnName("applied_amount");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(20)
                .HasColumnName("discount_type");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(12, 2)
                .HasColumnName("discount_value");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDiscounts)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("fk_cart_discount");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cart_items_pkey");

            entity.ToTable("cart_items");

            entity.HasIndex(e => new { e.CartId, e.ProductId, e.VariantId }, "uq_cart_item_unique").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(12, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(12, 2)
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");
            entity.Property(e => e.VariantName)
                .HasMaxLength(255)
                .HasColumnName("variant_name");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("fk_cart_items_cart");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
