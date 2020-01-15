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
        public virtual DbSet<ExportUnit> ExportUnit { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceLine> InvoiceLine { get; set; }
        public virtual DbSet<OperationHistory> OperationHistory { get; set; }
        public virtual DbSet<PriceFluctuation> PriceFluctuation { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<TxReport> TxReport { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=32769;database=TIRE_STORE_ALTER;user=root;password=nhat1997;treattinyasboolean=true", x => x.ServerVersion("8.0.18-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("ACCOUNT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasColumnName("PASSWORD")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Salt)
                    .HasColumnName("SALT")
                    .HasColumnType("char(32)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Username)
                    .HasColumnName("USERNAME")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMER");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

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

            modelBuilder.Entity<ExportUnit>(entity =>
            {
                entity.ToTable("EXPORT_UNIT");

                entity.HasIndex(e => e.InvoiceLineId)
                    .HasName("FK_EXPORT_UNIT_INVOICE_LINE");

                entity.HasIndex(e => e.PriceFluctuationId)
                    .HasName("FK_EXPORT_UNIT_PRICE_FLUCTUATION");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Billing).HasColumnName("BILLING");

                entity.Property(e => e.ExportPrice)
                    .HasColumnName("EXPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.InvoiceLineId)
                    .HasColumnName("INVOICE_LINE_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.PriceFluctuationId)
                    .HasColumnName("PRICE_FLUCTUATION_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("int(11)");

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

                entity.HasOne(d => d.InvoiceLine)
                    .WithMany(p => p.ExportUnit)
                    .HasForeignKey(d => d.InvoiceLineId)
                    .HasConstraintName("FK_EXPORT_UNIT_INVOICE_LINE");

                entity.HasOne(d => d.PriceFluctuation)
                    .WithMany(p => p.ExportUnit)
                    .HasForeignKey(d => d.PriceFluctuationId)
                    .HasConstraintName("FK_EXPORT_UNIT_PRICE_FLUCTUATION");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("INVOICE");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_ACCOUNT_INVOICE");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_CUSTOMER_INVOICE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.CustomerId)
                    .HasColumnName("CUSTOMER_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

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

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Total)
                    .HasColumnName("TOTAL")
                    .HasColumnType("decimal(13,4)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ACCOUNT_INVOICE");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CUSTOMER_INVOICE");
            });

            modelBuilder.Entity<InvoiceLine>(entity =>
            {
                entity.ToTable("INVOICE_LINE");

                entity.HasIndex(e => e.InvoiceId)
                    .HasName("FK_INVOICE_INVOICE_LINE");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("varchar(256)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.InvoiceId)
                    .HasColumnName("INVOICE_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Quantity)
                    .HasColumnName("QUANTITY")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Total)
                    .HasColumnName("TOTAL")
                    .HasColumnType("decimal(13,4)");

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceLine)
                    .HasForeignKey(d => d.InvoiceId)
                    .HasConstraintName("FK_INVOICE_INVOICE_LINE");
            });

            modelBuilder.Entity<OperationHistory>(entity =>
            {
                entity.ToTable("OPERATION_HISTORY");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_ACCOUNT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

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

            modelBuilder.Entity<PriceFluctuation>(entity =>
            {
                entity.ToTable("PRICE_FLUCTUATION");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.ChangedImportPrice)
                    .HasColumnName("CHANGED_IMPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.ChangedPrice)
                    .HasColumnName("CHANGED_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.CurrentImportPrice)
                    .HasColumnName("CURRENT_IMPORT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.CurrentPrice)
                    .HasColumnName("CURRENT_PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.Date)
                    .HasColumnName("DATE")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasColumnName("PRODUCT_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCT");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Brand)
                    .HasColumnName("BRAND")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasMaxLength(16)
                    .IsFixedLength();

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

                entity.Property(e => e.ModifyBy)
                    .HasColumnName("MODIFY_BY")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.ModifyDate)
                    .HasColumnName("MODIFY_DATE")
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

                entity.Property(e => e.Price)
                    .HasColumnName("PRICE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.QuantityRemain)
                    .HasColumnName("QUANTITY_REMAIN")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Size)
                    .HasColumnName("SIZE")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.Type)
                    .HasColumnName("TYPE")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");
            });

            modelBuilder.Entity<TxReport>(entity =>
            {
                entity.ToTable("TX_REPORT");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_ACCOUNT_TX_HISTORY");

                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_PRODUCT_TX_HISOTRY");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.AccountId)
                    .HasColumnName("ACCOUNT_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.Action)
                    .HasColumnName("ACTION")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.PriceUpdate)
                    .HasColumnName("PRICE_UPDATE")
                    .HasColumnType("decimal(13,4)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("PRODUCT_ID")
                    .HasMaxLength(16)
                    .IsFixedLength();

                entity.Property(e => e.QuantityUpdate)
                    .HasColumnName("QUANTITY_UPDATE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("UPDATE_TIME")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TxReport)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_ACCOUNT_TX_HISTORY");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TxReport)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_PRODUCT_TX_HISOTRY");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
