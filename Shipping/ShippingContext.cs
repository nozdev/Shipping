using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shipping.Domain
{


    public class ShippingContextFactory : IDesignTimeDbContextFactory<ShippingContext>
    {
        public ShippingContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddEnvironmentVariables() // <- Now this will work
            .Build();

        var connectionString = config.GetConnectionString("ShippingDb");

        var optionsBuilder = new DbContextOptionsBuilder<ShippingContext>();
        optionsBuilder.UseSqlServer(connectionString);   return new ShippingContext(optionsBuilder.Options);
        }
    }

    public class ShippingContext : DbContext
    {
        public ShippingContext(DbContextOptions<ShippingContext> options) : base(options) { }

        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CarrierMovement> CarrierMovements { get; set; }
        public DbSet<HandlingEvent> HandlingEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example: map table names
            modelBuilder.Entity<Cargo>().ToTable("Cargos");
            modelBuilder.Entity<Customer>().ToTable("Customers");
            modelBuilder.Entity<CarrierMovement>().ToTable("CarrierMovements");
            modelBuilder.Entity<HandlingEvent>().ToTable("HandlingEvents");
        }
    }
}
