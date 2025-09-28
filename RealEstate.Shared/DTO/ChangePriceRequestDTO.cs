namespace RealEstate.Shared.DTO
{
    public class ChangePriceRequestDTO
    {
        public int PropertyId { get; set; }
        public decimal NewPrice { get; set; }
        public decimal Tax { get; set; }
        public string? Note { get; set; }
        public DateTime? DateSale { get; set; }
    }
}
