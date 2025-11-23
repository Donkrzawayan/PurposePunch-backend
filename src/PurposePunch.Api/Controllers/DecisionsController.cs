using Microsoft.AspNetCore.Mvc;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DecisionsController : ControllerBase
{
    private readonly IDecisionRepository _repository;

    public DecisionsController(IDecisionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var decision = await _repository.GetByIdAsync(id);
        return decision == null ? NotFound() : Ok(decision);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var decisions = await _repository.GetAllAsync();
        return Ok(decisions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string title, string description)
    {
        var decision = new Decision
        {
            Title = title,
            Description = description
        };

        Decision? addedDecision = await _repository.CreateAsync(decision);
        if (addedDecision == null)
            return BadRequest("Could not create decision.");
        return CreatedAtAction(nameof(GetById), new { id = addedDecision.Id }, addedDecision);
    }
}
