using NSubstitute;
using PurposePunch.Application.Features.PublicPosts;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Tests.Features.PublicPosts;

public class UpvotePostHandlerTests
{
    private readonly IPublicPostRepository _repoMock;
    private readonly ICurrentUserService _userMock;
    private readonly UpvotePostHandler _handler;

    public UpvotePostHandlerTests()
    {
        _repoMock = Substitute.For<IPublicPostRepository>();
        _userMock = Substitute.For<ICurrentUserService>();
        _handler = new UpvotePostHandler(_repoMock, _userMock);
    }

    [Fact]
    public async Task Handle_ShouldUseDeviceId_WhenUserIsNotLoggedIn()
    {
        // ARRANGE
        var postId = 1;
        var deviceId = "f02f3958-0ee8-4036-b35e-f6b973c6598e";

        _userMock.UserId.Returns((string?)null); // User not logged in
        _userMock.DeviceId.Returns(deviceId);

        var post = new PublicPost { Id = postId, UpvoteCount = 10, AuthorNickname = "Anon", Description = "", Title = "" };
        _repoMock.GetByIdAsync(postId).Returns(post);

        _repoMock.RegisterUpvoteAsync(postId, Arg.Any<string>()).Returns(true);

        // ACT
        await _handler.Handle(new UpvotePostCommand(postId), CancellationToken.None);

        // ASSERT
        await _repoMock.Received(1).RegisterUpvoteAsync(postId, $"DEVICE:{deviceId}");
    }

    [Fact]
    public async Task Handle_ShouldUseUserId_WhenUserIsLoggedIn()
    {
        // ARRANGE
        var userId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        _userMock.UserId.Returns(userId);
        _userMock.DeviceId.Returns("f02f3958-0ee8-4036-b35e-f6b973c6598e");

        var post = new PublicPost { Id = 1, UpvoteCount = 5, AuthorNickname = "Anon", Description = "", Title = "" };
        _repoMock.GetByIdAsync(1).Returns(post);
        _repoMock.RegisterUpvoteAsync(1, Arg.Any<string>()).Returns(true);

        // ACT
        await _handler.Handle(new UpvotePostCommand(1), CancellationToken.None);

        // ASSERT
        await _repoMock.Received(1).RegisterUpvoteAsync(1, userId);
    }
}
