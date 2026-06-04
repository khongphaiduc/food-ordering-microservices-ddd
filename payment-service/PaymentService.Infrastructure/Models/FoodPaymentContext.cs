using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace payment_service.PaymentService.Infrastructure.Models;

public partial class FoodPaymentContext : DbContext
{
    public FoodPaymentContext()
    {
    }

    public FoodPaymentContext(DbContextOptions<FoodPaymentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=PHAMTRUNGDUC\\SQLEXPRESS;Initial Catalog=food_payment;Persist Security Info=True;User ID=sa;Password=123;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC075A0B52BC");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TransactionId).HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentT__3214EC075ED04D2B");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ProviderTransactionId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Payment).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentTransactions_Payments");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Refunds__3214EC072E1FDF02");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProviderRefundId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Refunds_Payments");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
