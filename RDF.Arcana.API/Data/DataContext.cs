﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RDF.Arcana.API.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RDF.Arcana.API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Items> Items { get; set; }
    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<MeatType> MeatTypes { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    public virtual DbSet<ProductSubCategory> ProductSubCategories { get; set; }
    public virtual DbSet<Uom> Uoms { get; set; }
    public virtual DbSet<UserRoles> UserRoles { get; set; }
    public virtual DbSet<Discount> Discounts { get; set; }
    public virtual DbSet<TermDays> TermDays { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Status> Status { get; set; }
    public virtual DbSet<RequestedClient> RequestedClients { get; set; }
    public virtual DbSet<RejectedClients> RejectedClients { get; set; }
    public virtual DbSet<ApprovedClient> ApprovedClients { get; set; }
    public virtual DbSet<Freebies> Freebies { get; set; }
    public virtual DbSet<FreebieRequest> FreebieRequests { get; set; }
    public virtual DbSet<ApprovedFreebies> ApprovedFreebies { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<UserRoles>()
            .Property(e => e.Permissions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<ICollection<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));

        modelBuilder.Entity<UserRoles>()
            .HasOne(ur => ur.User)
            .WithOne(u => u.UserRoles)
            .HasForeignKey<User>(u => u.UserRoleId);

        modelBuilder.Entity<Company>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Company>(x => x.AddedBy);

        modelBuilder.Entity<Department>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Department>(u => u.AddedBy);

        modelBuilder.Entity<Discount>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Discount>(u => u.AddedBy);

        modelBuilder.Entity<Items>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Items>(u => u.AddedBy);

        modelBuilder.Entity<Location>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Location>(u => u.AddedBy);

        modelBuilder.Entity<MeatType>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<MeatType>(u => u.AddedBy);

        modelBuilder.Entity<ProductSubCategory>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<ProductSubCategory>(u => u.AddedBy);

        modelBuilder.Entity<ProductCategory>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<ProductCategory>(u => u.AddedBy);

        modelBuilder.Entity<TermDays>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<TermDays>(u => u.AddedBy);

        modelBuilder.Entity<Uom>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<Uom>(u => u.AddedBy);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<User>(u => u.AddedBy);

        modelBuilder.Entity<UserRoles>()
            .HasOne(u => u.AddedByUser)
            .WithOne()
            .HasForeignKey<UserRoles>(u => u.AddedBy);

        modelBuilder.Entity<Client>()
            .HasOne(x => x.AddedByUser)
            .WithOne()
            .HasForeignKey<Client>(x => x.AddedBy);

        modelBuilder.Entity<Client>()
            .HasOne(x => x.ModifiedByUser)
            .WithOne()
            .HasForeignKey<Client>(x => x.ModifiedBy);
        
        modelBuilder.Entity<Status>()
            .HasOne(x => x.ModifiedByUser)
            .WithOne()
            .HasForeignKey<Status>(x => x.ModifiedBy);
        
        modelBuilder.Entity<Status>()
            .HasOne(x => x.AddedByUser)
            .WithOne()
            .HasForeignKey<Status>(x => x.AddedBy);
        
        modelBuilder.Entity<RequestedClient>()
            .HasOne(x => x.RequestedByUser)
            .WithOne()
            .HasForeignKey<RequestedClient>(x => x.RequestedBy);
        
        modelBuilder.Entity<ApprovedClient>()
            .HasOne(x => x.ApprovedByUser)
            .WithOne()
            .HasForeignKey<ApprovedClient>(x => x.ApprovedBy);
        
        modelBuilder.Entity<RejectedClients>()
            .HasOne(x => x.RejectedByUser)
            .WithOne()
            .HasForeignKey<RejectedClients>(x => x.RejectedBy);
        
        //////
        
        modelBuilder.Entity<RejectedClients>()
            .HasOne(x => x.RejectedStatus)
            .WithOne()
            .HasForeignKey<RejectedClients>(x => x.Status);
        
        modelBuilder.Entity<RequestedClient>()
            .HasOne(x => x.RequestStatus)
            .WithOne()
            .HasForeignKey<RequestedClient>(x => x.Status);

        modelBuilder.Entity<ApprovedClient>()
            .HasOne(x => x.ApprovedStatus)
            .WithOne()
            .HasForeignKey<ApprovedClient>(x => x.Status);
        
        modelBuilder.Entity<ApprovedClient>(entity =>
        {
            entity.HasOne(ac => ac.Client)
                .WithMany()
                .HasForeignKey(ac => ac.ClientId);

            entity.HasOne(ac => ac.ApprovedByUser)
                .WithMany()
                .HasForeignKey(ac => ac.ApprovedBy);

            entity.HasOne(ac => ac.ApprovedStatus)
                .WithMany()
                .HasForeignKey(ac => ac.Status);
        });

        modelBuilder.Entity<FreebieRequest>(entity =>
        {
            entity.HasOne(fr => fr.ApprovedClient)
                .WithMany(ac => ac.Freebies)
                .HasForeignKey(fr => fr.ClientId);

            entity.HasOne(fr => fr.FreebieStatus)
                .WithMany()
                .HasForeignKey(fr => fr.StatusId);

            entity.HasOne(fr => fr.AddedByUser)
                .WithMany()
                .HasForeignKey(fr => fr.AddedBy);
        });

        modelBuilder.Entity<Freebies>(entity =>
        {
            entity.HasOne(f => f.Item)
                .WithMany()
                .HasForeignKey(f => f.ItemId);

            entity.HasOne(f => f.FreebieRequest)
                .WithMany(fr => fr.Freebies)
                .HasForeignKey(f => f.FreebieRequestId);
        });
        
        modelBuilder.Entity<Freebies>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.Item)
                .WithMany(p => p.Freebies)
                .HasForeignKey(d => d.ItemId);

            entity.HasOne(d => d.FreebieRequest)
                .WithMany(p => p.Freebies)
                .HasForeignKey(d => d.FreebieRequestId);
        });

        modelBuilder.Entity<FreebieRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(d => d.Freebies)
                .WithOne(p => p.FreebieRequest)
                .HasForeignKey(d => d.FreebieRequestId);
        });

        modelBuilder.Entity<ApprovedFreebies>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.FreebieRequest)
                .WithMany()
                .HasForeignKey(d => d.FreebieRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        
            entity.HasOne(d => d.ApprovedByUser)
                .WithMany()
                .HasForeignKey(d => d.ApprovedBy);
        });

        modelBuilder.Entity<RejectedFreebies>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.FreebieRequest)
                .WithMany()
                .HasForeignKey(d => d.FreebieRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        
            entity.HasOne(d => d.RejectedByUser)
                .WithMany()
                .HasForeignKey(d => d.RejectedBy);
        });

    }
}