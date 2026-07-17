using FluentValidation;
using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Validators;

public class EditPunchCardValidator : AbstractValidator<EditPunchCardDto>
{
    public EditPunchCardValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.EmployeeName)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200);
    }
}
