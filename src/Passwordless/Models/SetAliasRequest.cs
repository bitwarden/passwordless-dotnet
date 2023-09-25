namespace Passwordless.Models;

public class SetAliasRequest
{
    public SetAliasRequest(string userId)
        : this(userId, new HashSet<string>(), false)
    {
    }

    /// <summary>
    /// Sets a single alias for a given user, and removes any other aliases that may exist.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="alias"></param>
    /// <param name="hashing"></param>
    public SetAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, new HashSet<string> { alias }, hashing)
    {
    }

    /// <summary>
    /// Sets one or more aliases for a given user, and removes any other aliases that may exist.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="aliases"></param>
    /// <param name="hashing"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public SetAliasRequest(string userId, HashSet<string> aliases, bool hashing = true)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Hashing = hashing;

        Aliases = aliases == null
            ? new HashSet<string>()
            : new HashSet<string>(aliases.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public string UserId { get; }

    public IReadOnlyCollection<string> Aliases { get; }

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; }

    /// <summary>
    /// Removes all aliases from a user.
    /// </summary>
    /// <param name="userId"></param>
    public static SetAliasRequest Empty(string userId)
    {
        return new SetAliasRequest(userId);
    }
}