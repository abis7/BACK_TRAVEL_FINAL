using FluentValidation;
namespace TravelFriend;

public class CrearViajeValidator : AbstractValidator<CrearViajeDto>
{
    public CrearViajeValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del viaje es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre es muy largo.");

        RuleFor(x => x.Ubicacion)
            .NotEmpty().WithMessage("La ubicación es obligatoria.");

        RuleFor(x => x.ParticipantesIds)
            .NotNull().WithMessage("La lista de participantes no puede ser nula.")
            .Must(lista => lista != null && lista.Count > 0)
            .WithMessage("Debes invitar al menos a una persona.");
    }

}
