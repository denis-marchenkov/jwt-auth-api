# JWT Token Auth with minimal API


<br/><br/>
This is a bare bones example just to check the way token is created as well as the auth flow and middleware setup.
<br/><br/>
Secret key used for signing token and signature validation lives in appsettings.json which is just for testing purposes. Secret storage or at least environment variables should be utilized for that.
<br/><br/>
The actual authentication flow achieved by registering jwt bearer middleware in authentication builder (AddJwtBearer), utilizing the same signing key, issuer and audience as the token provider. Endpoint authorization is achieved by enabling authorization for respected endpoint route (RequireAuthorization()).