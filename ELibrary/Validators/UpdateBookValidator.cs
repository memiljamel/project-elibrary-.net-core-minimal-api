﻿using ELibrary.Models;
using ELibrary.Repositories;
using FluentValidation;

namespace ELibrary.Validators
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .OverridePropertyName("title")
                .WithName("Title");

            RuleFor(x => x.Category)
                .NotNull()
                .IsInEnum()
                .OverridePropertyName("category")
                .WithName("Category");

            RuleFor(x => x.Publisher)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .OverridePropertyName("publisher")
                .WithName("Publisher");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .GreaterThan(0)
                .OverridePropertyName("quantity")
                .WithName("Quantity");

            RuleFor(x => x.AuthorIds)
                .NotEmpty()
                .Must(IsAuthorIdsUnique)
                .ForEach(authorId =>
                {
                    authorId.NotEqual(Guid.Empty)
                        .Must(IsAuthorExists);
                })
                .OverridePropertyName("authorIds")
                .WithName("Author Ids");

            RuleFor(x => x.Image)
                .SetValidator(new ImageValidator())
                .OverridePropertyName("image")
                .WithName("Image");
        }
        
        private bool IsAuthorIdsUnique(List<Guid> ids)
        {
            return ids.Distinct().Count() == ids.Count();
        }

        private bool IsAuthorExists(Guid id)
        {
            return _unitOfWork.AuthorRepository.IsAuthorExists(id);
        }
    }
}