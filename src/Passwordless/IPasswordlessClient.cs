﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Passwordless.Models;

namespace Passwordless;

/// <summary>
/// Provides APIs that help you interact with Passwordless.dev.
/// </summary>
public interface IPasswordlessClient
{
    /// <summary>
    /// Creates a <see cref="RegisterTokenResponse" /> which will be used by your frontend to negotiate
    /// the creation of a WebAuth credential.
    /// </summary>
    /// <param name="options">The <see cref="RegisterOptions"/> that will be used to configure your token.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="RegisterTokenResponse" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<RegisterTokenResponse> CreateRegisterTokenAsync(
        RegisterOptions options,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a <see cref="SigninTokenResponse" /> which can be used to authenticate on behalf of a user.
    /// </summary>
    /// <param name="options">The <see cref="SigninOptions"/> that will be used to configure your token.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="SigninTokenResponse" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<SigninTokenResponse> CreateSigninTokenAsync(
        SigninOptions options,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Verifies that the given token is valid and returns information packed into it.
    /// The token should have been generated via calling a <c>signInWith*</c> method from your frontend code.
    /// If the token is not valid, an exception of type <see cref="PasswordlessApiException" /> will be thrown.
    /// </summary>
    /// <param name="verifyToken">The token to verify.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="VerifiedUser" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<VerifiedUser> VerifyTokenAsync(
        string verifyToken,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the users count.
    /// </summary>
    Task<UsersCount> GetUsersCountAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// List all the <see cref="PasswordlessUserSummary" /> for the account associated with your ApiSecret.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="IReadOnlyList{PasswordlessUserSummary}" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<IReadOnlyList<PasswordlessUserSummary>> ListUsersAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="userId">The id of the user that should be deleted.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task DeleteUserAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// List all the <see cref="AliasPointer" /> for a given user.
    /// </summary>
    /// <param name="userId">The userId of the user for which the aliases will be returned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="IReadOnlyList{AliasPointer}" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<IReadOnlyList<AliasPointer>> ListAliasesAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sets one or more aliases to an existing user and removes existing aliases that are not included in the request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetAliasAsync(
        SetAliasRequest request,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// List all the <see cref="Credential" /> for a given user.
    /// </summary>
    /// <param name="userId">The userId of the user for which the credentials will be returned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation containing the <see cref="IReadOnlyList{Credential}" />.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task<IReadOnlyList<Credential>> ListCredentialsAsync(
        string userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Attempts to delete a credential via the supplied id.
    /// </summary>
    /// <param name="id">The id of a credential representing as a Base64 URL encoded <see cref="string" />.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task DeleteCredentialAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Attempts to delete a credential via the supplied id.
    /// </summary>
    /// <param name="id">The id of a credential representing as a Base64 URL encoded <see cref="T:byte[]" />.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task object representing the asynchronous operation.</returns>
    /// <exception cref="PasswordlessApiException">An exception containing details about the reason for failure.</exception>
    Task DeleteCredentialAsync(
        byte[] id,
        CancellationToken cancellationToken = default
    );
}