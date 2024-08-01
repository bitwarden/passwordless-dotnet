1. Create entries in the hosts file:

    127.0.0.1 republicans.passwordless.local
    127.0.0.1 democrats.passwordless.local

    These are the tenants of your own backend as an example named 'republicans' and 'democrats'

2. Modify any tenants and their related `ApiSecret` setting in the `appsettings.json` file.

3. Run the sample locally with .NET 8, debug if you have to step through. And visit:

   - http://republicans.passwordless.local/swagger/index.html
   - http://democrats.passwordless.local/swagger/index.html

4. Call the 'Users' endpoint to test using your own API secrets obtained.