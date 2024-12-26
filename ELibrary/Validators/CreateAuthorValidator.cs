using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class CreateAuthorValidator : AbstractValidator<CreateAuthorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAuthorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .Must(IsNameUnique)
                .OverridePropertyName("name")
                .WithName("Name");

            RuleFor(x => x.Email)
                .MinimumLength(3)
                .MaximumLength(100)
                .EmailAddress()
                .OverridePropertyName("email")
                .WithName("Email");
        }

        private bool IsNameUnique(CreateAuthorRequest request, string name)
        {
            return _unitOfWork.AuthorRepository.IsNameUnique(name, null);
        }
    }
}