using Comments.Application.Comments.Queries.SearchComments;
using Comments.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Comments.API.Controllers;

[ApiController]
[Route("api/search")]
public sealed class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CommentDto>>> Search(
        [FromQuery] string q = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var query = new SearchCommentsQuery(q, page, pageSize);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
}
