using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class CreateMemberValidator : AbstractValidator<CreateMemberRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateMemberValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.MemberNumber)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(16)
                .Must(IsMemberNumberUnique)
                .OverridePropertyName("memberNumber")
                .WithName("Member Number");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .OverridePropertyName("name")
                .WithName("Name");

            RuleFor(x => x.Address)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255)
                .Matches(@"^[\w\s,.\-#]+$")
                .OverridePropertyName("address")
                .WithName("Address");

            RuleFor(x => x.Email)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .EmailAddress()
                .OverridePropertyName("email")
                .WithName("Email");

            RuleFor(x => x.Phones)
                .NotEmpty()
                .ForEach(phone =>
                {
                    phone.MinimumLength(10)
                        .MaximumLength(15)
                        .Matches(@"^\+?[0-9]+$");
                })
                .OverridePropertyName("phones")
                .WithName("Phones");

            RuleFor(x => x.Image)
                .SetValidator(new ImageValidator())
                .OverridePropertyName("image")
                .WithName("Image");
        }

        private bool IsMemberNumberUnique(CreateMemberRequest request, string memberNumber)
        {
            return _unitOfWork.MemberRepository.IsMemberNumberUnique(memberNumber, null);
        }
    }
}