using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace EFCore;

public static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}

public partial class TTContext : DbContext
{
    public TTContext()
    {

    }

    public TTContext(DbContextOptions<TTContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotEnv.Load("../../../SensitiveInfo.env");
        string connectionString = $"Host={Environment.GetEnvironmentVariable("Host")};" +
            $"Database={Environment.GetEnvironmentVariable("Database")};" +
            $"Username={Environment.GetEnvironmentVariable("Username")};" +
            $"Password={Environment.GetEnvironmentVariable("Password")}";
        optionsBuilder.UseNpgsql(connectionString);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Taskid).HasName("task_pkey");

            entity.ToTable("task");

            entity.Property(e => e.Taskid).HasColumnName("taskid");
            entity.Property(e => e.Completed)
                .HasDefaultValueSql("false")
                .HasColumnName("completed");
            entity.Property(e => e.Dateofcreation).HasColumnName("dateofcreation");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("''::text")
                .HasColumnName("description");
            entity.Property(e => e.Executiontime)
                .HasDefaultValueSql("'00:00:01'::interval")
                .HasColumnName("executiontime");
            entity.Property(e => e.Expectedfinishtime).HasColumnName("expectedfinishtime");
            entity.Property(e => e.Finishtime)
                .HasDefaultValueSql("'0001-01-01'::date")
                .HasColumnName("finishtime");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Useridref).HasColumnName("useridref");

            entity.HasOne(d => d.UseridrefNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.Useridref)
                .HasConstraintName("task_useridref_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Email, "user_email_key").IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Coinsamount)
                .HasDefaultValueSql("0")
                .HasColumnName("coinsamount");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
