namespace HR.Application.Interfaces.Authentication;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(
        string userId,
        string email,
        IEnumerable<string> roles);
}