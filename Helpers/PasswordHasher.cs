// PasswordHasher.cs
using System;
using System.Security.Cryptography;

namespace Spotify.Helpers;

public class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    public static (string hashedPassword, string salt) HashPassword(string password)
    {
        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Create the hash using Rfc2898DeriveBytes (PBKDF2)
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Convert to base64 for storage
            string hashedPassword = Convert.ToBase64String(hash);
            string saltString = Convert.ToBase64String(salt);

            return (hashedPassword, saltString);
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword, string saltString)
    {
        try
        {
            // Convert the salt from base64 string back to bytes
            byte[] salt = Convert.FromBase64String(saltString);

            // Create the hash using the same parameters
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                string newHashedPassword = Convert.ToBase64String(hash);

                // Compare the hashes
                return newHashedPassword == hashedPassword;
            }
        }
        catch
        {
            return false;
        }
    }
}