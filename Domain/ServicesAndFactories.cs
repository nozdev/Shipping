// File: Domain/ServicesAndFactories.cs
using System;
using System.Collections.Generic;

namespace Shipping.Domain
{
    // Routing service interface - translates a route specification to an Itinerary
    public interface IRoutingService
    {
        Itinerary Route(DeliverySpecification spec, Location origin);
    }

    // A trivial routing implementation for demo - in real world this would be graph search / optimized component
    public class SimpleRoutingService : IRoutingService
    {
        public Itinerary Route(DeliverySpecification spec, Location origin)
        {
            // naive example: single-leg itinerary from origin to destination next-day
            var legs = new List<Leg>();
            var now = DateTime.UtcNow;
            var loadTime = now.AddHours(12);
            var unloadTime = (spec.LatestArrival ?? now.AddDays(2)).AddHours(-1);

            var leg = new Leg(vesselVoyageId: "V001",
                              LoadLocation: origin,
                              LoadTime: loadTime,
                              UnloadLocation: spec.Destination,
                              UnloadTime: unloadTime);

            legs.Add(leg);
            return new Itinerary(legs);
        }
    }

    // Factories for domain aggregates
    public static class CargoFactory
    {
        public static Cargo Create(string trackingId, decimal size, DeliverySpecification? spec = null)
            => Cargo.CreateNew(trackingId, size, spec);
    }

    public static class HandlingEventFactory
    {
        public static HandlingEvent CreateLoadEvent(Guid cargoId, Guid? carrierMovementId, DateTime timestamp, Location location, string details = "")
            => new HandlingEvent(cargoId, "Load", timestamp, location, carrierMovementId, details);

        public static HandlingEvent CreateUnloadEvent(Guid cargoId, Guid? carrierMovementId, DateTime timestamp, Location location, string details = "")
            => new HandlingEvent(cargoId, "Unload", timestamp, location, carrierMovementId, details);

        public static HandlingEvent CreateCustomsEvent(Guid cargoId, DateTime timestamp, Location location, string details = "")
            => new HandlingEvent(cargoId, "Customs", timestamp, location, null, details);
    }
}
