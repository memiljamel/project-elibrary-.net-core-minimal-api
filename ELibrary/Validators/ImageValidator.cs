using FluentValidation;

namespace ELibrary.Validators
{
    public class ImageValidator : AbstractValidator<IFormFile>
    {
        public ImageValidator()
        {
            When(x => x != null, () =>
            {
                RuleFor(x => x.Length)
                    .LessThanOrEqualTo(2 * 1024 * 1024);

                RuleFor(x => x.ContentType)
                    .Must(type => new[] { "image/jpeg", "image/jpg", "image/png" }.Contains(type));
            });
        }
    }
}