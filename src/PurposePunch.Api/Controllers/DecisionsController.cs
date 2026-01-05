using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.DTOs;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DecisionDto>> GetById(int id)
    {
        var query = new GetDecisionByIdQuery(id);
        var decision = await _mediator.Send(query);

        if (decision == null)
            return NotFound();

        return Ok(decision);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DecisionDto>>> GetAll()
    {
        var query = new GetAllDecisionsQuery();
        var decisions = await _mediator.Send(query);
        return Ok(decisions);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDecisionCommand command)
    {
        var addedDecision = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = addedDecision.Id }, addedDecision);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDecisionCommand command)
    {
        var commandWithId = command with { Id = id };
        var success = await _mediator.Send(commandWithId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteDecisionCommand(id);
        var success = await _mediator.Send(command);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(int id)
    {
        var postId = await _mediator.Send(new PublishDecisionCommand(id));

        if (postId == null)
            return NotFound();

        return Ok(new { PublicPostId = postId, Message = "Decision published successfully!" });
    }
}
