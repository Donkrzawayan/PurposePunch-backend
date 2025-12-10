using FluentAssertions;
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
    public async Task Handle_ShouldPassVoterIdentifierToRepo_WhenIdentifierExists()
    {
        // ARRANGE
        var postId = 1;
        var expectedVoterId = "DEVICE:f02f3958-0ee8-4036-b35e-f6b973c6598e";
        _userMock.VoterIdentifier.Returns(expectedVoterId);

        var post = new PublicPost { Id = postId, UpvoteCount = 10, AuthorNickname = "Anon", Description = "", Title = "" };
        _repoMock.GetByIdAsync(postId).Returns(post);
        _repoMock.RegisterUpvoteAsync(postId, Arg.Any<string>()).Returns(true);

        // ACT
        await _handler.Handle(new UpvotePostCommand(postId), CancellationToken.None);

        // ASSERT
        await _repoMock.Received(1).RegisterUpvoteAsync(postId, expectedVoterId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenVoterIdentifierIsNull()
    {
        // ARRANGE
        _userMock.VoterIdentifier.Returns((string?)null);

        // ACT
        var result = await _handler.Handle(new UpvotePostCommand(1), CancellationToken.None);

        // ASSERT
        result.Should().BeNull();
        await _repoMock.DidNotReceive().RegisterUpvoteAsync(Arg.Any<int>(), Arg.Any<string>());
    }
}
