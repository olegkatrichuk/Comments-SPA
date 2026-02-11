using Comments.Application.Comments.Commands.CreateComment;
using Comments.Application.Comments.Queries.GetCommentById;
using Comments.Application.Comments.Queries.GetComments;
using Comments.Application.DTOs;
using Comments.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Comments.API.Controllers;

[ApiController]
[Route("api/comments")]
public sealed class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CommentDto>>> GetComments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] SortField sortField = SortField.CreatedAt,
        [FromQuery] SortDirection sortDirection = SortDirection.Descending,
        CancellationToken ct = default)
    {
        var query = new GetCommentsQuery(page, pageSize, sortField, sortDirection);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CommentDto>> GetById(
        Guid id,
        CancellationToken ct = default)
    {
        var query = new GetCommentByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting("post-limit")]
    public async Task<ActionResult<CommentDto>> CreateComment(
        [FromForm] CreateCommentRequest request,
        IFormFile? attachment,
        CancellationToken ct = default)
    {
        Stream? fileStream = null;
        string? fileName = null;
        string? fileContentType = null;

        if (attachment is { Length: > 0 })
        {
            fileStream = attachment.OpenReadStream();
            fileName = attachment.FileName;
            fileContentType = attachment.ContentType;
        }

        try
        {
            var command = new CreateCommentCommand(
                request.UserName,
                request.Email,
                request.HomePage,
                request.Text,
                request.ParentCommentId,
                request.CaptchaKey,
                request.CaptchaAnswer,
                fileStream,
                fileName,
                fileContentType);

            var result = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        finally
        {
            if (fileStream is not null)
                await fileStream.DisposeAsync();
        }
    }
}
