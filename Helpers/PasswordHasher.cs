// PasswordHasher.cs
using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Spotify.Helpers;

/// <summary>
/// Provides methods for hashing and verifying passwords.
/// </summary>
public static class PasswordHasher
{
    //private const int Iterations = 10000;
    private const int Iterations = 10;
    private const int HashSize = 32; // 256 bits

    /// <summary>
    /// Hashes the specified password and generates a salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A tuple containing the hashed password and the salt.</returns>
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

    /// <summary>
    /// Verifies the specified password against the hashed password and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <param name="saltString">The salt used to hash the password.</param>
    /// <returns><c>true</c> if the password is valid; otherwise, <c>false</c>.</returns>
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
