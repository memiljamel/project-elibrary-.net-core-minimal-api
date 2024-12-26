using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class UpdateStaffValidator : AbstractValidator<UpdateStaffRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStaffValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100)
                .Must(IsUsernameUnique)
                .OverridePropertyName("username")
                .WithName("Username");

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .MaximumLength(100)
                .OverridePropertyName("password")
                .WithName("Password");

            RuleFor(x => x.PasswordConfirmation)
                .Equal(x => x.Password)
                .OverridePropertyName("passwordConfirmation")
                .WithName("Password Confirmation");

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .OverridePropertyName("name")
                .WithName("Name");

            RuleFor(x => x.StaffNumber)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(16)
                .Must(IsStaffNumberUnique)
                .OverridePropertyName("staffNumber")
                .WithName("Staff Number");

            RuleFor(x => x.AccessLevel)
                .NotNull()
                .IsInEnum()
                .OverridePropertyName("accessLevel")
                .WithName("Access Level");

            RuleFor(x => x.Image)
                .SetValidator(new ImageValidator())
                .OverridePropertyName("image")
                .WithName("Image");
        }

        private bool IsUsernameUnique(UpdateStaffRequest request, string username)
        {
            return _unitOfWork.StaffRepository.IsUsernameUnique(username, request.Id);
        }

        private bool IsStaffNumberUnique(UpdateStaffRequest request, string staffNumber)
        {
            return _unitOfWork.StaffRepository.IsStaffNumberUnique(staffNumber, request.Id);
        }
    }
}