using Injectio.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Bilby.Services;

public interface IPasswordHasher : IPasswordHasher<string>
{
    string HashPassword(string password);
    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}

[RegisterSingleton<IPasswordHasher>]
public class PasswordHasher : PasswordHasher<string>, IPasswordHasher
{
    public string HashPassword(string password) =>
        HashPassword(string.Empty, password);

    public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword) =>
        VerifyHashedPassword(string.Empty, hashedPassword, providedPassword);
}
