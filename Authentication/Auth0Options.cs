using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Auth0.NET.Authentication
{    public class Auth0Options : OpenIdConnectOptions
    {
        public string Domain { get; set; }
        public Auth0Options()
        {
            ResponseType = OpenIdConnectResponseType.Code;

            Scope.Clear();
            Scope.Add("openid");
            Scope.Add("profile");
            Scope.Add("email");

            CallbackPath = Auth0Defaults.CallbackPath;

            ClaimsIssuer = Auth0Defaults.ClaimsIssuer;

            SaveTokens = true;

            TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name"
            };
        }
    }
}
