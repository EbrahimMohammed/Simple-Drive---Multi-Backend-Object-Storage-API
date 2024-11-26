using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<BlobTracking> BlobTrackings { get; set; }
        public DbSet<BlobData> BlobData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Table name
                entity.HasKey(u => u.Id); // Primary Key

                entity.Property(u => u.UserName)
                      .IsRequired()
                      .HasMaxLength(100); // Constraint: Username max length

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()"); // Default timestamp
            });


            modelBuilder.Entity<BlobTracking>(entity =>
            {
                entity.ToTable("BlobTracking");

                entity.Property(b => b.Id)
                    .IsRequired();

                entity.Property(b => b.StorageType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(b => b.Size)
                    .IsRequired();  
                
                entity.Property(b => b.CreatedBy)
                    .IsRequired();

                entity.Property(b => b.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()"); // Use UTC timestamps
            });


            modelBuilder.Entity<BlobData>(entity =>
            {
                entity.HasKey(e => e.Id); // Primary key
                entity.Property(e => e.Id)
                      .IsRequired();

                entity.Property(e => e.Blob)
                      .IsRequired(); 

                entity.HasOne(e => e.BlobTracking)
                      .WithOne()
                      .HasForeignKey<BlobData>(e => e.Id) 
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.ToTable("BlobData"); // Map to table BlobData
            });
        }
    }
}
