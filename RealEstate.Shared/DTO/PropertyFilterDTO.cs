namespace RealEstate.Shared.DTO
{
    public class PropertyFilterDTO
    {
        public int? IdOwner { get; set; }
        public string? CodeInternal { get; set; }
        public string? NameContains { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public short? MinYear { get; set; }
        public short? MaxYear { get; set; }
        public bool? HasImages { get; set; }
        public bool? ImageEnabled { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool TrackEntities { get; set; } = false;
    }
}
