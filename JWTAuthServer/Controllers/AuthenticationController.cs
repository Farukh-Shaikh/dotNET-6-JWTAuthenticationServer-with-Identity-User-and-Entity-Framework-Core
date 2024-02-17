using JWTAuthServer.Common.Constants;
using JWTAuthServer.Common.Models;
using JWTAuthServer.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthServer.Controllers
{
    public class AuthenticationController : BaseController
    {

        private readonly LoginService _loginService;

        //create constructor
        public AuthenticationController(LoginService loginService, UserManagementService userService) : base(userService)
        {
            _loginService = loginService;
        }

        //create login api
        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] ApplicationUser loginModel)
        {

            ApplicationUser user = await _userService.GetUserByUserName(loginModel.UserName);
            if(user != null && await _userService.CheckPassword(user, loginModel.Password))
            {
                // Generate a new access token
                var jwtToken = _loginService.GenerateToken(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.WriteToken(jwtToken);

                var refreshToken = tokenHandler.WriteToken(_loginService.GenerateRefreshToken(user));

                //return token
                return Ok(new TokenRequestModel
                {
                    access_token = securityToken,
                    expires = jwtToken.ValidTo,
                    Id = user.Id,
                    refresh_token = refreshToken
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokenRequestModel refreshTokenRequest)
        {
            ApplicationUser user = new ApplicationUser();
            if (!String.IsNullOrEmpty(refreshTokenRequest.Id))
            {
                // Get the user by the id
                user = await _userService.GetUserById(refreshTokenRequest.Id);

            }
            
            if(user == null)
                return BadRequest("Invalid User Id");

            if(user.Status == AuthServerConstants.getInstance().InActive)
                return BadRequest("User is inactive");

            // Validate the refresh token (you need to implement this logic)
            if (_loginService.ValidateRefreshToken(refreshTokenRequest.refresh_token, user, _userService))
            {

                // Generate a new access token
                var jwtToken = _loginService.GenerateToken(user);

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.WriteToken(jwtToken);


                //return token
                return Ok(new TokenRequestModel
                {
                    access_token = securityToken,
                    expires = jwtToken.ValidTo,
                    Id = user.Id,
                    refresh_token = refreshTokenRequest.refresh_token
                }) ;
            }

            // Invalid refresh token
            return BadRequest("Invalid refresh token");
        }



    }
}
