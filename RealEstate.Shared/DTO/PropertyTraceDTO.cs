namespace RealEstate.Shared.DTO
{
    public class PropertyTraceDTO
    {
        public int IdPropertyTrace { get; set; }
        public DateOnly DateSale { get; set; }
        public string Name { get; set; } = default!;
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public int IdProperty { get; set; }
    }
}
