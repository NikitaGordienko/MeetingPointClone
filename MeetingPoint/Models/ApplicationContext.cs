using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetingPoint.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<User> User { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<UserRoom> UserRoom { get; set; }
        public DbSet<Calendar> Calendar { get; set; }
        public DbSet<Interval> Interval { get; set; }
        public DbSet<CalculatedInterval> CalculatedInterval { get; set; }
        public DbSet<CalculatedIntervalUser> CalculatedIntervalUser { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Interval>().HasKey(t => new { t.Id, t.CalendarId, t.UserId });
            modelBuilder.Entity<Interval>().HasOne(t => t.Calendar).WithMany(t => t.Intervals).HasForeignKey(t => t.CalendarId);
            modelBuilder.Entity<Interval>().HasOne(t => t.User).WithMany(t => t.Intervals).HasForeignKey(t => t.UserId);

            modelBuilder.Entity<UserRoom>().HasKey(t => new { t.UserId,t.RoomId });
            modelBuilder.Entity<UserRoom>().HasOne(t => t.User).WithMany(t => t.UserRooms).HasForeignKey(t => t.UserId);
            modelBuilder.Entity<UserRoom>().HasOne(t => t.Room).WithMany(t => t.UserRooms).HasForeignKey(t => t.RoomId);

            modelBuilder.Entity<CalculatedIntervalUser>().HasKey(t => new { t.CalculatedIntervalId, t.UserId });
            modelBuilder.Entity<CalculatedIntervalUser>().HasOne(t => t.CalculatedInterval).WithMany(t => t.CalculatedIntervalUsers).HasForeignKey(t => t.CalculatedIntervalId);
            modelBuilder.Entity<CalculatedIntervalUser>().HasOne(t => t.User).WithMany(t => t.CalculatedIntervalUsers).HasForeignKey(t => t.UserId);

            modelBuilder.Entity<CalculatedInterval>().HasKey(t => new { t.Id });
            modelBuilder.Entity<CalculatedInterval>().HasOne(t => t.Calendar).WithMany(t => t.CalculatedInterval).HasForeignKey(t => t.CalendarId);
            modelBuilder.Entity<CalculatedInterval>().HasMany(t => t.CalculatedIntervalUsers).WithOne(t => t.CalculatedInterval).HasForeignKey(t => t.CalculatedIntervalId);

            // TODO: Перенести всю логику из DataAnnotations в modelBuilder
        }
    }
}
