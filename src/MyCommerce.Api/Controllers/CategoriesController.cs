using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCommerce.Application.Categories.Create;
using MyCommerce.Application.Categories.Delete;
using MyCommerce.Application.Categories.Queries.GetAllCategories;
using MyCommerce.Application.Categories.Queries.GetCategoryById;
using MyCommerce.Application.Categories.Update;
using MyCommerce.Domain.Common.Result;

namespace MyCommerce.Api.Controllers;

public class CategoriesController : ApiController
{
    private readonly GetAllCategoriesService _getAllCategoriesService;
    private readonly GetCategoryByIdService _getCategoryByIdService;
    private readonly CreateCategoryService _createCategoryService;
    private readonly UpdateCategoryService _updateCategoryService;
    private readonly DeleteCategoryService _deleteCategoryService;

    public CategoriesController(
        GetAllCategoriesService getAllCategoriesService,
        GetCategoryByIdService getCategoryByIdService,
        CreateCategoryService createCategoryService,
        UpdateCategoryService updateCategoryService,
        DeleteCategoryService deleteCategoryService)
    {
        _getAllCategoriesService = getAllCategoriesService;
        _getCategoryByIdService = getCategoryByIdService;
        _createCategoryService = createCategoryService;
        _updateCategoryService = updateCategoryService;
        _deleteCategoryService = deleteCategoryService;
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _updateCategoryService.UpdateAsync(id, request, cancellationToken);

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
        var result = await _deleteCategoryService.DeleteAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return Problem(result.Errors.ToList());
        }

        return NoContent();
    }
}