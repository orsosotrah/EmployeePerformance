namespace BuildingBlocks.Contracts.Models
{
    public record BaseDto
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    public record AuditableDto : BaseDto
    {
        public string CreatedBy { get; init; }
        public string UpdatedBy { get; init; }
    }

    public record AddressDto
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string Country { get; init; }
        public string PostalCode { get; init; }
    }

    public record ContactInfoDto
    {
        public string Email { get; init; }
        public string Phone { get; init; }
        public AddressDto Address { get; init; }
    }
}