using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Models;

public partial class EcommerceContext : DbContext
{
    public EcommerceContext()
    {
    }

    public EcommerceContext(DbContextOptions<EcommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ecommerce;TrustServerCertificate=True;User Id=chirag;Password=12345;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity
                .HasKey(c => new { c.UserId, c.ProductId });
            entity
                .ToTable("cart");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__cart__user_id__5DCAEF64");
        });

     

        modelBuilder.Entity<Order>(entity =>
        {
            entity
                .HasKey(c => new { c.OrderId });
            entity
                .ToTable("orders");

            entity.Property(e => e.CustomerAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("customer_address");
            entity.Property(e => e.ItemValue).HasColumnName("item_value");
            entity.Property(e => e.OrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("order_id");
            entity.Property(e => e.OrderTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_time");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__products__47027DF547706339");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("product_description");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductPic)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("product_pic");
            entity.Property(e => e.Sale)
                .HasDefaultValueSql("((0))")
                .HasColumnName("sale");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((1))")
                .HasColumnName("status");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Seller).WithMany(p => p.Products)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__products__seller__72C60C4A");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__sellers__780A0A973F0A0DF4");

            entity.ToTable("sellers");

            entity.HasIndex(e => e.SellerCompany, "UQ__sellers__B5A2B4675F76C622").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__sellers__B9BE370EBE679E57").IsUnique();

            entity.HasIndex(e => e.Gst, "UQ__sellers__DCD87CA2933AAB45").IsUnique();

            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Gst)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("gst");
            entity.Property(e => e.IsAuthorized)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("isAuthorized");
            entity.Property(e => e.SellerCompany)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("seller_company");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("test");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F73CE0776");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61641C4538D9").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__users__B43B145FE6049487").IsUnique();

            entity.HasIndex(e => e.Token, "UQ__users__CA90DA7AA989F611").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("('')")
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.ProfilePic)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("profile_pic");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.Verified)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValueSql("('false')")
                .IsFixedLength()
                .HasColumnName("verified");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("wishlist");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__wishlist__user_i__60A75C0F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
