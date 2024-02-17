using JWTAuthServer.Common;
using JWTAuthServer.Common.Constants;
using JWTAuthServer.Common.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Web;

namespace JWTAuthServer.Services.Services
{
    public class UserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public UserManagementService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        //create get by username method
        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        //create get by email method
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        //create user method
        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        //create Email confirmation method
        public async Task<bool> IsEmailConfirmed(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        //create check password method
        public async Task<bool> CheckPassword(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        //Generate Password Reset URL
        public async Task<dynamic> GeneratePasswordResetURL(ForgotPasswordViewModel model)
        {
            dynamic requestObject = new System.Dynamic.ExpandoObject();

            var user = await GetUserByEmail(model.Email);
            if (user == null)
            {
                requestObject.IsSuccessStatus = false;
                requestObject.StatusCode = (int)StatusCodes.Status404NotFound;
                requestObject.ErrorMessage = "User Not Found";
                return requestObject;
            }
            if (!(await IsEmailConfirmed(user)))
            {
                requestObject.IsSuccessStatus = false;
                requestObject.StatusCode = 400;
                requestObject.ErrorMessage = "Email Not Confirmed";
                return requestObject;
            }
            bool isLocalUser = await _userManager.HasPasswordAsync(user);
            if (!isLocalUser)
            {
                requestObject.IsSuccessStatus = false;
                requestObject.StatusCode = 400;
                requestObject.ErrorMessage = "No account found with that email address. Please contact your Administrator";
                return requestObject;
            }
            //generate reset token
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = HttpUtility.UrlEncode(code);

            //generate callback url
            string callbackUrl = model.ResetPasswordUrl + "?userId=" + user.Id + "&code=" + code;
            if (!String.IsNullOrEmpty(model.Source))
            {
                callbackUrl += "&dest=" + model.Source;
            }

            
            requestObject.UserId = user.Id;
            requestObject.ResetPasswordLink = callbackUrl;
            requestObject.IsSuccessStatus = true;
            requestObject.StatusCode = 200;
            requestObject.ErrorMessage = "";
        
            return requestObject;
        }

        //create change password method
        public async Task<IdentityResult> ChangeUserPassword(ChangePasswordViewModel model)
        {
            var user = await GetUserById(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }
            
            var result = await ChangePasswordAsync(user,model.NewPassword);
            return result;
        }
        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string newPassword)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result;
        }

        //create a method to change user.status to activate
        public async Task<IdentityResult> Activate(string userId)
        {
            var user = await GetUserById(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }

            user.Status = AuthServerConstants.getInstance().Active;
            return await _userManager.UpdateAsync(user);
        }

        //create a method to change user.status to deactivate
        public async Task<IdentityResult> Deactivate(string userId)
        {
            var user = await GetUserById(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }

            user.Status = AuthServerConstants.getInstance().InActive;
            return await _userManager.UpdateAsync(user);
        }

        //create changeEmail method
        public async Task<IdentityResult> ChangeEmailAndUsername(ChangeUserEmailModel model)
        {
            var user = await GetUserById(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }


            user.Email = model.Email;
            user.UserName = model.UserName;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }
        //create method to ChangePhoneNumber
        public async Task<IdentityResult> ChangePhoneNumber(ChangeUserPhoneNumberModel model)
        {
            var user = await GetUserById(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }

            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        //create method ChangePhoneNumberOrEmail
        public async Task<IdentityResult> ChangePhoneNumberOrEmail(ChangeUserPhoneOrEmailModel model)
        {
            var user = await GetUserById(model.Id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }

            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;

            //we will change the email and phone number along with the new password which is required for this feature
            await ChangePasswordAsync(user, model.NewPassword);

            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        //create a methdod to update user agreement flag
        public async Task<IdentityResult> UpdateUserAgreementFlag(string userId, bool flag)
        {
            var user = await GetUserById(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User Not Found" });
            }

            user.Agreement_Flag = flag;

            var result = await _userManager.UpdateAsync(user);
            return result;
        }


    }
}
