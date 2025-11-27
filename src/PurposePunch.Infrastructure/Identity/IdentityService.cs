using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PurposePunch.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PurposePunch.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly INicknameGenerator _nicknameGenerator;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        INicknameGenerator nicknameGenerator)
    {
        _userManager = userManager;
        _configuration = configuration;
        _nicknameGenerator = nicknameGenerator;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts. Please try again later.");

        var result = await _userManager.CheckPasswordAsync(user, password);

        if (!result)
        {
            await _userManager.AccessFailedAsync(user);
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts. Please try again later.");
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        return GenerateJwtToken(user);
    }

    public async Task<(bool IsSuccess, string[] Errors)> RegisterAsync(string email, string password)
    {
        string? nickname = await GenerateNickname();

        if (nickname == null)
            return (false, new[] { "System is busy generating unique nicknames. Please try again later." });

        var user = new ApplicationUser
        {
            AnonymousNickname = nickname,
            UserName = email,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        return (true, Array.Empty<string>());
    }

    public async Task<string?> GetNicknameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.AnonymousNickname;
    }

    private async Task<string?> GenerateNickname()
    {
        string nickname;

        int maxRetries = 10;
        bool isUnique = false;
        do
        {
            nickname = _nicknameGenerator.Generate();
            isUnique = !await _userManager.Users.AnyAsync(u => u.AnonymousNickname == nickname);
            maxRetries -= 1;
        } while (!isUnique && maxRetries > 0);

        return isUnique ? nickname : null;
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
