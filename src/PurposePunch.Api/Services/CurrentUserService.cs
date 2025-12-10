using PurposePunch.Application.Interfaces;
using System.Security.Claims;

namespace PurposePunch.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const int DeviceIdLength = 36;
    private const string DevicePrefix = "DEVICE:";

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    private string? DeviceId
    {
        get
        {
            var deviceId = _httpContextAccessor.HttpContext?.Request.Headers["X-Device-Id"].ToString();
            if (string.IsNullOrWhiteSpace(deviceId)
                || deviceId.Length != DeviceIdLength
                || !deviceId.All(c => char.IsLetterOrDigit(c) || c == '-'))
                return null;
            return deviceId;
        }
    }

    public string? VoterIdentifier
    {
        get
        {
            if (!string.IsNullOrEmpty(UserId))
                return UserId;
            if (!string.IsNullOrEmpty(DeviceId))
                return $"{DevicePrefix}{DeviceId}";

            return null;
        }
    }
}
