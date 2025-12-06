using System;
using System.Linq;
using Shipping.Domain;
using Xunit;

namespace Shipping.Tests
{
    public class CargoTests
    {
        [Fact]
        public void CanCreateCargoAndAssignCustomer()
        {
            var customer = new Customer("CUST-001", "ACME Logistics");
            var spec = new DeliverySpecification(new Location("LON", "London"), DateTime.UtcNow.AddDays(3));
            var cargo = CargoFactory.Create("TRACK-0001", 1.0m, spec);

            cargo.AddCustomerRole("shipper", customer.Id);

            Assert.Equal("TRACK-0001", cargo.TrackingId);
            Assert.Contains(cargo.CustomerRoles, r => r.Key == "shipper" && r.Value == customer.Id);
        }

        [Fact]
        public void CanAssignItineraryToCargo()
        {
            var origin = new Location("NYC", "New York");
            var destination = new Location("LON", "London");
            var spec = new DeliverySpecification(destination, DateTime.UtcNow.AddDays(3));
            var cargo = CargoFactory.Create("TRACK-0002", 1.0m, spec);

            var routing = new SimpleRoutingService();
            var itinerary = routing.Route(spec, origin);
            cargo.AssignItinerary(itinerary);

            Assert.Equal(itinerary, cargo.AssignedItinerary);
            Assert.Equal(destination, cargo.AssignedItinerary?.Destination);
        }

        [Fact]
        public void CanRecordHandlingEvent()
        {
            var cargo = CargoFactory.Create("TRACK-0003", 1.0m, new DeliverySpecification(new Location("LON", "London"), DateTime.UtcNow.AddDays(3)));
            var cm = new CarrierMovement("SCHED-001", new Location("NYC", "New York"), new Location("LON", "London"), DateTime.UtcNow, DateTime.UtcNow.AddDays(3));

            var loadEvent = HandlingEventFactory.CreateLoadEvent(cargo.Id, cm.Id, DateTime.UtcNow, cm.From, "Loaded onto vessel");
            cargo.RecordHandlingEvent(loadEvent.Id);

            Assert.Contains(loadEvent.Id, cargo.DeliveryHistory.HandlingEventIds.Select(e => e));
        }

        [Fact]
        public void CloneCargoCreatesNewCargo()
        {
            var cargo = CargoFactory.Create("TRACK-0004", 1.0m, new DeliverySpecification(new Location("LON", "London"), DateTime.UtcNow.AddDays(3)));
            var clone = cargo.CopyAsNew("TRACK-0005");

            Assert.NotEqual(cargo.TrackingId, clone.TrackingId);
            Assert.Equal(cargo.Size, clone.Size);
            Assert.Equal(cargo.DeliverySpecification?.Destination, clone.DeliverySpecification?.Destination);
        }

        [Fact]
        public void IsDeliveredReturnsFalseInitially()
        {
            var cargo = CargoFactory.Create("TRACK-0006", 1.0m, new DeliverySpecification(new Location("LON", "London"), DateTime.UtcNow.AddDays(3)));
            Assert.False(cargo.IsDelivered());
        }
    }
}
