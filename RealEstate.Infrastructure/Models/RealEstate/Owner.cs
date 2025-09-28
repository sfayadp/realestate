using System;
using System.Collections.Generic;

namespace RealEstate.Infrastructure.Models.RealEstate;

public partial class Owner
{
    public int IdOwner { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public byte[]? Photo { get; set; }

    public DateOnly? Birthday { get; set; }

    public virtual ICollection<Property> Property { get; set; } = new List<Property>();
}
