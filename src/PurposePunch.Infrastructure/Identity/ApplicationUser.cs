using Microsoft.AspNetCore.Identity;

namespace PurposePunch.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? AnonymousNickname { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
