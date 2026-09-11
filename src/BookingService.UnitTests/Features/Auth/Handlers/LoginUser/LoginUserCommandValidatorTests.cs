using Bogus;
using FluentValidation.TestHelper;
using BookingService.Web.Features.Auth.Handlers.LoginUser;

namespace BookingService.UnitTests.Features.Auth.Handlers.LoginUser;

public sealed class LoginUserCommandValidatorTests
{
    public LoginUserCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}