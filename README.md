# Adoxio Connect Framework for Dynamics 365

Implements OrganizationWebProxyClient with Dynamics 365 Server-to-Server authentication.

 * includes a setting manager for saving and loading application settings from a settings.json
 * utlizes Active Directory Authentication Library (ADAL) to retrieve and store tokens
 * implements OrganizationService and OrganizationServiceContext for easy Dynamics SDK functions
 
Application Settings:

```
	dyn:SdkClientVersion - define your SDK version, if not provided defaults to 8.2
    dyn:ClientId - Azure AD application ID or client ID
    dyn:ClientSecret - Azure AD application secret or client secret
    dyn:Resource - Dynamics 365 instance URI
    dyn:TenantId - Azure AD tenant ID
```

Supports .NET Framework 4.6.1, additional framework support in later releases

Licensed under GNU LGPLv3

### Installation

To install use NuGet Package Manager and search for Adoxio.Dynamics.Connect or package manager console and run the following command:

```
Install-Package Adoxio.Dynamics.Connect
```

NuGet: https://www.nuget.org/packages/Adoxio.Dynamics.Connect

### Samples included:

 * Console Application
 * ASP.NET Web App with MVC and WebAPI

#### Release Notes:

0.3.3 - March 14, 2017
 * Initial public release of Adoxio Connect Framework for Dynamics 365.

0.4.0 - March 26, 2017 
 * Add Unit Test project with constructor and S2SAppSetting tests
 * Add and update CrmContext constructors to standard pattern
 * Add assert of S2SAppSettings
 * Add reset token cache method to allow manual clearing of ADAL token cache

0.4.1 - March 26, 2017 
 * Fix parameter order on string create method of CrmContext
 * Resolve public key issue on signing