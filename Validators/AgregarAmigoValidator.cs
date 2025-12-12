using FluentValidation;
namespace TravelFriend;


public class AgregarAmigoValidator: AbstractValidator<AgregarAmigoDto>
{
public AgregarAmigoValidator()
    {
        RuleFor(x => x.EmailAmigo)
            .NotEmpty().WithMessage("El email del amigo es obligatorio.")
            .EmailAddress().WithMessage("Debes ingresar un formato de correo válido.");
    }
}
