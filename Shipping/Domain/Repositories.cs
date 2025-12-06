// File: Domain/Repositories.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipping.Domain
{
    // Repository interfaces - designed for aggregate roots
    public interface ICargoRepository
    {
        void Add(Cargo cargo);
        Cargo? GetById(Guid id);
        Cargo? GetByTrackingId(string trackingId);
        IEnumerable<Cargo> FindByCustomerId(Guid customerId);
    }

    public interface IHandlingEventRepository
    {
        void Add(HandlingEvent evt);
        HandlingEvent? GetById(Guid id);
        IEnumerable<HandlingEvent> FindByCargoId(Guid cargoId);
        IEnumerable<HandlingEvent> FindByCarrierMovementId(Guid carrierMovementId);
    }

    public interface ICustomerRepository
    {
        void Add(Customer c);
        Customer? GetById(Guid id);
        Customer? FindByCustomerId(string customerId);
    }

    public interface ICarrierMovementRepository
    {
        void Add(CarrierMovement m);
        CarrierMovement? GetById(Guid id);
    }

    // Simple thread-unsafe in-memory implementations for demo/testing
    public class InMemoryCargoRepository : ICargoRepository
    {
        private readonly Dictionary<Guid, Cargo> _store = new();

        public void Add(Cargo cargo) => _store[cargo.Id] = cargo;
        public Cargo? GetById(Guid id) => _store.TryGetValue(id, out var c) ? c : null;
        public Cargo? GetByTrackingId(string trackingId) => _store.Values.FirstOrDefault(c => c.TrackingId == trackingId);
        public IEnumerable<Cargo> FindByCustomerId(Guid customerId) => _store.Values.Where(c => c.CustomerRoles.Values.Contains(customerId));
    }

    public class InMemoryHandlingEventRepository : IHandlingEventRepository
    {
        private readonly Dictionary<Guid, HandlingEvent> _store = new();

        public void Add(HandlingEvent evt) => _store[evt.Id] = evt;
        public HandlingEvent? GetById(Guid id) => _store.TryGetValue(id, out var e) ? e : null;
        public IEnumerable<HandlingEvent> FindByCargoId(Guid cargoId) => _store.Values.Where(e => e.CargoId == cargoId).OrderBy(e => e.EventTime);
        public IEnumerable<HandlingEvent> FindByCarrierMovementId(Guid carrierMovementId) => _store.Values.Where(e => e.CarrierMovementId == carrierMovementId);
    }

    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private readonly Dictionary<Guid, Customer> _store = new();
        public void Add(Customer c) => _store[c.Id] = c;
        public Customer? GetById(Guid id) => _store.TryGetValue(id, out var c) ? c : null;
        public Customer? FindByCustomerId(string customerId) => _store.Values.FirstOrDefault(c => c.CustomerId == customerId);
    }

    public class InMemoryCarrierMovementRepository : ICarrierMovementRepository
    {
        private readonly Dictionary<Guid, CarrierMovement> _store = new();
        public void Add(CarrierMovement m) => _store[m.Id] = m;
        public CarrierMovement? GetById(Guid id) => _store.TryGetValue(id, out var m) ? m : null;
    }
}
