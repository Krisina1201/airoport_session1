using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Airport.Models;

public partial class User23Context : DbContext
{
    public User23Context()
    {
    }

    public User23Context(DbContextOptions<User23Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Aircraft> Aircrafts { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Office> Offices { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=45.67.56.214;Port=5421;Database=user23;Username=user23;Password=Njx4mFby");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("ses3", "client_type", new[] { "ЮЛ", "ФЛ" })
            .HasPostgresEnum("ses3", "order_status", new[] { "Новая", "На исследовании", "Закрыта" });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_airplan");

            entity.ToTable("aircrafts", "session2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Businessseats).HasColumnName("businessseats");
            entity.Property(e => e.Economyseats).HasColumnName("economyseats");
            entity.Property(e => e.Makemodel)
                .HasMaxLength(10)
                .HasColumnName("makemodel");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Totalseats).HasColumnName("totalseats");
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("airports_pkey");

            entity.ToTable("airports", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Iatacode)
                .HasMaxLength(3)
                .HasColumnName("iatacode");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.Airports)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_airport_country");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("countries_pkey");

            entity.ToTable("countries", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Office>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("offices_pkey");

            entity.ToTable("offices", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contact)
                .HasMaxLength(250)
                .HasColumnName("contact");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            entity.HasOne(d => d.Country).WithMany(p => p.Offices)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_office_country");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_userrole");

            entity.ToTable("roles", "session2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("routes_pkey");

            entity.ToTable("routes", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Arrivalairportid).HasColumnName("arrivalairportid");
            entity.Property(e => e.Departureairportid).HasColumnName("departureairportid");
            entity.Property(e => e.Distance).HasColumnName("distance");
            entity.Property(e => e.Flighttime).HasColumnName("flighttime");

            entity.HasOne(d => d.Arrivalairport).WithMany(p => p.RouteArrivalairports)
                .HasForeignKey(d => d.Arrivalairportid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_routes_airports3");

            entity.HasOne(d => d.Departureairport).WithMany(p => p.RouteDepartureairports)
                .HasForeignKey(d => d.Departureairportid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_routes_airports2");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("schedules_pkey");

            entity.ToTable("schedules", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aircraftid).HasColumnName("aircraftid");
            entity.Property(e => e.Confirmed).HasColumnName("confirmed");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Economyprice)
                .HasPrecision(10, 2)
                .HasColumnName("economyprice");
            entity.Property(e => e.Flightnumber)
                .HasMaxLength(10)
                .HasColumnName("flightnumber");
            entity.Property(e => e.Routeid).HasColumnName("routeid");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.Aircraftid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedule_aircraft");

            entity.HasOne(d => d.Route).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.Routeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedule_routes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_user");

            entity.ToTable("users", "session2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Officeid).HasColumnName("officeid");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Office).WithMany(p => p.Users)
                .HasForeignKey(d => d.Officeid)
                .HasConstraintName("fk_users_offices");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users_roles");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_info_pkey");

            entity.ToTable("user_info", "session2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Entrance)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("entrance");
            entity.Property(e => e.Error).HasColumnName("error");
            entity.Property(e => e.Exit)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("exit");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_info_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
