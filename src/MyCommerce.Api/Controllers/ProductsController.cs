using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Products.Create;
using MyCommerce.Application.Products.Delete;
using MyCommerce.Application.Products.Queries.GetAllProducts;
using MyCommerce.Application.Products.Queries.GetProductById;
using MyCommerce.Application.Products.Update;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

public class ProductsController : ApiController
{
    private readonly CreateProductService _createProductService;
    private readonly GetAllProductsService _getAllProductsService;
    private readonly GetProductByIdService _getProductByIdService;
    private readonly UpdateProductService _updateProductService;
    private readonly DeleteProductService _deleteProductService;

    public ProductsController(
        CreateProductService createProductService,
        GetAllProductsService getAllProductsService,
        GetProductByIdService getProductByIdService,
        UpdateProductService updateProductService,
        DeleteProductService deleteProductService)
    {
        _createProductService = createProductService;
        _getAllProductsService = getAllProductsService;
        _getProductByIdService = getProductByIdService;
        _updateProductService = updateProductService;
        _deleteProductService = deleteProductService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Requires Admin Role
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        [FromServices] IValidator<CreateProductRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(
                validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ));
        }

        var result = await _createProductService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id, 
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _updateProductService.UpdateAsync(id, request, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteProductService.DeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllProductsQuery(pageNumber, pageSize, searchTerm, categoryId, minPrice, maxPrice);
        var result = await _getAllProductsService.GetAllAsync(query, cancellationToken);
        
        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getProductByIdService.GetByIdAsync(new GetProductByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }
}