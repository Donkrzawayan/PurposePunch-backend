namespace PurposePunch.Domain.Entities;

public class Decision
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ExpectedOutcome { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpectedAt { get; set; } = DateTime.UtcNow.AddDays(30);
    public required string UserId { get; set; }
}

