// File: Demo/Program.cs
using System;
using Shipping.Domain;

namespace Shipping.Demo
{
    internal class Program
    {
        static void Main()
        {
            // Repositories and services
            var cargoRepo = new InMemoryCargoRepository();
            var customerRepo = new InMemoryCustomerRepository();
            var handlingRepo = new InMemoryHandlingEventRepository();
            var carrierRepo = new InMemoryCarrierMovementRepository();
            var routing = new SimpleRoutingService();

            // Domain objects
            var origin = new Location("NYC", "New York");
            var destination = new Location("LON", "London");

            var customer = new Customer("CUST-001", "ACME Logistics");
            customerRepo.Add(customer);

            // Create a booking (Cargo)
            var spec = new DeliverySpecification(destination, DateTime.UtcNow.AddDays(10));
            var cargo = CargoFactory.Create(trackingId: "TRACK-0001", size: 1.0m, spec);
            cargo.AddCustomerRole("shipper", customer.Id);

            cargoRepo.Add(cargo);
            Console.WriteLine($"Created Cargo {cargo.TrackingId} ({cargo.Id}) for customer {customer.Name}");

            // Route the cargo (use routing service -> returns Itinerary)
            var itinerary = routing.Route(spec, origin);
            cargo.AssignItinerary(itinerary);
            Console.WriteLine($"Assigned itinerary: {itinerary.LegCountDescription()}");

            // Create a carrier movement (a scheduled voyage) and persist
            var cm = new CarrierMovement("SCHED-001", origin, destination, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));
            carrierRepo.Add(cm);

            // Create a handling event - load
            var loadEvent = HandlingEventFactory.CreateLoadEvent(cargo.Id, cm.Id, DateTime.UtcNow.AddDays(1).AddHours(2), origin, "Loaded onto vessel V001");
            handlingRepo.Add(loadEvent);

            // Record the handling event id into Cargo's DeliveryHistory (aggregate relationship)
            cargo.RecordHandlingEvent(loadEvent.Id);

            Console.WriteLine($"Recorded HandlingEvent {loadEvent.EventType} at {loadEvent.Location.Code} for Cargo {cargo.TrackingId}");

            // Query: find handling events for cargo
            var events = handlingRepo.FindByCargoId(cargo.Id);
            foreach (var e in events)
            {
                Console.WriteLine($"Event: {e.EventType} at {e.Location.Code} on {e.EventTime}");
            }

            // Clone cargo as a prototype (repeat business)
            var clone = cargo.CopyAsNew("TRACK-0002");
            cargoRepo.Add(clone);
            Console.WriteLine($"Created clone cargo {clone.TrackingId} from prototype {cargo.TrackingId}");

            // Simple status
            Console.WriteLine($"Is original cargo delivered? {cargo.IsDelivered()}");
        }
    }

    // Extension to summarize legs for console
    public static class ItineraryExtensions
    {
        public static string LegCountDescription(this Itinerary it)
        {
            if (it is null) return "none";
            return $"{it.Legs.Count} leg(s) from {it.Origin.Code} to {it.Destination.Code}, ETA {it.Legs[^1].UnloadTime:u}";
        }
    }
}
