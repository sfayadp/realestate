namespace RealEstate.Shared.DTO
{
    public class PropertyDTO
    {
        public int IdProperty { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public decimal Price { get; set; }
        public string CodeInternal { get; set; } = default!;
        public short? Year { get; set; }
        public int IdOwner { get; set; }
        public IEnumerable<PropertyImageDTO>? Images { get; set; }
    }
}
