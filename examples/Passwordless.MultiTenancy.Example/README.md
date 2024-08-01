# Requirements:
- .NET 8.0 or later
- Passwordless.dev Pro or Enterprise

# Getting started
This example demonstrates how to use the Passwordless library in a multi-tenant environment. Where the subdomain is used
to identify the tenant. For example:

- `gameofthrones.passwordless.local`
- `thewalkingdead.passwordless.local`

We can achieve this by bootstrapping the Passwordless SDK ourselves in `Program.cs` and providing the necessary
configuration.

For a tenant `thewalkingdead` or `republicants` respectively. You would find the configuration then in your appsettings.json
file. Similarly, you can use a database or any other configuration source.

```json
{
    "Passwordless": {
        "Tenants": {
            "gameofthrones": {
                "ApiSecret": "gameofthrones:secret:00000000000000000000000000000000"
            },
            "thewalkingdead": {
                "ApiSecret": "thewalkingdead:secret:00000000000000000000000000000000"
            }
        }
    }
}
```

1. Create entries in the hosts file:

    127.0.0.1 gameofthrones.passwordless.local
    127.0.0.1 thewalkingdead.passwordless.local

    These are the tenants of your own backend as an example named 'gameofthrones' and 'thewalkingdead'

2. Modify any tenants and their related `ApiSecret` setting in the `appsettings.json` file.

3. Run the sample locally with .NET 8, debug if you have to step through. And visit:

   - http://gameofthrones.passwordless.local/swagger/index.html
   - http://thewalkingdead.passwordless.local/swagger/index.html

4. Call the 'Users' endpoint from Swagger to test using your own API secrets obtained.

You can refer to the `Passwordless.Example` project how to create your own complete backend with the Passwordless
library, as this example is a stripped-down version of it to demonstrate multi-tenancy in a simple way.