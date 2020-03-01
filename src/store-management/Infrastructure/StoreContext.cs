using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace store_management.Domain
{
    public partial class StoreContext : DbContext
    {
        public StoreContext()
        {
        }

        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceLine> InvoiceLine { get; set; }
        public virtual DbSet<OperationHistory> OperationHistory { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductImport> ProductImport { get; set; }
        public virtual DbSet<ProductSale> ProductSale { get; set; }
        public virtual DbSet<SaleImportReport> SaleImportReport { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<ProductExport> ProductExport { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;port=32769;database=TYRE_STORE_MS_DEV;user=root;password=nhat1997;treattinyasboolean=true", x => x.ServerVersion("8.0.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("ACCOUNT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Salt)
                    .HasColumnName("SALT")
                    .HasColumnType("char(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Hash)
                    .HasColumnName("HASH")
                    .HasColumnType("char(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Username)
                    .HasColumnName("USERNAME")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Role)
                    .HasColumnName("ROLE")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMER");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Address)
                    .HasColumnName("ADDRESS")
                    .HasColumnType("varchar(512)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.BankAccountNumber)
                    .HasColumnName("BANK_ACCOUNT_NUMBER")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.FullName)
                    .HasColumnName("FULL_NAME")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TaxCode)
                    .HasColumnName("TAX_CODE")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<ProductExport>(entity =>
            {
                entity.ToTable("PRODUCT_EXPORT");

                entity.HasIndex(e => e.ProductImportId)
                    .HasName("FK_PRODUCT_EXPORT_PRODUCT_IMPORT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.NoBillRemainQuantity)
                    .HasColumnName("NOT_BILL_REMAIN_QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductImportId)
                    .HasColumnName("PRODUCT_IMPORT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("INVOICE");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_INVOICE_ACCOUNT");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_INVOICE_CUSTOMER");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CUSTOMER_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Detail)
                    .HasColumnName("DETAIL")
                    .HasColumnType("varchar(512)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ExportDate)
                    .HasColumnName("EXPORT_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.InvoiceNo)
                    .HasColumnName("INVOICE_NO")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Total)
                    .HasColumnName("TOTAL")
                    .HasColumnType("decimal(13,4)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_INVOICE_ACCOUNT");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_INVOICE_CUSTOMER");
            });

            modelBuilder.Entity<InvoiceLine>(entity =>
            {
                entity.ToTable("INVOICE_LINE");

                entity.HasIndex(e => e.InvoiceId)
                    .HasName("FK_INVOICE_LINE_INVOICE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ExportPrice)
                    .HasColumnName("EXPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.InvoiceId)
                    .HasColumnName("INVOICE_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ProductId)
                    .HasColumnName("PRODUCT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci"); ;

                entity.Property(e => e.ExportAmount)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Total)
                    .HasColumnName("TOTAL")
                    .HasColumnType("decimal(13,4)");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceLine)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_INVOICE_LINE_INVOICE");
                
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.InvoiceLine)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_INVOICE_LINE_PRODUCT");
                
                
            });

            modelBuilder.Entity<OperationHistory>(entity =>
            {
                entity.ToTable("OPERATION_HISTORY");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_ACCOUNT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Action)
                    .HasColumnName("ACTION")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ActionDate)
                    .HasColumnName("ACTION_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasColumnName("MESSAGE")
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.OperationHistory)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ACCOUNT");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Brand)
                    .HasColumnName("BRAND")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("CREATED_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(512)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ImagePath)
                    .HasColumnName("IMAGE_PATH")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("MODIFIED_DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Pattern)
                    .HasColumnName("PATTERN")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.RefPrice)
                    .HasColumnName("REF_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.Size)
                    .HasColumnName("SIZE")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TotalQuantity)
                    .HasColumnName("TOTAL_QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<ProductImport>(entity =>
            {
                entity.ToTable("PRODUCT_IMPORT");

                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_PRODUCT_IMPORT_PRODUCT");

                entity.HasIndex(e => e.TransactionId)
                    .HasName("FK_PRODUCT_IMPORT_TRANSACTION");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CurrentImportPrice)
                    .HasColumnName("CURRENT_IMPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.Date)
                    .HasColumnName("DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.ImportPrice)
                    .HasColumnName("IMPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.ImportAmount)
                    .HasColumnName("IMPORT_QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RemainQuantity)
                    .HasColumnName("REMAIN_QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ExportableAmount)
                    .HasColumnName("EXPORTABLE_QUANITTY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnName("PRODUCT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("TRANSACTION_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImport)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PRODUCT_IMPORT_PRODUCT");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.ProductImport)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_PRODUCT_IMPORT_TRANSACTION");
            });

            modelBuilder.Entity<ProductSale>(entity =>
            {
                entity.ToTable("PRODUCT_SALE");

                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_PRODUCT_SALE_PRODUCT_IMPORT");

                entity.HasIndex(e => e.TransactionId)
                    .HasName("FK_PRODUCT_SALE_TRANSACTION");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ProductId)
                    .HasColumnName("PRODUCT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.SaleAmount)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SalePrice)
                    .HasColumnName("SALE_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("TRANSACTION_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.WarrantyCode)
                    .HasColumnName("WARRANTY_CODE")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductSale)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_PRODUCT_SALE_PRODUCT");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.ProductSale)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_PRODUCT_SALE_TRANSACTION");
            });

            modelBuilder.Entity<SaleImportReport>(entity =>
            {
                entity.HasKey(e => new { e.ProductImportId, e.ProductSaleId })
                    .HasName("PRIMARY");

                entity.ToTable("SALE_IMPORT_REPORT");

                entity.HasIndex(e => e.ProductSaleId)
                    .HasName("FK_SIR_PRODUCT_SALE");

                entity.Property(e => e.ProductImportId)
                    .HasColumnName("PRODUCT_IMPORT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.ProductSaleId)
                    .HasColumnName("PRODUCT_SALE_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.ProductImport)
                    .WithMany(p => p.SaleImportReport)
                    .HasForeignKey(d => d.ProductImportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SIR_PRODUCT_IMPORT");

                entity.HasOne(d => d.ProductSale)
                    .WithMany(p => p.SaleImportReport)
                    .HasForeignKey(d => d.ProductSaleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SIR_PRODUCT_SALE");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("TRANSACTION");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_TRANSACTION_ACCOUNT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Billing).HasColumnName("BILLING");

                entity.Property(e => e.Date)
                    .HasColumnName("DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("varchar(1)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_TRANSACTION_ACCOUNT");

                entity.Property(e => e.Note)
                    .HasColumnName("NOTE")
                    .HasColumnType("varchar(5000)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
