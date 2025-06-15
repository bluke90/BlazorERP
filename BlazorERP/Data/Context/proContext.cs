using Microsoft.Extensions.Configuration;using System;
using System.Collections.Generic;
using BlazorERP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorERP.Data.Context;

public partial class proContext : DbContext
{
        private readonly IConfiguration _configuration;
    
    public proContext(DbContextOptions<proContext> options) : base(options) {}
    
    public proContext() {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = Program.Configuration;
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
        
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    } 

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<InventoryTransaction> InventoryTransactions { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemCategory> ItemCategories { get; set; }

    public virtual DbSet<ItemCostHistory> ItemCostHistories { get; set; }

    public virtual DbSet<ItemImage> ItemImages { get; set; }

    public virtual DbSet<ItemPriceHistory> ItemPriceHistories { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }

    public virtual DbSet<SalesOrder> SalesOrders { get; set; }

    public virtual DbSet<SalesOrderLine> SalesOrderLines { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StorageLocation> StorageLocations { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8058F3527");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MailingAddress).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(40);
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(e => e.TxnId).HasName("PK__Inventor__C196085464BABFBA");

            entity.ToTable("InventoryTransactions", "IMS");

            entity.Property(e => e.CreatedUtc).HasColumnType("datetime");
            entity.Property(e => e.PerUnitCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RefDocument).HasMaxLength(50);
            entity.Property(e => e.TxnType).HasMaxLength(30);

            entity.HasOne(d => d.Item).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__ItemI__73BA3083");

            entity.HasOne(d => d.StorageLocation).WithMany(p => p.InventoryTransactions)
                .HasForeignKey(d => d.StorageLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Stora__74AE54BC");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Items__727E838BDCB7AECC");

            entity.ToTable("Items", "IMS");

            entity.HasIndex(e => e.SKU, "UQ__Items__CA1ECF0D16BCE34E").IsUnique();

            entity.Property(e => e.CreatedUtc).HasColumnType("datetime");
            entity.Property(e => e.DefaultCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DefaultPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.SKU).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Items__CategoryI__6383C8BA");

            entity.HasOne(d => d.Unit).WithMany(p => p.Items)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK__Items__UnitId__6477ECF3");
        });

        modelBuilder.Entity<ItemCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ItemCate__19093A0BDC2A75BE");

            entity.ToTable("ItemCategories", "IMS");

            entity.HasIndex(e => e.Name, "UQ__ItemCate__737584F6B0EFAD3C").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<ItemCostHistory>(entity =>
        {
            entity.HasKey(e => e.ItemCostId).HasName("PK__ItemCost__14F1E3F332F2506B");

            entity.ToTable("ItemCostHistory", "IMS");

            entity.Property(e => e.ChangedUtc).HasColumnType("datetime");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemCostHistories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemCostH__ItemI__787EE5A0");
        });

        modelBuilder.Entity<ItemImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ItemImages_pk");

            entity.ToTable("ItemImages", "FileStore");

            entity.Property(e => e.Content).HasColumnType("image");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MimeType)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Item).WithMany(p => p.ItemImages)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ItemImages_Item_fk");
        });

        modelBuilder.Entity<ItemPriceHistory>(entity =>
        {
            entity.HasKey(e => e.ItemPriceId).HasName("PK__ItemPric__7E70A2627C870EAE");

            entity.ToTable("ItemPriceHistory", "IMS");

            entity.Property(e => e.ChangedUtc).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemPriceHistories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemPrice__ItemI__7C4F7684");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("OrderStatus_pk");

            entity.ToTable("OrderStatus", "Status");

            entity.HasIndex(e => e.Code, "UQ__OrderSta__A25C5AA75A62459D").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(4);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.POId).HasName("PK__Purchase__5F02A2D4E8836831");

            entity.ToTable("PurchaseOrders", "Supply");

            entity.Property(e => e.ExpectedUtc).HasColumnType("datetime");
            entity.Property(e => e.OrderedUtc).HasColumnType("datetime");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("PurchaseOrder_OrdStatus__fk");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PurchaseOrder_Supplier__fk");
        });

        modelBuilder.Entity<PurchaseOrderLine>(entity =>
        {
            entity.HasKey(e => e.POLineId).HasName("PK__Purchase__07B9D362C263D31A");

            entity.ToTable("PurchaseOrderLines", "Supply");

            entity.Property(e => e.UnitCost).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseO__ItemI__07C12930");

            entity.HasOne(d => d.PO).WithMany(p => p.PurchaseOrderLines)
                .HasForeignKey(d => d.POId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PurchaseOr__POId__06CD04F7");
        });

        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.SOId).HasName("PK__SalesOrd__A7FF3282BFD7641C");

            entity.Property(e => e.OrderedUtc).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__SalesOrde__Custo__0C85DE4D");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.SalesOrders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("SalesOrders_OrderStatus__fk");
        });

        modelBuilder.Entity<SalesOrderLine>(entity =>
        {
            entity.HasKey(e => e.SOLineId).HasName("PK__SalesOrd__599D1455A0CD034C");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Item).WithMany(p => p.SalesOrderLines)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesOrde__ItemI__123EB7A3");

            entity.HasOne(d => d.SO).WithMany(p => p.SalesOrderLines)
                .HasForeignKey(d => d.SOId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SalesOrder__SOId__114A936A");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => new { e.ItemId, e.StorageLocationId });

            entity.ToTable("Stock", "IMS");

            entity.HasOne(d => d.Item).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stock__ItemId__6EF57B66");

            entity.HasOne(d => d.StorageLocation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.StorageLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Stock__StorageLo__6FE99F9F");
        });

        modelBuilder.Entity<StorageLocation>(entity =>
        {
            entity.HasKey(e => e.StorageLocationId).HasName("PK__StorageL__59BA7AF9A8FD988C");

            entity.ToTable("StorageLocations", "IMS");

            entity.HasIndex(e => e.Code, "UQ__StorageL__A25C5AA711A6E5E6").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B44B0EE533");

            entity.ToTable("Suppliers", "Supply");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(40);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5ECB58473AE67");

            entity.ToTable("Units", "IMS");

            entity.HasIndex(e => e.Name, "UQ__Units__737584F6C4BC49D5").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

