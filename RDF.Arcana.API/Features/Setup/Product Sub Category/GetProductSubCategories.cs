﻿using Microsoft.AspNetCore.Mvc;
using RDF.Arcana.API.Common;
using RDF.Arcana.API.Common.Extension;
using RDF.Arcana.API.Common.Pagination;
using RDF.Arcana.API.Data;
using RDF.Arcana.API.Domain;

namespace RDF.Arcana.API.Features.Setup.Product_Sub_Category;

[Route("api/ProductSubCategory")]
[ApiController]

public class GetProductSubCategories : ControllerBase
{
    private readonly IMediator _mediator;

    public GetProductSubCategories(IMediator mediator)
    {
        _mediator = mediator;
    }

    public class GetProductSubCategoriesQuery : UserParams, IRequest<PagedList<GetProductSubCategoriesResult>>
    {
        public string Search { get; set; }
        public bool? Status { get; set; }
    }

    public class GetProductSubCategoriesResult
    {
        public int Id { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductSubCategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class Handler : IRequestHandler<GetProductSubCategoriesQuery, PagedList<GetProductSubCategoriesResult>>
    {
        private readonly ArcanaDbContext _context;

        public Handler(ArcanaDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<GetProductSubCategoriesResult>> Handle(GetProductSubCategoriesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ProductSubCategory> productSubCategories =
                _context.ProductSubCategories.
                    Include(x => x.ProductCategory)
                    .Include(x => x.AddedByUser);

            if (!string.IsNullOrEmpty(request.Search))
            {
                productSubCategories =
                    productSubCategories.Where(p => p.ProductSubCategoryName.Contains(request.Search));
            }

            if (request.Status != null)
            {
                productSubCategories = productSubCategories.Where(x => x.IsActive == request.Status);
            }

            var result = productSubCategories.Select(x => x.GetProductSubCategoriesResult());

            return await PagedList<GetProductSubCategoriesResult>.CreateAsync(result, request.PageNumber,
                request.PageSize);
        }
    }
    
    [HttpGet("GetProductSubCategories")]
    public async Task<IActionResult> Get([FromQuery]GetProductSubCategoriesQuery query)
    {
        try
        {
            var productSubCategories =  await _mediator.Send(query);
            Response.AddPaginationHeader(
                productSubCategories.CurrentPage,
                productSubCategories.PageSize,
                productSubCategories.TotalCount,
                productSubCategories.TotalPages,
                productSubCategories.HasPreviousPage,
                productSubCategories.HasNextPage
            );

            var result = new
            {
                productSubCategories,
                productSubCategories.CurrentPage,
                productSubCategories.PageSize,
                productSubCategories.TotalCount,
                productSubCategories.TotalPages,
                productSubCategories.HasPreviousPage,
                productSubCategories.HasNextPage

            };

            var successResult = Result.Success(result);
            return Ok(successResult);
        }
        catch (Exception e)
        {
            return Conflict(e.Message);
        }
    }
}