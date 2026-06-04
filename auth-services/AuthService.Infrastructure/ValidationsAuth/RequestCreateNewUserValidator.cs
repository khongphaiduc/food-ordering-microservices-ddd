using auth_services.AuthService.Application.DTOS;
using FluentValidation;

namespace auth_services.AuthService.Infrastructure.ValidationsAuth
{
    public class RequestCreateNewUserValidator : AbstractValidator<RequestCreateNewUser>
    {
        public RequestCreateNewUserValidator()
        {
            //RuleFor(x => x.UserName)
            //    .NotEmpty().WithMessage("UserName không du?c d? tr?ng.")
            //    .MinimumLength(3).WithMessage("UserName ph?i có ít nh?t 3 ký t?.")
            //    .MaximumLength(50).WithMessage("UserName không du?c vu?t quá 50 ký t?.")
            //    .Matches("^[a-zA-Z0-9_]+$").WithMessage("UserName ch? du?c ch?a ch? cái, s? và d?u g?ch du?i.");

            //RuleFor(x => x.Email)
            //    .NotEmpty().WithMessage("Email không du?c d? tr?ng.")
            //    .EmailAddress().WithMessage("Email không h?p l?.")
            //    .MaximumLength(100).WithMessage("Email không du?c vu?t quá 100 ký t?.");

            //RuleFor(x => x.Password)
            //    .NotEmpty().WithMessage("Password không du?c d? tr?ng.")
            //    .MinimumLength(8).WithMessage("Password ph?i có ít nh?t 8 ký t?.")
            //    .Matches("[A-Z]").WithMessage("Password ph?i có ít nh?t 1 ch? hoa.")
            //    .Matches("[a-z]").WithMessage("Password ph?i có ít nh?t 1 ch? thu?ng.")
            //    .Matches("[0-9]").WithMessage("Password ph?i có ít nh?t 1 ch? s?.")
            //    .Matches("[^a-zA-Z0-9]").WithMessage("Password ph?i có ít nh?t 1 ký t? d?c bi?t.");

            //RuleFor(x => x.ConfirmPassword)
            //    .NotEmpty().WithMessage("ConfirmPassword không du?c d? tr?ng.")
            //    .Equal(x => x.Password).WithMessage("ConfirmPassword không kh?p v?i Password.");
        }
    }
}
