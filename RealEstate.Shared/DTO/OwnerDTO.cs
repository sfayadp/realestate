namespace RealEstate.Shared.DTO
{
    public class OwnerDTO
    {
        public int IdOwner { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public byte[]? Photo { get; set; }
        public DateOnly? Birthday { get; set; }
    }
}
