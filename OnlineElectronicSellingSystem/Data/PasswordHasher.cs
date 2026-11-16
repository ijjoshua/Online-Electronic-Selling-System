using System.Security.Cryptography;

namespace OnlineElectronicSellingSystem.Data;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 120000, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }
}
