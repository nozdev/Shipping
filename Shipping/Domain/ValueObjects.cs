// File: Domain/ValueObjects.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipping.Domain
{
    // Value object for Location (simple)
    public class Location : Entity
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        // Parameterless constructor required by EF Core
        public Location() { }

        // Constructor to allow instantiation with Name and Code
        public Location(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }


    // Delivery specification is a Value Object describing the desired delivery
    public sealed record DeliverySpecification
    {
        // EF Core primary key
        public int Id { get; set; }

        // Navigation property stored as scalar foreign key
        public string DestinationCode { get; set; }
        public Location Destination { get; set; }

        public DateTime? LatestArrival { get; set; }

        // Parameterless constructor for EF Core
        public DeliverySpecification() { }

        // Constructor for domain logic
        public DeliverySpecification(Location destination, DateTime? latestArrival)
        {
            Destination = destination;
            DestinationCode = destination?.Code;
            LatestArrival = latestArrival;
        }

        // Copy constructor for cloning
        public DeliverySpecification(DeliverySpecification original)
        {
            Destination = original.Destination;
            DestinationCode = original.Destination?.Code;
            LatestArrival = original.LatestArrival;
        }
    }
    // Leg in an itinerary (value)
public class Leg
    {
        // EF Core primary key
        public int Id { get; set; }

        // Scalar properties
        public string VesselVoyageId { get; set; }

        // Navigation properties
        public Location LoadLocation { get; set; }
        public DateTime LoadTime { get; set; }

        public Location UnloadLocation { get; set; }
        public DateTime UnloadTime { get; set; }

        // Parameterless constructor for EF Core
        public Leg() { }

        // Domain constructor for in-memory usage
        public Leg(string vesselVoyageId, Location loadLocation, DateTime loadTime, Location unloadLocation, DateTime unloadTime)
        {
            VesselVoyageId = vesselVoyageId;
            LoadLocation = loadLocation;
            LoadTime = loadTime;
            UnloadLocation = unloadLocation;
            UnloadTime = unloadTime;
        }

        // Copy constructor
        public Leg(Leg original)
        {
            VesselVoyageId = original.VesselVoyageId;
            LoadLocation = original.LoadLocation;
            LoadTime = original.LoadTime;
            UnloadLocation = original.UnloadLocation;
            UnloadTime = original.UnloadTime;
        }
    }
    // Itinerary is a Value Object composed of ordered Legs
    public class Itinerary
    {
        // EF Core primary key
        public int Id { get; set; }

        // Navigation property — EF Core can map collections
        public List<Leg> Legs { get; set; } = new();

        // Parameterless constructor for EF Core
        public Itinerary() { }

        // Constructor for domain logic / in-memory creation
        public Itinerary(IEnumerable<Leg> legs)
        {
            Legs = new List<Leg>(legs);
        }

        // Copy constructor
        public Itinerary(Itinerary original)
        {
            Legs = new List<Leg>(original.Legs);
        }
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
