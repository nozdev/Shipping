// File: Domain/CargoAggregate.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipping.Domain
{
    // Delivery history is modeled as part of the Cargo aggregate initially.
    // In the refined model it can be derived from HandlingEvent repository; still kept here as a simple in-memory representation when needed.
    public class DeliveryHistory
    {
        // Note: when HandlingEvent is its own aggregate, history may be rebuilt from repository queries.
        private readonly List<Guid> _handlingEventIds = new();

        public IReadOnlyList<Guid> HandlingEventIds => _handlingEventIds.AsReadOnly();

        public void AddHandlingEvent(Guid handlingEventId)
        {
            if (!_handlingEventIds.Contains(handlingEventId))
                _handlingEventIds.Add(handlingEventId);
        }
    }

    // Aggregate root
    public class Cargo : Entity
    {
        public string TrackingId { get; private set; }   // business tracking id
        public DeliverySpecification? DeliverySpecification { get; private set; }
        public DeliveryHistory DeliveryHistory { get; private set; }
        public decimal Size { get; private set; }        // e.g., capacity units
        public Dictionary<string, Guid> CustomerRoles { get; private set; } // role -> customerId

        public Itinerary? AssignedItinerary { get; private set; } // derived/assigned itinerary

        // Domain invariants enforced in factories or domain methods
        private Cargo(string trackingId, decimal size)
        {
            Id = Guid.NewGuid();
            TrackingId = trackingId;
            Size = size;
            DeliveryHistory = new DeliveryHistory();
            CustomerRoles = new Dictionary<string, Guid>();
        }

        // Factory method
        public static Cargo CreateNew(string trackingId, decimal size, DeliverySpecification? spec = null)
        {
            var c = new Cargo(trackingId, size);
            if (spec is not null) c.DeliverySpecification = spec;
            return c;
        }

        // Clone as prototype (repeat business)
        public Cargo CopyAsNew(string newTrackingId)
        {
            var clone = new Cargo(newTrackingId, this.Size);
            clone.DeliverySpecification = this.DeliverySpecification;
            // copy keys for customer roles but reference same customers (IDs)
            foreach (var kv in CustomerRoles) clone.CustomerRoles[kv.Key] = kv.Value;
            return clone;
        }

        // Set or change specification (value object replacement)
        public void ChangeDeliverySpecification(DeliverySpecification spec)
        {
            DeliverySpecification = spec ?? throw new ArgumentNullException(nameof(spec));
        }

        // Attach itinerary (from routing service)
        public void AssignItinerary(Itinerary itinerary)
        {
            if (itinerary is null) throw new ArgumentNullException(nameof(itinerary));
            if (DeliverySpecification is not null && !itinerary.Satisfies(DeliverySpecification))
                throw new InvalidOperationException("Itinerary does not satisfy delivery specification.");
            AssignedItinerary = itinerary;
        }

        // Record a handling event id into history (when HandlingEvent exists as separate aggregate)
        public void RecordHandlingEvent(Guid handlingEventId)
        {
            DeliveryHistory.AddHandlingEvent(handlingEventId);
        }

        public void AddCustomerRole(string role, Guid customerId)
        {
            CustomerRoles[role] = customerId;
        }

        public bool IsDelivered()
        {
            // simple heuristic: if last recorded handling event is 'Claim' or similar; here we just check itinerary arrival passed
            if (AssignedItinerary is null) return false;
            var arrival = AssignedItinerary.Legs.Last().UnloadTime;
            return arrival <= DateTime.UtcNow;
        }
    }
}
