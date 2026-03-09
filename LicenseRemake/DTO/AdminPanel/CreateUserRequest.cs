namespace LicenseRemake.DTO.AdminPanel
{
    public record CreateUserRequest(
        string UserName,
        string Password,
        int UserTypeId
    );
}
