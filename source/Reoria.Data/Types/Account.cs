using Reoria.Data.Types.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Reoria.Data.Types;

/// <summary>
/// Defines <see cref="Account"/> data and functions which are used to store player login information.
/// </summary>
/// <remarks>Do not change anything related to passwords, salts, or hashing directly unless you know what you are doing. You will make it impossible to authenticate into the account.</remarks>
public class Account : IAccount
{
    /// <summary>
    /// Defines the salt key size that new accounts and updated passwords should use.
    /// </summary>
    public const int SALT_KEY_SIZE = 64;
    /// <summary>
    /// Defines the number of hash iterations that new accounts and updated passwords should use.
    /// </summary>
    public const int HASH_ITERATIONS = 350000;
    /// <summary>
    /// Defines the hashing algorithm that new accounts and updated passwords should use.
    /// </summary>
    public static readonly HashAlgorithmName HASH_ALGORITHM = HashAlgorithmName.SHA512;

    /// <summary>
    /// The email address the player uses to authenticate into the game with.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// The salted and hashed password the player uses to authenticate into the game with.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    public string Password { get; set; }
    /// <summary>
    /// The password salt that was generated when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    public string Salt { get; set; }
    /// <summary>
    /// The salt key size that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    public int SaltKeySize { get; set; }
    /// <summary>
    /// The number of hash iterations that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    public int HashIterations { get; set; }
    /// <summary>
    /// The hashing algorithm that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    public HashAlgorithmName HashAlgorithm { get; set; }

    /// <summary>
    /// Constructs a new account data instance.
    /// </summary>
    public Account()
    {
        // Assign default values to the properties.
        this.Email = string.Empty;
        this.Password = string.Empty;
        this.Salt = string.Empty;
        this.SaltKeySize = default;
        this.HashIterations = default;
        this.HashAlgorithm = default;
    }

    /// <summary>
    /// Changes the email address the player uses to authenticate into the game with.
    /// </summary>
    /// <param name="newEmail">The new email address the player wishes to authenticate into the game with.</param>
    /// <exception cref="ArgumentException"></exception>
    public void ChangeEmail(string newEmail)
    {
        // Check to see if a new email address was passed.
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            // There was no email address passed, throw an exception.
            throw new ArgumentException($"'{nameof(newEmail)}' cannot be null or whitespace.", nameof(newEmail));
        }

        // Update the email address.
        this.Email = newEmail.Trim();
    }

    /// <summary>
    /// Regenerates and changes the salted and hashed password the player uses to authenticate into the game with.
    /// </summary>
    /// <param name="newPassword">The password the player uses to authenticate into the game with.</param>
    /// <exception cref="ArgumentException"></exception>
    public void ChangePassword(string newPassword)
    {
        // Check to see if a new password was passed.
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            // There was no password passed, throw an exception.
            throw new ArgumentException($"'{nameof(newPassword)}' cannot be null or whitespace.", nameof(newPassword));
        }

        // Update the password salt and hashing settings.
        this.SaltKeySize = SALT_KEY_SIZE;
        this.HashIterations = HASH_ITERATIONS;
        this.HashAlgorithm = HASH_ALGORITHM;

        // Regenerate the salt and hashed password.
        this.Salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(this.SaltKeySize));
        this.Password = Convert.ToHexString(HashString(newPassword, this.Salt, this.SaltKeySize, this.HashIterations, this.HashAlgorithm));
    }

    /// <summary>
    /// Verifies that the password the player uses to authenticate into the game with matched the one stored on this account.
    /// </summary>
    /// <param name="challengePassword">The password the player uses to authenticate into the game with.</param>
    /// <returns>True if the challenge password matches the one stored on the account, false under any other result.</returns>
    /// <exception cref="ArgumentException"></exception>
    public bool VerifyPassword(string challengePassword)
    {
        // Check to see if a new password was passed.
        if (string.IsNullOrWhiteSpace(challengePassword))
        {
            // There was no password passed, throw an exception.
            throw new ArgumentException($"'{nameof(challengePassword)}' cannot be null or whitespace.", nameof(challengePassword));
        }

        // Generate a new password hash using the provided password and the known settings for this account's password.
        byte[] hashToCompare = HashString(challengePassword, this.Salt, this.SaltKeySize, this.HashIterations, this.HashAlgorithm);

        // Verify that the passwords match and return the result.
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(this.Password));
    }

    /// <summary>
    /// Generates a salted and hashed string based in the inputs provided.
    /// </summary>
    /// <param name="inputString">The string to salt and hash.</param>
    /// <param name="salt">The random salt to salt the input string with.</param>
    /// <param name="keySize">The size of the random salt.</param>
    /// <param name="iterations">The number of iterations to use when hashing.</param>
    /// <param name="hashAlgorithm">The hashing algorithm to use when hashing.</param>
    /// <returns>The salted and hashed string based in the inputs provided.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static byte[] HashString(string inputString, string salt, int keySize, int iterations, HashAlgorithmName hashAlgorithm)
    {
        // Check to see if a new password was passed.
        if (string.IsNullOrWhiteSpace(inputString))
        {
            // There was no password passed, throw an exception.
            throw new ArgumentException($"'{nameof(inputString)}' cannot be null or whitespace.", nameof(inputString));
        }

        // Check to see if a new salt was passed.
        if (string.IsNullOrWhiteSpace(salt))
        {
            // There was no password passed, throw an exception.
            throw new ArgumentException($"'{nameof(salt)}' cannot be null or whitespace.", nameof(salt));
        }

        // Convert the input password and salt to byte arrays.
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

        // Hash and return the password.
        return Rfc2898DeriveBytes.Pbkdf2(inputBytes, saltBytes, iterations, hashAlgorithm, keySize);
    }
}
