using FluentValidation;
namespace TravelFriend;

public class CrearGastoValidator : AbstractValidator<CrearGastoDto>
{
    public CrearGastoValidator()
    {
        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("La categoría es obligatoria.");

     

        // Regla Condicional: Si es Porcentaje, la suma debe dar 100
     
    }

}
