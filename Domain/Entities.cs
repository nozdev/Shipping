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
        public string ScheduleId { get; private set; } // external schedule id
        public Location From { get; private set; }
        public Location To { get; private set; }
        public DateTime Departure { get; private set; }
        public DateTime Arrival { get; private set; }

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
        public Guid CargoId { get; private set; }         // reference (not navigation) to Cargo
        public Guid? CarrierMovementId { get; private set; } // optional relation to movement
        public string EventType { get; private set; }     // "Load","Unload","Customs","Claim"...
        public DateTime EventTime { get; private set; }
        public Location Location { get; private set; }
        public string Details { get; private set; }

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
