using JWTAuthServer.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthServer.Services.Services
{
    public class LoginService
    {
        private readonly IConfiguration _configuration;


        //create constructor
        public LoginService( IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static List<Claim> GenerateClaim(ApplicationUser user)
        {
            //Add claims according to requirement
            var identity = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("userName", user.UserName),
                new Claim("userId", user.Id),
                //Roles can also be configured here but we are considering only one role which is Admin
                new Claim("role", "Admin"),
                new Claim("tenantId", user.Tenant_ID.ToString())
            };
            return identity;
        }
        public JwtSecurityToken GenerateToken(ApplicationUser user)
        {
            //get claims
            var authClaims = GenerateClaim(user);

            int tokenExpirationMinutes;

            //get tokenExpirationMinutes from configuration. If not found then set it to 30 minutes
            if (!int.TryParse(_configuration["JWT:AccessTokenExpirationMinutes"], out tokenExpirationMinutes))
                tokenExpirationMinutes = 30;

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return token;
        }

        public JwtSecurityToken GenerateRefreshToken(ApplicationUser user)
        {
            int refreshTokenExpirationInDays;

            //get tokenExpirationMinutes from configuration. If not found then set it to 30 minutes
            if (!int.TryParse(_configuration["JWT:RefreshTokenExpirationDays"], out refreshTokenExpirationInDays))
                refreshTokenExpirationInDays = 30;

            //generate claims

            var refreshTokenclaim = new List<Claim>
            {
                new Claim("userId", user.Id)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:RefreshTokenSecret"]));
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddDays(refreshTokenExpirationInDays),
                    claims: refreshTokenclaim,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return token;
        }

        public bool ValidateRefreshToken(string refreshToken, ApplicationUser user, UserManagementService _userService)
        {
            // Retrieve the refresh token secret key used during token generation
            var refreshSecret = _configuration["JWT:RefreshTokenSecret"];

            // Validate the refresh token using the same key used during token generation
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecret)),
                ValidateIssuer = true, 
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ClockSkew = TimeSpan.FromMinutes(5) // To add flexibility of 5 min even if token is expired
            };

            try
            {
                // Validate the refresh token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);

                //Check if the refresh token is associated with a valid user
                var userIdClaim = principal.FindFirst("userId");
                if (userIdClaim == null)
                {
                    // The refresh token is missing the user ID claim
                    return false;
                }
                
                if(user.Id != userIdClaim.Value)
                {
                    //refresh token is not of the same user 
                    return false;
                }

                //refresh token is valid
                return true;
            }
            catch (SecurityTokenException)
            {
                // Token validation failed
                return false;
            }
        }

    }
}
