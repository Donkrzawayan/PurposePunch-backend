namespace PurposePunch.Domain.Entities;

public class PostLike
{
    public required int PostId { get; set; }
    public required string VoterIdentifier { get; set; }
}