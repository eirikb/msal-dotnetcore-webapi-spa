Minimal minimum setup in order to use msal to _authenticate_ against a custom dotnet core webapi.  
This means:
  * SPA client hosted away from API.
  * MSAL with JWT, no cookies.
  * No delegated calls to other Microsoft APIs such as Graph.
    * Point here is that the SPA would do this, and ask for those scopes, not the API.
  * Active Directory against own organization, _no_ B2C.
    
    
Project is split into two parts:

#### Client
SPA application  
Created manually with 

```bash
npm init -y
npm i @eirikb/domdom msal
npm i -D parcel
```

No CLI.

Run like this:

```bash
cd client
npm i
npm start
```


#### API
dotnet core webapi  
Created like this:
```bash
dotnet new webapi
dotnet add package Microsoft.AspNetCore.Authentication.AzureAD.UI
```

Added `AzureAd` to [api/appsettings.json](api/appsettings.json):
```json 
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "TenantId": "********-****-****-****-************",
  "ClientId": "api://80ce0b17-ac0b-43f5-add5-cd8c3412b6c9"
}
```

Note `api://` in start of `ClientId`. The ID comes from Azure AD App, see below.

Changed [api/Startup.cs](api/Startup.cs) to include this:

```c#
services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
    .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
```

And added cors:

```c#
app.UseCors(options => options.AllowCredentials().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
```

Run like this:

```bash
cd api
dotnet run
```


##### Azure AD App

  * MSAL love multi tenant, this makes configuration easier.
  * Redirect URI points at local SPA location. 
  When SPA is deployed somewhere on the internet that URI must be added as well.  
    If not then an error about Redirect URI will appear after login.
    
![create app](https://i.imgur.com/TjkYrCO.png)

After app is created:
  * Copy Application (client) ID into:
    * `clientId` in [client/src/app.jsx](client/src/app.jsx)
    * `ClientId` in [api/appsettings.json](api/appsettings.json)
  * Copy Directory (tenant) ID into:
    * `TenantId` in [api/appsettings.json](api/appsettings.json)
    
![app created](https://i.imgur.com/B8ND9DH.png)

Add support for Implicit Grant in "Authentication":
  * Check off "Access tokens" and "ID tokens".
    
![allow-implicit-grant](https://i.imgur.com/XwTzAIK.png)


Expose API, Add new Scope. Accept the default and click "Save and continue"

![expose-api](https://i.imgur.com/9omjZPJ.png)

  * Add a scope. Make up something in Scope name
  * Copy scope URI and paste it into 
    * `scopes` in [client/src/app.jsx](client/src/app.jsx)
  
![create-scope](https://i.imgur.com/eqwRAqR.png)
