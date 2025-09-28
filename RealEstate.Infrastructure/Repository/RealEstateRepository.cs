using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RealEstate.Domain.Contract;
using RealEstate.Infrastructure.Contexts;
using RealEstate.Infrastructure.Models.RealEstate;
using RealEstate.Shared.DTO;

namespace RealEstate.Infrastructure.Repository
{
    public class RealEstateRepository : IRealEstateRepository
    {
        private readonly RealEstateDbContext _dbContext;

        public RealEstateRepository(RealEstateDbContext dbContext) => _dbContext = dbContext;

        public async Task<OwnerDTO> GetOwnerAsync(int ownerId)
        {
            Owner? owner = await _dbContext.Owner.FirstOrDefaultAsync(x => x.IdOwner == ownerId);

            if (owner == null)
            {
                throw new InvalidOperationException($"Owner con ID {ownerId} no existe");
            }

            OwnerDTO response = new OwnerDTO
            {
                IdOwner = owner.IdOwner,
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday
            };

            return response;
        }

        public async Task<PropertyDTO> CreatePropertyBuildingAsync(PropertyDTO property, CancellationToken cancellationToken = default)
        {
            if (property.Price < 0) throw new ArgumentOutOfRangeException(nameof(property.Price));
            if (string.IsNullOrWhiteSpace(property.CodeInternal)) throw new ArgumentException("CodeInternal requerido", nameof(property.CodeInternal));

            bool ownerExists = await _dbContext.Owner.AnyAsync(o => o.IdOwner == property.IdOwner, cancellationToken);
            if (!ownerExists) throw new InvalidOperationException($"Owner {property.IdOwner} no existe");

            Property? entity = new Property
            {
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                IdOwner = property.IdOwner
            };

            if (property.Images != null)
            {
                foreach (PropertyImageDTO? img in property.Images)
                {
                    string base64String = ExtractBase64FromDataUrl(img.File);
                    
                    entity.PropertyImage.Add(new PropertyImage
                    {
                        File = Convert.FromBase64String(base64String),
                        Enabled = img.Enabled
                    });
                }
            }

            _dbContext.Property.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await _dbContext.Entry(entity).ReloadAsync(cancellationToken);
            return ConvertToPropertyDTO(entity);
        }

        public async Task<PropertyDTO> AddImageFromPropertyAsync(int propertyId, byte[] imageFile, bool enabled = true, CancellationToken cancellationToken = default)
        {
            bool exists = await _dbContext.Property.AnyAsync(p => p.IdProperty == propertyId, cancellationToken);
            if (!exists) throw new InvalidOperationException($"Property {propertyId} no existe");

            await using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            PropertyImage? img = new PropertyImage
            {
                IdProperty = propertyId,
                File = imageFile,
                Enabled = enabled
            };
            _dbContext.PropertyImage.Add(img);

            PropertyTrace trace = new PropertyTrace
            {
                IdProperty = propertyId,
                DateSale = DateOnly.FromDateTime(DateTime.Now),
                Name = "Image added",
                Value = 0,
                Tax = 0
            };
            _dbContext.PropertyTrace.Add(trace);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            Property? property = await _dbContext.Property
                .Include(p => p.PropertyImage)
                .FirstOrDefaultAsync(p => p.IdProperty == propertyId, cancellationToken);
            
            return ConvertToPropertyDTO(property!);
        }

        public async Task<PropertyDTO> ChangePriceAsync(int propertyId, decimal newPrice, decimal tax, string note, DateTime? dateSale = null, CancellationToken cancellationToken = default)
        {
            if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice));
            if (tax < 0) throw new ArgumentOutOfRangeException(nameof(tax));

            await using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            Property? prop = await _dbContext.Property.FirstOrDefaultAsync(p => p.IdProperty == propertyId, cancellationToken);
            if (prop == null) throw new InvalidOperationException($"Property {propertyId} no existe");

            prop.Price = newPrice;
            _dbContext.Property.Update(prop);

            PropertyTrace trace = new PropertyTrace
            {
                IdProperty = propertyId,
                DateSale = DateOnly.FromDateTime(DateTime.Now),
                Name = string.IsNullOrWhiteSpace(note) ? "Price change" : note,
                Value = newPrice,
                Tax = tax
            };
            _dbContext.PropertyTrace.Add(trace);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            Property? updatedProperty = await _dbContext.Property
                .Include(p => p.PropertyImage)
                .FirstOrDefaultAsync(p => p.IdProperty == propertyId, cancellationToken);
            
