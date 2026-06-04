using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace order_service.OrderService.Infrastructure.Models;

public partial class FoodOrderContext : DbContext
{
    public FoodOrderContext()
    {
    }

    public FoodOrderContext(DbContextOptions<FoodOrderContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDelivery> OrderDeliveries { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderPayment> OrderPayments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC073A21A3D9");

            entity.HasIndex(e => e.OrderCode, "UQ__Orders__999B52291A368D34").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderCode).HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).HasMaxLength(30);
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<OrderDelivery>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__OrderDel__C3905BCF084454EC");

            entity.ToTable("OrderDelivery");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ReceiverName).HasMaxLength(255);

            entity.HasOne(d => d.Order).WithOne(p => p.OrderDelivery)
                .HasForeignKey<OrderDelivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDelivery_Orders");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC074DF45E70");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.VariantName).HasMaxLength(255);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderPay__3214EC07BA1036D5");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentProvider).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TransactionId).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderPayments_Orders");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
