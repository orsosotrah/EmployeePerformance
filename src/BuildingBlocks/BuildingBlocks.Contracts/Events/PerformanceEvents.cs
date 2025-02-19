using BuildingBlocks.EventBus.Events;

namespace BuildingBlocks.Contracts.Events
{
    // Events untuk Review Process
    public record PerformanceReviewCreatedEvent : IntegrationEvent
    {
        public Guid ReviewId { get; init; }
        public Guid EmployeeId { get; init; }
        public string ReviewPeriod { get; init; }
        public DateTime DueDate { get; init; }
    }

    public record PerformanceReviewSubmittedEvent : IntegrationEvent
    {
        public Guid ReviewId { get; init; }
        public Guid EmployeeId { get; init; }
        public decimal OverallScore { get; init; }
        public string Status { get; init; }
        public DateTime SubmissionDate { get; init; }
    }

    public record PerformanceReviewApprovedEvent : IntegrationEvent
    {
        public Guid ReviewId { get; init; }
        public Guid EmployeeId { get; init; }
        public Guid ApproverId { get; init; }
        public DateTime ApprovalDate { get; init; }
    }

    // Events untuk KPI Updates
    public record KpiUpdatedEvent : IntegrationEvent
    {
        public Guid KpiId { get; init; }
        public Guid EmployeeId { get; init; }
        public string Metric { get; init; }
        public decimal Target { get; init; }
        public decimal Achievement { get; init; }
        public DateTime UpdateDate { get; init; }
    }

    // Events untuk Performance Alerts
    public record PerformanceAlertRaisedEvent : IntegrationEvent
    {
        public Guid EmployeeId { get; init; }
        public string AlertType { get; init; }
        public string Description { get; init; }
        public DateTime AlertDate { get; init; }
    }
}