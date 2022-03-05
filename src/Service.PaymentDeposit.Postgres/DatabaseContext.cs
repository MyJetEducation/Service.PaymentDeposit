using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.PaymentDeposit.Postgres.Models;

namespace Service.PaymentDeposit.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "education";
        private const string PaymentDepositTableName = "paymentdeposit";

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<PaymentDepositEntity> AssetsDictionarEntities { get; set; }

        public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
        {
            MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

            return new DatabaseContext(options.Options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetUserInfoEntityEntry(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void SetUserInfoEntityEntry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentDepositEntity>().ToTable(PaymentDepositTableName);
            modelBuilder.Entity<PaymentDepositEntity>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<PaymentDepositEntity>().Property(e => e.Date).IsRequired();
            modelBuilder.Entity<PaymentDepositEntity>().Property(e => e.Value);
            modelBuilder.Entity<PaymentDepositEntity>().HasKey(e => e.Id);
        }
    }
}
