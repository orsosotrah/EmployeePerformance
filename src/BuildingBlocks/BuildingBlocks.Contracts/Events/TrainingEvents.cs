using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.Contracts.Events
{
    // Events untuk Course Management
    public record CourseCreatedEvent : IntegrationEvent
    {
        public Guid CourseId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Category { get; init; }
        public int DurationInHours { get; init; }
        public bool IsMandatory { get; init; }
    }

    public record CourseUpdatedEvent : IntegrationEvent
    {
        public Guid CourseId { get; init; }
        public IDictionary<string, object> ChangedProperties { get; init; }
    }

    // Events untuk Enrollment
    public record CourseEnrollmentCreatedEvent : IntegrationEvent
    {
        public Guid EnrollmentId { get; init; }
        public Guid CourseId { get; init; }
        public Guid EmployeeId { get; init; }
        public DateTime EnrollmentDate { get; init; }
        public DateTime DueDate { get; init; }
    }

    public record CourseCompletedEvent : IntegrationEvent
    {
        public Guid EnrollmentId { get; init; }
        public Guid CourseId { get; init; }
        public Guid EmployeeId { get; init; }
        public decimal Score { get; init; }
        public DateTime CompletionDate { get; init; }
    }

    // Events untuk Progress Tracking
    public record TrainingProgressUpdatedEvent : IntegrationEvent
    {
        public Guid EnrollmentId { get; init; }
        public Guid EmployeeId { get; init; }
        public int ProgressPercentage { get; init; }
        public DateTime LastActivityDate { get; init; }
    }

    // Events untuk Certifications
    public record CertificationIssuedEvent : IntegrationEvent
    {
        public Guid CertificationId { get; init; }
        public Guid EmployeeId { get; init; }
        public string CertificationType { get; init; }
        public DateTime IssueDate { get; init; }
        public DateTime ExpiryDate { get; init; }
    }

    public record CertificationExpiringEvent : IntegrationEvent
    {
        public Guid CertificationId { get; init; }
        public Guid EmployeeId { get; init; }
        public string CertificationType { get; init; }
        public DateTime ExpiryDate { get; init; }
        public int DaysUntilExpiry { get; init; }
    }
}