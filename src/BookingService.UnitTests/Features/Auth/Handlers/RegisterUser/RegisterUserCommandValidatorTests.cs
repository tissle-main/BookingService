using Bogus;
using FluentValidation.TestHelper;
using BookingService.Web.Features.Auth.Handlers.RegisterUser;

namespace BookingService.UnitTests.Features.Auth.Handlers.RegisterUser;

public sealed class RegisterUserCommandValidatorTests
{
    public RegisterUserCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().ValidInstance().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, cancellationToken: cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}