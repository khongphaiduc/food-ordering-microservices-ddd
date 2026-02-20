namespace auth_services.AuthService.Application.DTOS
{
    public class ViewListStaffDTO
    {
        public Guid IdStaff { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public List<string> Role { get; set; } = null!;
    }
}
