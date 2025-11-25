using Microsoft.AspNetCore.Identity;

namespace PurposePunch.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
