using System.Collections.Generic;
using System.Linq;

namespace Passwordless.Models;

public class SetAliasRequest(string userId, HashSet<string> aliases, bool hashing = true)
{
    /// <summary>
    /// Sets a single alias for a given user, and removes any other aliases that may exist.
    /// </summary>
    public SetAliasRequest(string userId, string alias, bool hashing = true)
        : this(userId, [alias], hashing)
    {
    }

    public string UserId { get; } = userId;

    public IReadOnlyCollection<string> Aliases { get; } = aliases == null
        ? []
        : new HashSet<string>(aliases.Where(x => !string.IsNullOrWhiteSpace(x)));

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; } = hashing;

    /// <summary>
    /// Removes all aliases from a user.
    /// </summary>
    /// <param name="userId"></param>
    public static SetAliasRequest Empty(string userId) => new(userId, new HashSet<string>(), true);
}