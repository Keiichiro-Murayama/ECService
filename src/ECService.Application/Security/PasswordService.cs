using Microsoft.AspNetCore.Identity;

namespace ECService.Application.Security;

/// <summary>
/// パスワードのハッシュ化と検証を行うサービス
/// </summary>
public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<string> _passwordHasher;

    public PasswordService(IPasswordHasher<string> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Hash(string rawPassword)
    {
        return _passwordHasher.HashPassword("dummy", rawPassword);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            "dummy",
            hashedPassword,
            providedPassword);

        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}