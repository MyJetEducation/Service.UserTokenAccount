using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Service.UserTokenAccount.Postgres.Models;

namespace Service.UserTokenAccount.Postgres
{
	public class DatabaseContext : MyDbContext
	{
		public const string Schema = "education";
		public const string UserTokenAccountTableName = "usertoken_account";
		public const string UserTokenOperationTableName = "usertoken_operation";

		public DatabaseContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<UserTokenAccountEntity> UserTokenAccountEntities { get; set; }
		public DbSet<UserTokenOperationEntity> UserTokenOperationEntities { get; set; }

		public static DatabaseContext Create(DbContextOptionsBuilder<DatabaseContext> options)
		{
			MyTelemetry.StartActivity($"Database context {Schema}")?.AddTag("db-schema", Schema);

			return new DatabaseContext(options.Options);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(Schema);

			SetUserTokenAccountEntityEntry(modelBuilder);
			SetUserTokenOperationEntityEntry(modelBuilder);

			base.OnModelCreating(modelBuilder);
		}

		private static void SetUserTokenAccountEntityEntry(ModelBuilder modelBuilder)
		{
			EntityTypeBuilder<UserTokenAccountEntity> entityTypeBuilder = modelBuilder.Entity<UserTokenAccountEntity>();

			entityTypeBuilder.ToTable(UserTokenAccountTableName);

			entityTypeBuilder.Property(e => e.UserId).IsRequired();
			entityTypeBuilder.Property(e => e.Date).IsRequired();
			entityTypeBuilder.Property(e => e.Value).IsRequired();

			entityTypeBuilder.HasKey(e => e.UserId);
		}

		private static void SetUserTokenOperationEntityEntry(ModelBuilder modelBuilder)
		{
			EntityTypeBuilder<UserTokenOperationEntity> entityTypeBuilder = modelBuilder.Entity<UserTokenOperationEntity>();

			entityTypeBuilder.ToTable(UserTokenOperationTableName);

			entityTypeBuilder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
			entityTypeBuilder.Property(e => e.Date).IsRequired();
			entityTypeBuilder.Property(e => e.UserId).IsRequired();
			entityTypeBuilder.Property(e => e.Movement).HasConversion<string>().IsRequired();
			entityTypeBuilder.Property(e => e.Source).HasConversion<string>().IsRequired();
			entityTypeBuilder.Property(e => e.ProductType).HasConversion<string>();
			entityTypeBuilder.Property(e => e.Value).IsRequired();
			entityTypeBuilder.Property(e => e.Info);

			entityTypeBuilder.HasKey(e => e.Id);

			entityTypeBuilder.HasIndex(e => e.Date);
			entityTypeBuilder.HasIndex(e => e.UserId);
			entityTypeBuilder.HasIndex(e => e.Movement);
			entityTypeBuilder.HasIndex(e => e.Source);
			entityTypeBuilder.HasIndex(e => e.ProductType);
			entityTypeBuilder.HasIndex(e => e.Value);
		}
	}
}