using FluentValidation.TestHelper;
using PurposePunch.Application.Features.Decisions;
using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.Tests.Features.Decisions;

public class CreateDecisionValidatorTests
{
    private readonly CreateDecisionValidator _validator;

    public CreateDecisionValidatorTests()
    {
        _validator = new CreateDecisionValidator();
    }

    [Fact]
    public void ShouldHaveError_WhenTitleIsEmpty()
    {
        // 1. ARRANGE
        var nextMonth = DateTime.UtcNow.AddMonths(1);
        var command = new CreateDecisionCommand("", "Desc", "Outcome", Visibility.Private, nextMonth);

        // 2. ACT
        var result = _validator.TestValidate(command);

        // 3. ASSERT
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void ShouldHaveError_WhenDateIsInPast()
    {
        // 1. ARRANGE
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = new CreateDecisionCommand("Title", "Desc", "Outcome", Visibility.Private, pastDate);

        // 2. ACT
        var result = _validator.TestValidate(command);

        // 3. ASSERT
        result.ShouldHaveValidationErrorFor(x => x.ExpectedReflectionDate)
              .WithErrorMessage("Reflection date must be at least 1 hour in the future.");
    }

    [Fact]
    public void ShouldNotHaveError_WhenDateIsFuture()
    {
        // 1. ARRANGE
        var futureDate = DateTime.UtcNow.AddDays(5);
        var command = new CreateDecisionCommand("Title", "Desc", "Outcome", Visibility.Private, futureDate);

        // 2. ACT
        var result = _validator.TestValidate(command);

        // 3. ASSERT
        result.ShouldNotHaveValidationErrorFor(x => x.ExpectedReflectionDate);
    }
}
