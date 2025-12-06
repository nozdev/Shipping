// File: Domain/Entities.cs
using System;
using System.Collections.Generic;

namespace Shipping.Domain
{
    // Base entity with GUID identity
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
    }

    // Customer entity (identity)
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public string CustomerId { get; private set; } // business key

        public Customer(string customerId, string name)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    // CarrierMovement is an aggregate root representing a scheduled movement (a transport leg in schedule)
    public class CarrierMovement : Entity
    {
        public string ScheduleId { get; set; }

        // Navigation properties
        public Location From { get; set; }
        public Location To { get; set; }

        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }

        // EF Core needs a parameterless constructor
        public CarrierMovement() { }

        public CarrierMovement(string scheduleId, Location from, Location to, DateTime departure, DateTime arrival)
        {
            Id = Guid.NewGuid();
            ScheduleId = scheduleId;
            From = from;
            To = to;
            Departure = departure;
            Arrival = arrival;
        }
    }

    // HandlingEvent can be its own aggregate (created in low-contention transactions)
    public class HandlingEvent : Entity
    {
       // Scalar properties
        public Guid CargoId { get; set; }
        public string EventType { get; set; }
        public DateTime EventTime { get; set; }

        // Store location as a scalar for EF Core
        public string LocationCode { get; set; }

        // Navigation properties
        public Location Location { get; set; }
        public Guid? CarrierMovementId { get; set; }
        public CarrierMovement CarrierMovement { get; set; }

        public string Details { get; set; }

        // Parameterless constructor for EF Core
        public HandlingEvent() { }

        public HandlingEvent(Guid cargoId, string eventType, DateTime eventTime, Location location, Guid? carrierMovementId = null, string details = "")
        {
            Id = Guid.NewGuid();
            CargoId = cargoId;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            EventTime = eventTime;
            Location = location ?? throw new ArgumentNullException(nameof(location));
            CarrierMovementId = carrierMovementId;
            Details = details ?? string.Empty;
        }
    }
}
