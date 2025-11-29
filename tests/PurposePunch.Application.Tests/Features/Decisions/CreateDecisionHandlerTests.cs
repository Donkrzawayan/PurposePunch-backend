using FluentAssertions;
using NSubstitute;
using PurposePunch.Application.Features.Decisions;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.Tests.Features.Decisions;

public class CreateDecisionHandlerTests
{
    private readonly IDecisionRepository _repoMock;
    private readonly ICurrentUserService _userMock;

    private readonly CreateDecisionHandler _handler;

    public CreateDecisionHandlerTests()
    {
        _repoMock = Substitute.For<IDecisionRepository>();
        _userMock = Substitute.For<ICurrentUserService>();

        _handler = new CreateDecisionHandler(_repoMock, _userMock);
    }

    [Fact]
    public async Task Handle_ShouldCreateDecision_WhenUserIsLoggedIn()
    {
        // 1. ARRANGE
        var userId = "user-123";
        _userMock.UserId.Returns(userId);

        var command = new CreateDecisionCommand(
            Title: "Test Title",
            Description: "Desc",
            ExpectedOutcome: "Success",
            Visibility: Visibility.Private,
            ExpectedReflectionDate: DateTime.UtcNow.AddMonths(1)
        );

        // 2. ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // 3. ASSERT
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);
        result.UserId.Should().Be(userId);
        result.Status.Should().Be(DecisionStatus.Active);

        await _repoMock.Received(1)
            .CreateAsync(Arg.Is<Decision>(d => d.UserId == userId && d.Title == "Test Title"));
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotLoggedIn()
    {
        // 1. ARRANGE
        _userMock.UserId.Returns((string?)null);

        var nextMonth = DateTime.UtcNow.AddMonths(1);
        var command = new CreateDecisionCommand("Title", "Desc", "Outcome", Visibility.Private, nextMonth);

        // 2. ACT & ASSERT
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not logged in.");

        await _repoMock.DidNotReceive().CreateAsync(Arg.Any<Decision>());
    }
}
