using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class CreateLoanValidator : AbstractValidator<CreateLoanRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLoanValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.LoanDate)
                .NotEmpty()
                .Equal(DateOnly.FromDateTime(DateTime.UtcNow))
                .OverridePropertyName("loanDate")
                .WithName("Loan Date");

            RuleFor(x => x.BookId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .Must(IsBookExists)
                .OverridePropertyName("bookId")
                .WithName("Book Id");

            RuleFor(x => x.MemberId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .Must(IsMemberExists)
                .OverridePropertyName("memberId")
                .WithName("Member Id");
        }

        private bool IsBookExists(Guid id)
        {
            return _unitOfWork.BookRepository.IsBookExists(id);
        }

        private bool IsMemberExists(Guid id)
        {
            return _unitOfWork.MemberRepository.IsMemberExists(id);
        }
    }
}