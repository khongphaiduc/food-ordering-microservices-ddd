namespace auth_services.AuthService.Application.DTOS
{
    public class AddAccountStaffDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public Guid IdRole { get; set; }
    }
}
