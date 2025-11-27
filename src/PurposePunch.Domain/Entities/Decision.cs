using PurposePunch.Domain.Enums;

namespace PurposePunch.Domain.Entities;

public class Decision
{
    public int Id { get; set; }

    // PHASE 1: Defining Decisions
    public required string UserId { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ExpectedOutcome { get; set; } = string.Empty;
    public Visibility Visibility { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpectedReflectionDate { get; set; }

    // PHASE 2: Reflection (filled in after the deadline)
    public string? ActualOutcome { get; set; }
    public string? LessonsLearned { get; set; }
    public string? PrivateNotes { get; set; }
    public DateTime? ReflectedAt { get; set; }
    public SatisfactionScale? Satisfaction { get; set; }

    public DecisionStatus Status { get; set; } = DecisionStatus.Active;

}
