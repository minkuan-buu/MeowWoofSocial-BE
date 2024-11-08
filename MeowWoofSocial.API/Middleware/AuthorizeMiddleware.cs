using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MeowWoofSocial.API.Middleware
{
    public class AuthorizeMiddleware : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepositories _userRepositories;

        public AuthorizeMiddleware(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IUserRepositories userRepositories)
            : base(options, logger, encoder, clock)
        {
            _userRepositories = userRepositories;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var requestPath = Context.Request.Path;

            // Allow the login endpoint to be bypassed
            if (requestPath.StartsWithSegments("/api/auth/login") || requestPath.StartsWithSegments("/api/auth/register"))
            {
                return AuthenticateResult.NoResult(); // Cho phép request đi qua mà không xác thực
            }

            // Get the Authorization header
            string authorizationHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Authorization header is missing or invalid.");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Validate the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("TestingIssuerSigningKeyPTEducationMS@123"); // Use your JWT signing key here

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true, // Kiểm tra thời gian sống của token
                    ValidIssuer = "TestingJWTIssuerSigningPTEducationMS@123",
                    ValidAudience = "TestingJWTIssuerSigningPTEducationMS@123",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                // Kiểm tra nếu token đã hết hạn
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    return AuthenticateResult.Fail("Token has expired.");
                }

                // Token is valid, create the authentication ticket
                var identity = claimsPrincipal.Identity as ClaimsIdentity;

                if (identity == null || !identity.IsAuthenticated)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                // Kiểm tra yêu cầu đặt lại mật khẩu
                if (requestPath.StartsWithSegments("/api/auth/reset-password"))
                {
                    var typeClaim = identity.FindFirst("type")?.Value;
                    if (typeClaim != "reset")
                    {
                        return AuthenticateResult.Fail("Invalid token for reset-password.");
                    }
                } else
                {
                    var typeClaim = identity.FindFirst("type")?.Value;
                    if (typeClaim == "reset")
                    {
                        return AuthenticateResult.Fail("Invalid token for reset-password.");
                    }
                }

                // You can further check user status or other conditions by querying your repository
                var userIdClaim = identity.FindFirst("userid")?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await _userRepositories.GetSingle(u => u.Id == userId);
                    if (user == null || user.Status.Equals(AccountStatusEnums.Inactive.ToString()))
                    {
                        return AuthenticateResult.Fail("User is inactive or not found.");
                    }
                }

                // Kiểm tra vai trò yêu cầu cho endpoint (nếu có)
                var endpointRoles = GetEndpointRoles();
                var userRoles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                if (endpointRoles.Any() && !userRoles.Any(ur => endpointRoles.Contains(ur)))
                {
                    return AuthenticateResult.Fail("User does not have the required role.");
                }

                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (SecurityTokenExpiredException ex)
            {
                // Token đã hết hạn
                return AuthenticateResult.Fail($"Token has expired: {ex.Message}");
            }
            catch (SecurityTokenException ex)
            {
                // Token không hợp lệ
                return AuthenticateResult.Fail($"Token validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Lỗi khác
                return AuthenticateResult.Fail($"An error occurred: {ex.Message}");
            }
        }
        private List<string> GetEndpointRoles()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint == null)
            {
                return new List<string>();
            }

            var authorizeAttributes = endpoint.Metadata.GetOrderedMetadata<AuthorizeAttribute>();
            var roles = new List<string>();

            foreach (var attribute in authorizeAttributes)
            {
                if (!string.IsNullOrEmpty(attribute.Roles))
                {
                    roles.AddRange(attribute.Roles.Split(',').Select(r => r.Trim()));
                }
            }

            return roles;
        }
    }
}
