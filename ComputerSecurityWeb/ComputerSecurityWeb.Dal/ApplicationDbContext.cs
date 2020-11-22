using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using ComputerSecurityWeb.Dal.Models;

namespace ComputerSecurityWeb.Dal
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<CaffFileModel> CaffFiles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>(c =>
            {
                c.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(m => m.UserId);
            });

            modelBuilder.Entity<Comment>(c =>
            {
                c.HasOne(c => c.CaffFile)
                .WithMany(u => u.Comments)
                .HasForeignKey(m => m.CaffId);
            });
        }
    }
}
