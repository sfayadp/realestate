using MediatR;
using RealEstate.Application.Property.AddImageProperty.Commands;
using RealEstate.Application.Property.ChangePrice.Commands;
using RealEstate.Application.Property.CreateProperty.Commands;
using RealEstate.Application.Property.UpdateProperty.Commands;
using RealEstate.Application.Property.ListPropertyWithFilters.Querys;
using RealEstate.Shared.DTO;

public static class RealEstateEndpoints
{
    public static void MapRealEstateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/realEstate")
                             .WithTags("RealEstate");

        /// <summary>
        /// Crea una nueva propiedad inmobiliaria con toda su información básica e imágenes
        /// </summary>
        /// <param name="request">Datos completos de la propiedad a crear</param>
        /// <returns>Propiedad creada con su ID generado</returns>
        /// <response code="200">Propiedad creada exitosamente</response>
        /// <response code="400">Datos de validación inválidos</response>
        /// <response code="401">Token de autenticación inválido</response>
        group.MapPost("/CreateProperty", async (ISender sender, PropertyDTO request) =>
        {
            var response = await sender.Send(new CreatePropertyCommand(request));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .Accepts<PropertyDTO>("application/json")
        .Produces<PropertyDTO>(200, "application/json")
        .Produces(400);

        /// <summary>
        /// Agrega una nueva imagen a una propiedad existente
        /// </summary>
        /// <param name="request">Datos de la imagen a agregar (propertyId, imageFile, enabled)</param>
        /// <returns>Propiedad actualizada con la nueva imagen agregada</returns>
        /// <response code="200">Imagen agregada exitosamente</response>
        /// <response code="400">Request inválido o propiedad no encontrada</response>
        /// <response code="401">Token de autenticación inválido</response>
        group.MapPost("/AddImageProperty", async (ISender sender, AddImageRequestDTO request) =>
        {
            if (request == null) return Results.BadRequest();

            var response = await sender.Send(new AddImagePropertyCommand(request));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .Accepts<AddImageRequestDTO>("application/json")
        .Produces<PropertyDTO>(200, "application/json")
        .Produces(400);

        /// <summary>
        /// Actualiza el precio de una propiedad y registra el cambio en el historial de trazas
        /// </summary>
        /// <param name="request">Datos del cambio de precio (propertyId, newPrice, tax, note, dateSale)</param>
        /// <returns>Propiedad con el precio actualizado</returns>
        /// <response code="200">Precio actualizado exitosamente</response>
        /// <response code="400">Request inválido o propiedad no encontrada</response>
        /// <response code="401">Token de autenticación inválido</response>
        group.MapPost("/ChangePrice", async (ISender sender, ChangePriceRequestDTO request) =>
        {
            if (request == null) return Results.BadRequest();

            var response = await sender.Send(new ChangePriceCommand(request));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .Accepts<ChangePriceRequestDTO>("application/json")
        .Produces<PropertyDTO>(200, "application/json")
        .Produces(400);

        /// <summary>
        /// Actualiza los datos de una propiedad existente (nombre, dirección, código interno, año, propietario)
        /// </summary>
        /// <param name="request">Datos a actualizar de la propiedad (idProperty y campos opcionales)</param>
        /// <returns>Propiedad con los datos actualizados</returns>
        /// <response code="200">Propiedad actualizada exitosamente</response>
        /// <response code="400">Request inválido o datos de validación incorrectos</response>
        /// <response code="401">Token de autenticación inválido</response>
        group.MapPost("/UpdateProperty", async (ISender sender, UpdatePropertyRequest request) =>
        {
            if (request == null) return Results.BadRequest();

            var response = await sender.Send(new UpdatePropertyCommand(request));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .Accepts<UpdatePropertyRequest>("application/json")
        .Produces<PropertyDTO>(200, "application/json")
        .Produces(400);

        /// <summary>
        /// Lista propiedades aplicando filtros avanzados y paginación
        /// </summary>
        /// <param name="request">Filtros de búsqueda (idOwner, codeInternal, nameContains, minPrice, maxPrice, minYear, maxYear, hasImages, imageEnabled, page, pageSize)</param>
        /// <returns>Lista paginada de propiedades que coinciden con los filtros</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <response code="400">Request inválido o filtros incorrectos</response>
        /// <response code="401">Token de autenticación inválido</response>
        group.MapPost("/ListPropertyWithFilters", async (ISender sender, PropertyFilterDTO request) =>
        {
            if (request == null) return Results.BadRequest();
    
            var response = await sender.Send(new ListPropertyWithFiltersQuery(request));
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .Accepts<PropertyFilterDTO>("application/json")
        .Produces<PagedResult<PropertyDTO>>(200, "application/json")
        .Produces(400);
    }
}
