using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.Common.Models;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Features.PublicPosts;

namespace PurposePunch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublicPostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicPostsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<PublicPostDto>>> Get([FromQuery] GetPublicPostsQuery query)
    {
        var pagedResult = await _mediator.Send(query);
        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<ActionResult<PublicPostDto>> GetById(int id)
    {
        var post = await _mediator.Send(new GetPublicPostByIdQuery(id));

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpPost("{id}/upvote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> Upvote(int id)
    {
        var newCount = await _mediator.Send(new UpvotePostCommand(id));

        if (newCount == null)
            return NotFound();

        return Ok(new { UpvoteCount = newCount });
    }
}
