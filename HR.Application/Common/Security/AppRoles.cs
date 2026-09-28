namespace HR.Application.Common.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string HR = "HR";
    public const string Manager = "Manager";
    public const string Employee = "Employee";

    public static readonly string[] All =
    [
        Admin,
        HR,
        Manager,
        Employee
    ];
}