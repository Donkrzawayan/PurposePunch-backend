using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.Features.Decisions;

namespace PurposePunch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DecisionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DecisionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetDecisionByIdQuery(id);
        var decision = await _mediator.Send(query);

        if (decision == null)
            return NotFound();

        return Ok(decision);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllDecisionsQuery();
        var decisions = await _mediator.Send(query);
        return Ok(decisions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDecisionCommand command)
    {
        var addedDecision = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = addedDecision.Id }, addedDecision);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDecisionCommand command)
    {
        var commandWithId = command with { Id = id };
        var success = await _mediator.Send(commandWithId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            var postId = await _mediator.Send(new PublishDecisionCommand(id));

            if (postId == null)
                return NotFound();

            return Ok(new { PublicPostId = postId, Message = "Decision published successfully!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