            return ConvertToPropertyDTO(updatedProperty!);
        }

        public async Task<PropertyDTO> UpdatePropertyAsync(int propertyId, PropertyDTO property, CancellationToken cancellationToken = default)
        {
            Property? prop = await _dbContext.Property.FirstOrDefaultAsync(p => p.IdProperty == propertyId, cancellationToken);
            if (prop == null) throw new InvalidOperationException($"Property {propertyId} no existe");

            await using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            bool hasChanges = false;
            if (!string.IsNullOrWhiteSpace(property.Name)) 
            {
                prop.Name = property.Name;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(property.Address)) 
            {
                prop.Address = property.Address;
                hasChanges = true;
            }
            if (!string.IsNullOrWhiteSpace(property.CodeInternal)) 
            {
                prop.CodeInternal = property.CodeInternal;
                hasChanges = true;
            }
            if (property.Year.HasValue) 
            {
                prop.Year = property.Year.Value;
                hasChanges = true;
            }
            if (property.IdOwner > 0)
            {
                bool ownerExists = await _dbContext.Owner.AnyAsync(o => o.IdOwner == property.IdOwner, cancellationToken);
                if (!ownerExists) throw new InvalidOperationException($"Owner {property.IdOwner} no existe");
                prop.IdOwner = property.IdOwner;
                hasChanges = true;
            }
            if (property.Price > 0) 
            {
                prop.Price = property.Price;
                hasChanges = true;
            }

            if (hasChanges)
            {
                _dbContext.Property.Update(prop);

                PropertyTrace trace = new PropertyTrace
                {
                    IdProperty = propertyId,
                    DateSale = DateOnly.FromDateTime(DateTime.Now),
                    Name = "Property updated",
                    Value = property.Price,
                    Tax = 0
                };
                _dbContext.PropertyTrace.Add(trace);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            Property? updatedProperty = await _dbContext.Property
                .Include(p => p.PropertyImage)
                .FirstOrDefaultAsync(p => p.IdProperty == propertyId, cancellationToken);
            
            return ConvertToPropertyDTO(updatedProperty!);
        }

        public async Task<PagedResult<PropertyDTO>> ListPropertyWithFiltersAsync(PropertyFilterDTO filter, CancellationToken cancellationToken = default)
        {
            IQueryable<Property> query = _dbContext.Property.AsQueryable();

            if (!filter.TrackEntities)
                query = query.AsNoTracking();

            if (filter.IdOwner.HasValue)
                query = query.Where(p => p.IdOwner == filter.IdOwner.Value);

            if (!string.IsNullOrWhiteSpace(filter.CodeInternal))
                query = query.Where(p => p.CodeInternal == filter.CodeInternal);

            if (!string.IsNullOrWhiteSpace(filter.NameContains))
                query = query.Where(p => p.Name.Contains(filter.NameContains));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.MinYear.HasValue)
                query = query.Where(p => p.Year >= filter.MinYear.Value);

            if (filter.MaxYear.HasValue)
                query = query.Where(p => p.Year <= filter.MaxYear.Value);

            if (filter.HasImages.HasValue)
            {
                if (filter.HasImages.Value)
                {
                    query = query.Where(p => p.PropertyImage.Any());
                    if (filter.ImageEnabled.HasValue)
                        query = query.Where(p => p.PropertyImage.Any(i => i.Enabled == filter.ImageEnabled.Value));
                }
                else
                {
                    query = query.Where(p => !p.PropertyImage.Any());
                }
            }

            query = query
                .Include(p => p.IdOwnerNavigation)
                .Include(p => p.PropertyImage.Where(i => !filter.ImageEnabled.HasValue || i.Enabled == filter.ImageEnabled.Value));

            int total = await query.CountAsync(cancellationToken);

            int page = Math.Max(1, filter.Page);
            int pageSize = Math.Clamp(filter.PageSize, 1, 200);

            List<PropertyDTO>? items = await query
                .OrderBy(p => p.IdProperty)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PropertyDTO
                {
                    IdProperty = x.IdProperty,
                    Address = x.Address,
                    CodeInternal = x.CodeInternal,
                    IdOwner = x.IdOwner,
                    Name = x.Name,
                    Price = x.Price,
                    Year = x.Year,
                    Images = x.PropertyImage.Any() ? x.PropertyImage.Select(x => new PropertyImageDTO
                    {
                        Enabled = x.Enabled,
                        File = Convert.ToBase64String(x.File),

                    }).ToList() : null,
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<PropertyDTO>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        private PropertyDTO ConvertToPropertyDTO(Property property)
        {
            return new PropertyDTO
            {
                IdProperty = property.IdProperty,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                IdOwner = property.IdOwner,
                Images = property.PropertyImage?.Any() == true 
                    ? property.PropertyImage.Select(img => new PropertyImageDTO
                    {
                        File = Convert.ToBase64String(img.File),
                        Enabled = img.Enabled
                    }).ToList()
                    : null
            };
        }

        private string ExtractBase64FromDataUrl(string dataUrl)
        {
            if (string.IsNullOrEmpty(dataUrl))
                return string.Empty;

            if (dataUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                int base64Index = dataUrl.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
                if (base64Index >= 0)
                {
                    return dataUrl.Substring(base64Index + 7);
                }
            }

            return dataUrl;
        }
    }
}
