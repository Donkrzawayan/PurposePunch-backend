namespace PurposePunch.Application.Interfaces;

public interface IIdentityService
{
    Task<string> LoginAsync(string email, string password);
    Task<(bool IsSuccess, string[] Errors)> RegisterAsync(string email, string password);
    Task<string?> GetNicknameAsync(string userId);
}
