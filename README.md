#Adoxio Connect Framework for Dynamics 365

Implements OrganizationWebProxyClient with Dynamics 365 Server-to-Server authentication.

 - includes a setting manager for saving and loading application settings from a settings.json
 - utlizes Active Directory Authentication Library (ADAL) to retrieve and store tokens
 - implements OrganizationService and OrganizationServiceContext for easy Dynamics SDK functions
 
Application Settings:

    dyn:SdkClientVersion - define your SDK version, if not provided defaults to 8.2
    dyn:ClientId - Azure AD application ID or client ID
    dyn:ClientSecret - Azure AD application secret or client secret
    dyn:Resource - Dynamics 365 instance URI
    dyn:TenantId - Azure AD tenant ID

Supports .NET Framework 4.6.1, additional framework support in later releases

License under GNU LGPLv3