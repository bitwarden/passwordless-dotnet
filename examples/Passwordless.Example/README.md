# Passwordless asp.net example

Please read the documentation here: https://docs.passwordless.dev

1. Get your own API keys here: https://admin.passwordless.dev/signup
2. In `Passwordless.Example/appsettings.Development.json` replace `YOUR_API_SECRET` with your own private api key or secret.
3. (optional) In case of self-hosting, `Passwordless.Example/appsettings.Development.json` replace the value for key `Passwordless__apiUrl` with the base url where your `Passwordless API` instance is running.
4. In `Passwordless.Example/wwwroot/index.html` replace `YOUR_API_KEY` with your own public api key.
5. (optional) In case of self-hosting, `Passwordless.Example/wwwroot/index.html` replace the value of constant `PASSWORDLESS_API_URL` with the base url where your `Passwordless API` instance is running.
6. Replace "YOUR_API_KEY" and YOUR_API_SECRET with your own ApiKey / secret
7. Run via Visual Studio or `dotnet run --project .\passwordless-dotnet-example\`

