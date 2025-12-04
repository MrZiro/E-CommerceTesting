using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Categories.Create;
using MyCommerce.Application.Categories.Queries.GetAllCategories;
using MyCommerce.Application.Categories.Queries.GetCategoryById;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

public class CategoriesController : ApiController
{
    private readonly GetAllCategoriesService _getAllCategoriesService;
    private readonly GetCategoryByIdService _getCategoryByIdService;
    private readonly CreateCategoryService _createCategoryService;

    public CategoriesController(
        GetAllCategoriesService getAllCategoriesService,
        GetCategoryByIdService getCategoryByIdService,
        CreateCategoryService createCategoryService)
    {
        _getAllCategoriesService = getAllCategoriesService;
        _getCategoryByIdService = getCategoryByIdService;
        _createCategoryService = createCategoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _getAllCategoriesService.GetAllAsync(new GetAllCategoriesQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getCategoryByIdService.GetByIdAsync(new GetCategoryByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _createCategoryService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }
}
