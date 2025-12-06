// File: Domain/ValueObjects.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipping.Domain
{
    // Value object for Location (simple)
    public sealed record Location(string Code, string Name);

    // Delivery specification is a Value Object describing the desired delivery
    public sealed record DeliverySpecification(Location Destination, DateTime? LatestArrival);

    // Leg in an itinerary (value)
    public sealed record Leg(string VesselVoyageId, Location LoadLocation, DateTime LoadTime, Location UnloadLocation, DateTime UnloadTime);

    // Itinerary is a Value Object composed of ordered Legs
    public sealed record Itinerary(IReadOnlyList<Leg> Legs)
    {
        public Location Origin => Legs.First().LoadLocation;
        public Location Destination => Legs.Last().UnloadLocation;

        public bool Satisfies(DeliverySpecification spec)
        {
            if (spec is null) return false;
            if (spec.Destination is not null && !spec.Destination.Equals(Destination)) return false;
            if (spec.LatestArrival.HasValue)
            {
                var arrival = Legs.Last().UnloadTime;
                if (arrival > spec.LatestArrival.Value) return false;
            }
            return true;
        }

        public static Itinerary Empty => new Itinerary(Array.Empty<Leg>());
    }
}
