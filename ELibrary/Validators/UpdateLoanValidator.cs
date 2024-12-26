using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class UpdateLoanValidator : AbstractValidator<UpdateLoanRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLoanValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.ReturnDate)
                .Equal(DateOnly.FromDateTime(DateTime.Today))
                .OverridePropertyName("returnDate")
                .WithName("Return Date");

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