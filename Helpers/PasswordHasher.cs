// PasswordHasher.cs
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Spotify.Helpers;

public static class PasswordHasher
{
    //private const int Iterations = 10000;
    private const int Iterations = 10;
    private const int HashSize = 32; // 256 bits

    public static (string hashedPassword, string salt) HashPassword(string password)
    {
        // Generate a random salt
        string salt = Guid.NewGuid().ToString();

        // Convert GUID string to bytes
        byte[] saltBytes = Guid.Parse(salt).ToByteArray();

        // Create hash
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(HashSize);
            string hashedPassword = Convert.ToBase64String(hash);
            return (hashedPassword, salt);
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword, string saltString)
    {
        try
        {
            // Convert the salt from GUID string to bytes
            byte[] salt = Guid.Parse(saltString).ToByteArray();

            // Create the hash using the same parameters
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                string newHashedPassword = Convert.ToBase64String(hash);
                // Compare the hashes
                return newHashedPassword == hashedPassword;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error verifying password: {ex.Message}");
            return false;
        }
    }
}