namespace Passwordless.Net.Models;

internal class DeleteCredentialRequest
{
    public DeleteCredentialRequest(string credentialId)
    {
        CredentialId = credentialId;
    }

    public string CredentialId { get; }
}