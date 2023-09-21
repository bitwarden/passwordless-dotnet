namespace Passwordless.Models;

public class AddAliasRequest
{
    public AddAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, hashing)
    {
        Aliases = new();
        if (!string.IsNullOrWhiteSpace(alias))
        {
            Aliases = new() { alias };
        }
    }

    public AddAliasRequest(string userId, HashSet<string> aliases, bool hashing = true)
        : this(userId, hashing)
    {
        if (aliases == null) return;

        Aliases = new HashSet<string>(aliases.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    private AddAliasRequest(string userId, bool hashing = true)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Hashing = hashing;
    }

    public string UserId { get; }
    public HashSet<string> Aliases { get; }

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; }
}