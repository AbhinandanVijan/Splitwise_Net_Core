using System;
using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace Splitwise.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseShare> ExpenseShares => Set<ExpenseShare>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membership>().HasKey(m => new { m.GroupId, m.UserId });
        modelBuilder.Entity<ExpenseShare>().HasKey(s => new { s.ExpenseId, s.UserId });

        modelBuilder.Entity<Group>()
            .HasMany(g => g.Memberships)
            .WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Memberships)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Expenses)
            .HasForeignKey(e => e.GroupId);

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Payer)
            .WithMany()
            .HasForeignKey(e => e.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseShare>()
            .HasOne(s => s.Expense)
            .WithMany(e => e.Shares)
            .HasForeignKey(s => s.ExpenseId);

        modelBuilder.Entity<ExpenseShare>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Group)
            .WithMany()
            .HasForeignKey(t => t.GroupId);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Sender)
            .WithMany()
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Receiver)
            .WithMany()
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optimistic concurrency (Postgres xmin)
        modelBuilder.Entity<Expense>().UseXminAsConcurrencyToken();
        modelBuilder.Entity<Transaction>().UseXminAsConcurrencyToken();
    }
}
