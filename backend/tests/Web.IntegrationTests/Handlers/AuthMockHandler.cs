using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.IntegrationTests.Contexts;

namespace Web.IntegrationTests.Handlers
{
    public class AuthMockHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        private readonly TestAuthContext _auth;

        public AuthMockHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, TestAuthContext auth) : base(options, logger, encoder)
        {
            _auth = auth;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!_auth.IsAuthenticated)
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }

            var identity = new ClaimsIdentity(_auth.Claims, DefaultScheme);

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, DefaultScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}