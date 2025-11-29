using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.DTOs;

public record DecisionDto(
    int Id,

    // PHASE 1: Defining Decisions
    string Title,
    string Description,
    string ExpectedOutcome,
    Visibility Visibility,

    DateTime CreatedAt,
    DateTime ExpectedReflectionDate,
    DecisionStatus Status,

    // PHASE 2: Reflection
    string? ActualOutcome,
    string? LessonsLearned,
    string? PrivateNotes,
    SatisfactionScale? Satisfaction,
    DateTime? ReflectedAt
);
