# Passwordless ASP.NET Identity Example

## Requirements

* .NET 7 SDK
* A free account obtained from [passwordless.dev](https://admin.passwordless.dev/signup).
* [A Passwordless.dev public and private API key pair](https://docs.passwordless.dev/guide/get-started.html#create-an-application).
* An IDE such as Visual Studio or JetBrains Rider.
* A web browser supporting WebAuthn, see [here](https://caniuse.passwordless.dev/).

## Running the example

Please read the documentation here: https://docs.passwordless.dev

1. Get your own API keys here: https://admin.passwordless.dev/signup
2. In `Passwordless.Example/appsettings.json` replace:
   1. `YOUR_API_SECRET` with your own private api key or secret.
   2. `YOUR_API_KEY` with your own public api key.
3. Run the solution with your favorite IDE or with `dotnet run` in the `Passwordless.Example` folder.
