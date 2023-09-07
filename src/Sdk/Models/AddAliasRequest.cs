namespace Passwordless.Net.Models;

public class AddAliasRequest
{
    public string UserId { get; set; }
    public HashSet<string> Aliases { get; set; }

    /// <summary>
    /// If you want your aliases to be available in plain text, set the <see cref="bool"/> false.
    /// </summary>
    public bool Hashing { get; set; } = true;
}