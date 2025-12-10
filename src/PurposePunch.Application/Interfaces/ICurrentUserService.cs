namespace PurposePunch.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? VoterIdentifier { get; }
}
