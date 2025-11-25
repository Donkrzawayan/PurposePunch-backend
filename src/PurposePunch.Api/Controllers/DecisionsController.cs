using MediatR;
using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.Features.Decisions;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
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
}
