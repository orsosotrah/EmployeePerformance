using BuildingBlocks.Contracts.Models;
using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.Contracts.Events
{
    // Events ketika Employee dibuat/diupdate
    public record EmployeeCreatedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Department { get; init; }
        public string Position { get; init; }
        public DateTime JoinDate { get; init; }
        public ContactInfoDto ContactInfo { get; init; }
    }

    public record EmployeeUpdatedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public IDictionary<string, object> ChangedProperties { get; init; }
    }

    // Events untuk perubahan status/departemen
    public record EmployeeStatusChangedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public string OldStatus { get; init; }
        public string NewStatus { get; init; }
        public string Reason { get; init; }
    }

    public record EmployeeDepartmentChangedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public string OldDepartment { get; init; }
        public string NewDepartment { get; init; }
        public DateTime EffectiveDate { get; init; }
    }

    // Events untuk team management
    public record TeamMemberAssignedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public Guid TeamId { get; init; }
        public Guid ManagerId { get; init; }
        public DateTime AssignmentDate { get; init; }
    }

    public record TeamMemberRemovedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public Guid TeamId { get; init; }
        public DateTime RemovalDate { get; init; }
    }
}