namespace RealEstate.Shared.DTO
{
    public class AddImageRequestDTO
    {
        public int PropertyId { get; set; }
        public byte[] ImageFile { get; set; } = default!;
        public bool Enabled { get; set; } = true;

    }
}
