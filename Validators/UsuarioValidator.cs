namespace TravelFriend;
using FluentValidation;

public class UsuarioValidator: AbstractValidator<Usuario>
{
 public UsuarioValidator()
        {
            RuleFor(u => u.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres.");

            RuleFor(u => u.UsuarioNombre)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 caracteres.")
                .MaximumLength(20).WithMessage("El usuario no puede exceder 20 caracteres.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .EmailAddress().WithMessage("El correo electrónico no es válido.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder 100 caracteres.");
        }
}
