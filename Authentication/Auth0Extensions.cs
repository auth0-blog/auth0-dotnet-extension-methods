using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Auth0.NET.Authentication
{
    public static class Auth0Extensions
    {
        public static AuthenticationBuilder AddAuth0(this AuthenticationBuilder builder, Action<Auth0Options> configureOptions)
        {
            return builder.AddAuth0(Auth0Defaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddAuth0(this AuthenticationBuilder builder, string authenticationScheme, Action<Auth0Options> configureOptions)
        {
            var auth0Options = new Auth0Options();

            configureOptions(auth0Options);

            return builder.AddOpenIdConnect(authenticationScheme, options =>
            {
                options.Authority = $"https://{auth0Options.Domain}";

                options.ClientId = auth0Options.ClientId;
                options.ClientSecret = auth0Options.ClientSecret;

                options.ResponseType = auth0Options.ResponseType;

                options.Scope.Clear();
                foreach(var scope in auth0Options.Scope)
                {
                    options.Scope.Add(scope);
                }

                options.CallbackPath = auth0Options.CallbackPath;

                options.ClaimsIssuer = auth0Options.ClaimsIssuer;

                options.SaveTokens = auth0Options.SaveTokens;

                options.TokenValidationParameters = auth0Options.TokenValidationParameters;

                options.Events = new OpenIdConnectEvents
                {
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"{options.Authority}/v2/logout?client_id={options.ClientId}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}