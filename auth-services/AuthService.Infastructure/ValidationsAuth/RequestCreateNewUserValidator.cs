using auth_services.AuthService.Application.DTOS;
using FluentValidation;

namespace auth_services.AuthService.Infastructure.ValidationsAuth
{
    public class RequestCreateNewUserValidator : AbstractValidator<RequestCreateNewUser>
    {
        public RequestCreateNewUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName không được để trống.")
                .MinimumLength(3).WithMessage("UserName phải có ít nhất 3 ký tự.")
                .MaximumLength(50).WithMessage("UserName không được vượt quá 50 ký tự.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("UserName chỉ được chứa chữ cái, số và dấu gạch dưới.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống.")
                .EmailAddress().WithMessage("Email không hợp lệ.")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password không được để trống.")
                .MinimumLength(8).WithMessage("Password phải có ít nhất 8 ký tự.")
                .Matches("[A-Z]").WithMessage("Password phải có ít nhất 1 chữ hoa.")
                .Matches("[a-z]").WithMessage("Password phải có ít nhất 1 chữ thường.")
                .Matches("[0-9]").WithMessage("Password phải có ít nhất 1 chữ số.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password phải có ít nhất 1 ký tự đặc biệt.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword không được để trống.")
                .Equal(x => x.Password).WithMessage("ConfirmPassword không khớp với Password.");
        }
    }
}
