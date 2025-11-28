using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.Features.PublicPosts;

namespace PurposePunch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublicPostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicPostsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] GetPublicPostsQuery query)
    {
        var pagedResult = await _mediator.Send(query);
        return Ok(pagedResult);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var post = await _mediator.Send(new GetPublicPostByIdQuery(id));

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpPost("{id}/upvote")]
    [AllowAnonymous]
    public async Task<IActionResult> Upvote(int id)
    {
        var newCount = await _mediator.Send(new UpvotePostCommand(id));

        if (newCount == null)
            return NotFound();

        return Ok(new { HelpfulCount = newCount });
    }
}
