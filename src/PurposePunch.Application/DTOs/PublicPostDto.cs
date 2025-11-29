using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.DTOs;

public record PublicPostDto(
    int Id,
    string AuthorNickname,
    string Title,
    string Description,
    string? ActualOutcome,
    string? LessonsLearned,
    SatisfactionScale? Satisfaction,
    int UpvoteCount,
    DateTime PublishedAt
);
