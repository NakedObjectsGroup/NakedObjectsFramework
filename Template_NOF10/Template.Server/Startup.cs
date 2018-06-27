using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System.Web.Http;
using Auth0.Owin;
using AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode;

[assembly: OwinStartup(typeof(Template.Server.Startup))]

namespace Template.Server {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            var domain = $"https://{ConfigurationManager.AppSettings["Auth0Domain"]}/";
            var apiIdentifier = ConfigurationManager.AppSettings["Auth0ApiIdentifier"];

            if (apiIdentifier != "") {
                var keyResolver = new OpenIdConnectSigningKeyResolver(domain);
                app.UseJwtBearerAuthentication(
                    new JwtBearerAuthenticationOptions {
                        AuthenticationMode = AuthenticationMode.Active,
                        TokenValidationParameters = new TokenValidationParameters() {
                            // causes this claim to be used as 'Identity.Name'
                            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                            ValidAudience = apiIdentifier,
                            ValidIssuer = domain,
                            IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) => keyResolver.GetSigningKey(identifier)
                        }
                    });
            }

            // Configure Web API
            WebApiConfig.Configure(app);
            GlobalConfiguration.Configure(CorsConfig.RegisterCors);
        }
    }
}
