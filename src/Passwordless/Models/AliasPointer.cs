namespace Passwordless;

public class AliasPointer
{
    public AliasPointer(string userId, string alias, string plaintext)
    {
        UserId = userId;
        Alias = alias;
        Plaintext = plaintext;
    }

    public string UserId { get; }
    public string Alias { get; }
    public string Plaintext { get; }
}