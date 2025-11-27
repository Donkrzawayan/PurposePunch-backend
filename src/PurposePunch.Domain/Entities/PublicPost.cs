using PurposePunch.Domain.Enums;

namespace PurposePunch.Domain.Entities;

public class PublicPost
{
    public int Id { get; set; }
    public int? OriginalDecisionId { get; set; }

    public required string AuthorNickname { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? ActualOutcome { get; set; }
    public string? LessonsLearned { get; set; }
    public SatisfactionScale? Satisfaction { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public int HelpfulCount { get; set; } = 0;
}
