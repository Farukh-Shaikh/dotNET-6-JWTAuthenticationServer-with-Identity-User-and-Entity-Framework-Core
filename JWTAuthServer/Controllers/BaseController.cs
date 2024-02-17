using JWTAuthServer.Common.Models;
using JWTAuthServer.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWTAuthServer.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly UserManagementService _userService;

        //claims
        protected string UserId => User.FindFirstValue("userId");
        protected string UserName => User.FindFirstValue(ClaimTypes.Name);
        protected string Role => User.FindFirstValue("role");
        protected string TenantId => User.FindFirstValue("tenantId");

        public BaseController(UserManagementService userService)
        {
            _userService = userService;

        }
        // In case of error this function is used to return formatted response.
        protected ActionResult GetErrorResult(IdentityResult result)
        {

            try
            {
                ResponseViewModel<Object> response = new ResponseViewModel<Object>();
                if (result == null)
                {
                    response.status.code = 500;
                    response.status.message = "Internal Server Error!";
                    return Ok(response);
                }

                if (!result.Succeeded)
                {
                    if (result.Errors != null)
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        // No ModelState errors are available to send, so just return an empty BadRequest.

                        response.status.code = 400;
                        response.status.message = "Bad Request(But Model State is valid)";
                        return Ok(response);
                    }

                    //return BadRequest(ModelState);
                    response.status.code = 400;
                    response.status.message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                    return Ok(response);
                }

                response.status.code = 500;
                response.status.message = "Internal Server Error!";
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
