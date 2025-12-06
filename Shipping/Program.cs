using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipping.Domain;

namespace Shipping.Domain
{
    internal class Program
    {
        static void Main()
        {
            // --- Setup DI and DbContext ---
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables() // This allows env vars to override JSON
                .Build();

            var services = new ServiceCollection();
            services.AddDbContext<ShippingContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ShippingDb"))
            );

            var serviceProvider = services.BuildServiceProvider();
            using (var context = serviceProvider.GetRequiredService<ShippingContext>())
            {
                context.Database.Migrate(); // Ensures DB exists and applies migrations
                // --- Domain objects ---
                var origin = new Location("NYC", "New York");
                var destination = new Location("LON", "London");

                var customer = new Customer("CUST-001", "ACME Logistics");
                context.Customers.Add(customer);
                context.SaveChanges();

                // Create cargo
                var spec = new DeliverySpecification(destination, DateTime.UtcNow.AddMilliseconds(10));
                var cargo = CargoFactory.Create("TRACK-0001", 1.0m, spec);
                cargo.AddCustomerRole("shipper", customer.Id);

                context.Cargos.Add(cargo);
                context.SaveChanges();
                Console.WriteLine($"Created Cargo {cargo.TrackingId} ({cargo.Id}) for customer {customer.Name}");

                // Routing service
                var routing = new SimpleRoutingService();
                var itinerary = routing.Route(spec, origin);
                if (itinerary != null)
                {
                    cargo.AssignItinerary(itinerary);
                    Console.WriteLine($"Assigned itinerary: {cargo.AssignedItinerary}");
                }

                // Carrier movement
                var cm = new CarrierMovement("SCHED-001", origin, destination, DateTime.UtcNow.AddMilliseconds(10), DateTime.UtcNow.AddDays(3));
                context.CarrierMovements.Add(cm);
                context.SaveChanges();

                // Handling event
                var loadEvent = HandlingEventFactory.CreateLoadEvent(cargo.Id, cm.Id, DateTime.UtcNow.AddDays(1).AddHours(2), origin, "Loaded onto vessel V001");
                context.HandlingEvents.Add(loadEvent);
                context.SaveChanges();

                cargo.RecordHandlingEvent(loadEvent.Id);
                context.Cargos.Update(cargo);
                context.SaveChanges();

                Console.WriteLine($"Recorded HandlingEvent {loadEvent.EventType} at {loadEvent.Location.Code} for Cargo {cargo.TrackingId}");

                // Clone cargo
                var clone = cargo.CopyAsNew("TRACK-0002");
                context.Cargos.Add(clone);
                context.SaveChanges();
                Console.WriteLine($"Created clone cargo {clone.TrackingId} from prototype {cargo.TrackingId}");

                // Status
                Console.WriteLine($"Is original cargo delivered? {cargo.IsDelivered()}");
            }
        }
    }
}
