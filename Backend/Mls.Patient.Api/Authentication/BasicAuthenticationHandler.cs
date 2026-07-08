using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Mls.Patient.Api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("En-tête Authorization manquant."));
            }

            AuthenticationHeaderValue? headerValue;

            if (!AuthenticationHeaderValue.TryParse(authorizationHeader.ToString(), out headerValue)
                || !"Basic".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(headerValue.Parameter))
            {
                return Task.FromResult(AuthenticateResult.Fail("Schéma d'authentification invalide."));
            }

            string username;
            string password;

            try
            {
                var credentialBytes = Convert.FromBase64String(headerValue.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                if (credentials.Length != 2)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Format des identifiants invalide."));
                }

                username = credentials[0];
                password = credentials[1];
            }
            catch (FormatException)
            {
                return Task.FromResult(AuthenticateResult.Fail("Format des identifiants invalide."));
            }

            var user = InMemoryUserStore.FindByUsername(username);

            if (user is null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Identifiants invalides."));
            }

            var hasher = new PasswordHasher<ApiUser>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return Task.FromResult(AuthenticateResult.Fail("Identifiants invalides."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Mls.Patient.Api\"");
            return base.HandleChallengeAsync(properties);
        }
    }
}
