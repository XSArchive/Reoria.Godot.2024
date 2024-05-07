using System.Security.Cryptography;

namespace Reoria.Data.Types.Interfaces;


/// <summary>
/// Defines <see cref="IAccount"/> data and functions which are used to store player login information.
/// </summary>
public interface IAccount
{
    /// <summary>
    /// The email address the player uses to authenticate into the game with.
    /// </summary>
    string Email { get; set; }
    /// <summary>
    /// The salted and hashed password the player uses to authenticate into the game with.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    string Password { get; set; }
    /// <summary>
    /// The password salt that was generated when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    string Salt { get; set; }
    /// <summary>
    /// The salt key size that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    int SaltKeySize { get; set; }
    /// <summary>
    /// The number of hash iterations that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    int HashIterations { get; set; }
    /// <summary>
    /// The hashing algorithm that was used when the password was last generated.
    /// </summary>
    /// <remarks>Do not change this directly unless you know what you are doing. You will make it impossible to authenticate into this account.</remarks>
    HashAlgorithmName HashAlgorithm { get; set; }

    /// <summary>
    /// Changes the email address the player uses to authenticate into the game with.
    /// </summary>
    /// <param name="newEmail">The new email address the player wishes to authenticate into the game with.</param>
    /// <exception cref="ArgumentException"></exception>
    void ChangeEmail(string newEmail);
    /// <summary>
    /// Regenerates and changes the salted and hashed password the player uses to authenticate into the game with.
    /// </summary>
    /// <param name="newPassword">The password the player uses to authenticate into the game with.</param>
    /// <exception cref="ArgumentException"></exception>
    void ChangePassword(string newPassword);
    /// <summary>
    /// Verifies that the password the player uses to authenticate into the game with matched the one stored on this account.
    /// </summary>
    /// <param name="challengePassword">The password the player uses to authenticate into the game with.</param>
    /// <returns>True if the challenge password matches the one stored on the account, false under any other result.</returns>
    /// <exception cref="ArgumentException"></exception>
    bool VerifyPassword(string rawPassword);
}
