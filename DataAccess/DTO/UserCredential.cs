namespace DataAccess.DTO
{
    public partial class UserCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public bool? SystemAdministration { get; set; }
        public string OrganizationId { get; set; }
        public string RoleId { get; set; }

        public UserCredential()
        {
            SystemAdministration = false;
        }
    }
}