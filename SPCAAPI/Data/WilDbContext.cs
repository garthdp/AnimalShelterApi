using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SPCAAPI.Models;

namespace SPCAAPI.Data;

public partial class WilDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public WilDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public WilDbContext()
    {
    }
    public WilDbContext(DbContextOptions<WilDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<BoardingRequest> BoardingRequests { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Volunteer> Volunteers { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("WilDb");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.Property(e => e.AnimalId).HasColumnName("AnimalID");
            entity.Property(e => e.AdoptionStatus).HasMaxLength(20);
            entity.Property(e => e.AnimalType).HasMaxLength(50);
            entity.Property(e => e.Breed).HasMaxLength(50);
            entity.Property(e => e.Health).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<BoardingRequest>(entity =>
        {
            entity.HasKey(e => e.BoardingId);

            entity.Property(e => e.BoardingId).HasColumnName("BoardingID");
            entity.Property(e => e.Breed).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasMaxLength(50);
            entity.Property(e => e.OwnerEmail).HasMaxLength(100);
            entity.Property(e => e.OwnerName).HasMaxLength(50);
            entity.Property(e => e.PetName).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasMaxLength(50);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.EventId).HasColumnName("EventID");
            entity.Property(e => e.EventDate).HasMaxLength(50);
            entity.Property(e => e.EventDescription).HasMaxLength(300);
            entity.Property(e => e.EventName).HasMaxLength(100);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(e => e.ReportId).HasColumnName("ReportID");
            entity.Property(e => e.ContactInfo).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserEmail);

            entity.Property(e => e.UserEmail).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.ProfilePicture).HasMaxLength(50);
            entity.Property(e => e.UserType).HasMaxLength(50);
        });

        modelBuilder.Entity<Volunteer>(entity =>
        {
            entity.Property(e => e.VolunteerId).HasColumnName("VolunteerID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.VolunteerDate).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
