# Requirements:
- .NET 8.0 or later
- Passwordless.dev Pro or Enterprise

# Getting started
This example demonstrates how to use the Passwordless library in a multi-tenant environment. Where the subdomain is used
to identify the tenant. For example:

- `republicans.passwordless.local`
- `democrats.passwordless.local`

We can achieve this by bootstrapping the Passwordless SDK ourselves in `Program.cs` and providing the necessary
configuration.

For a tenant `democrats` or `republicants` respectively. You would find the configuration then in your appsettings.json
file. Similarly, you can use a database or any other configuration source.

```json
{
    "Passwordless": {
        "Tenants": {
            "republicans": {
                "ApiSecret": "republicans:secret:00000000000000000000000000000000"
            },
            "democrats": {
                "ApiSecret": "democrats:secret:00000000000000000000000000000000"
            }
        }
    }
}
```

1. Create entries in the hosts file:

    127.0.0.1 republicans.passwordless.local
    127.0.0.1 democrats.passwordless.local

    These are the tenants of your own backend as an example named 'republicans' and 'democrats'

2. Modify any tenants and their related `ApiSecret` setting in the `appsettings.json` file.

3. Run the sample locally with .NET 8, debug if you have to step through. And visit:

   - http://republicans.passwordless.local/swagger/index.html
   - http://democrats.passwordless.local/swagger/index.html

4. Call the 'Users' endpoint from Swagger to test using your own API secrets obtained.

You can refer to the `Passwordless.Example` project how to create your own complete backend with the Passwordless
library, as this example is a stripped-down version of it to demonstrate multi-tenancy in a simple way.