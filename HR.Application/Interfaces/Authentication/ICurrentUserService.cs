namespace HR.Application.Interfaces.Authentication;
public interface ICurrentUserService
{
    string? UserId { get; }
    Task<long?> GetEmployeeIdAsync();
}