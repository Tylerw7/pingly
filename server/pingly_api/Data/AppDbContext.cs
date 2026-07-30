using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pingly_api.Models;

namespace pingly_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<DeviceSubscription> DeviceSubscriptions { get; set; }

        public DbSet<Device> Devices { get; set; }
        

        protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        ConfigureTopics(modelBuilder);

        ConfigureMessages(modelBuilder);

        ConfigureDeviceSubscriptions(modelBuilder);

        ConfigureDevices(modelBuilder);
    }



    private static void ConfigureTopics(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Topic>()
            .HasKey(x => x.Id);


        modelBuilder.Entity<Topic>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();


        // Topic names must be unique
        modelBuilder.Entity<Topic>()
            .HasIndex(x => x.Name)
            .IsUnique();


        modelBuilder.Entity<Topic>()
            .Property(x => x.Name)
            .HasMaxLength(200);


        modelBuilder.Entity<Topic>()
            .HasMany(x => x.Messages)
            .WithOne(x => x.Topic)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Topic>()
            .HasMany(x => x.DeviceSubscriptions)
            .WithOne(x => x.Topic)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }



    private static void ConfigureMessages(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasKey(x => x.Id);


        modelBuilder.Entity<Message>()
            .Property(x => x.Body)
            .HasMaxLength(4000);


        // Faster topic history queries
        modelBuilder.Entity<Message>()
            .HasIndex(x => new
            {
                x.TopicId,
                x.CreatedAt
            });
    }



    private static void ConfigureDeviceSubscriptions(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceSubscription>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<DeviceSubscription>()
            .HasOne(x => x.Topic)
            .WithMany(x => x.DeviceSubscriptions)
            .HasForeignKey(x => x.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeviceSubscription>()
            .HasOne(x => x.Device)
            .WithMany(x => x.DeviceSubscriptions)
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate subscriptions
        modelBuilder.Entity<DeviceSubscription>()
            .HasIndex(x => new
            {
                x.TopicId,
                x.DeviceId
            })
            .IsUnique();
}


    private static void ConfigureDevices(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasKey(x => x.Id);


        modelBuilder.Entity<Device>()
            .Property(x => x.ApnsToken)
            .HasMaxLength(500);


        // One APNS token should not duplicate
        modelBuilder.Entity<Device>()
            .HasIndex(x => x.ApnsToken)
            .IsUnique();
    }
    }
}