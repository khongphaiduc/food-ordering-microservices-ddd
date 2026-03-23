using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace tracking_service.Tracking.Infastructrure.Models;

public partial class FoodProductsDbContext : DbContext
{
    public FoodProductsDbContext()
    {
    }

    public FoodProductsDbContext(DbContextOptions<FoodProductsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TrackingEvent> TrackingEvents { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=food_tracking_db;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackingEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tracking_events_pkey");

            entity.ToTable("tracking_events");

            entity.HasIndex(e => e.EventType, "idx_tracking_event");

            entity.HasIndex(e => e.ProductId, "idx_tracking_product");

            entity.HasIndex(e => e.UserId, "idx_tracking_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .HasColumnName("event_type");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Session).WithMany(p => p.TrackingEvents)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("tracking_events_session_id_fkey");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_sessions_pkey");

            entity.ToTable("user_sessions");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("started_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
