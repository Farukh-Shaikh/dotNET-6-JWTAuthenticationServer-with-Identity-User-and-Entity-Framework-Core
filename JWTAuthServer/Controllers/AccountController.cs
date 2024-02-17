using JWTAuthServer.Common.Models;
using JWTAuthServer.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using System.Security.Claims;
using System.Web;

namespace JWTAuthServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        public AccountController(UserManagementService userService) : base(userService)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] ApplicationUser user, string clientId, bool sendEmail = false)
        {
            //INFO
            //client Id: can be used to identify the client in case uf different sources are going to use the same API.
            //sendEmail: can be used to check if email should be sent to user. SMTP server email can be configured in this project

            ResponseViewModel<String> response = new ResponseViewModel<String>();
            var userExist = await _userService.GetUserByEmail(user.Email);
            if (userExist != null)
            {

                response.status.code = (int)StatusCodes.Status403Forbidden;
                response.status.message = "User Already Exists";

                return BadRequest(response);
            }

            IdentityResult result = await _userService.CreateUser(user, user.Password);
            if (!result.Succeeded)
            {   
                return GetErrorResult(result);
            }
            else
            {
                response.status.code = (int)System.Net.HttpStatusCode.Created;
                response.status.message = "User Created Successfully";
                response.data = user.Id;

                return Ok(response);
            }


        }

        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();


                dynamic requestObject = await _userService.GeneratePasswordResetURL(model);
                response.status.code = requestObject.StatusCode;
                response.status.message = requestObject.ErrorMessage;
                response.data = requestObject;
                return Ok(response);


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //create changeUserPassword Method
        [HttpPost]
        [Route("ChangeUserPassword")]
        public async Task<ActionResult> ChangeUserPassword(ChangePasswordViewModel model)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
  
                var result = await _userService.ChangeUserPassword(model);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "User password changed Successfully.";
                    return Ok(response);
                }
                else
                {
                    return GetErrorResult(result);
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create a method named activate to change user status to active
        [HttpPut]
        [Route("Activate/{userId}")]
        public async Task<ActionResult> Activate(string userId)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                
                var result = await _userService.Activate(userId);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "Successfully Activated";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create a method named deactivate to change user status to inactive
        [HttpPut]
        [Route("Deactivate/{userId}")]
        public async Task<ActionResult> Deactivate(string userId)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                var result = await _userService.Deactivate(userId);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "Successfully Deactivated";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create a method to change user email
        [HttpPost]
        [Route("ChangeUserEmail")]
        public async Task<ActionResult> ChangeUserEmail(ChangeUserEmailModel model)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                var result = await _userService.ChangeEmailAndUsername(model);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "User updated Successfully";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create a method ChangeUserPhoneNumber
        [HttpPost]
        [Route("ChangeUserPhoneNumber")]
        public async Task<ActionResult> ChangeUserPhoneNumber(ChangeUserPhoneNumberModel model)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                var result = await _userService.ChangePhoneNumber(model);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "User updated Successfully";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create method ChangeUserPhoneOrEmail
        [HttpPost]
        [Route("ChangeUserPhoneOrEmail")]
        public async Task<ActionResult> ChangeUserPhoneOrEmail(ChangeUserPhoneOrEmailModel model)
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                var result = await _userService.ChangePhoneNumberOrEmail(model);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "User updated Successfully";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //create put method AcceptLicenceAgreement to make user Agreement_Flag true
        [HttpPut]
        [Route("AcceptLicenceAgreement")]
        public async Task<ActionResult> AcceptLicenceAgreement()
        {
            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();

                string loggedInUserId = UserId;

                if(loggedInUserId== null)
                {
                    response.status.code = 400;
                    response.status.message = "Could not get user";
                    return Ok(response);
                }

                var result = await _userService.UpdateUserAgreementFlag(loggedInUserId,true);
                if (result.Succeeded)
                {
                    response.status.code = 200;
                    response.status.message = "User Aggrement Flag updated Successfully";
                    return Ok(response);
                }

                return GetErrorResult(result);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        

    }


}
