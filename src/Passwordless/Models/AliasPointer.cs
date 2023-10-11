namespace Passwordless;

/// <summary>
/// Alias used for asserting a user.
/// </summary>
public class AliasPointer
{
    /// <summary>
    /// Initialize the instance of <see cref="AliasPointer"/>
    /// </summary>
    /// <param name="userId">Identifier for the user</param>
    /// <param name="alias">Hashed alias for the user</param>
    /// <param name="plaintext">Plaintext value of the alias if stored</param>
    public AliasPointer(string userId, string alias, string plaintext)
    {
        UserId = userId;
        Alias = alias;
        Plaintext = plaintext;
    }

    /// <summary>
    /// Identifier for the user
    /// </summary>
    public string UserId { get; }

    /// <summary>
    /// Hashed value of the user's alias
    /// </summary>
    public string Alias { get; }

    /// <summary>
    /// Plaintext value of user's alias.
    /// </summary>
    public string Plaintext { get; }
}