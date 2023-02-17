using AspNetIdentity.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetIdentity.Api.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);

        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);

        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserManagerResponse> ForgetPasswordAsync(string email);
    }

    public class UserService : IUserService
    {
        private UserManager<IdentityUser> _userManager;
        private IConfiguration _configuration;
        private IMailService _mailService;
        public UserService(UserManager<IdentityUser> userManager,IConfiguration configuration, IMailService mailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
        }
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register model is null");
            }

            if(model.Password != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };
            }

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result  = await _userManager.CreateAsync(identityUser,model.Password);

            if (result.Succeeded)
            {
                //generating a token for confirm email
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                //transform into string with normal values
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationToken);
                //valid token with plain string without no special charactors
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                //setting the url for user to comeback
                string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userId={identityUser.Id}&token={validEmailToken}";

                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<h1>Welcome to auth demo</h1>" 
                    + $"<p>please confirm your email by <a href='{url}'>Clicking here</a></p>"); 

                return new UserManagerResponse
                {
                    Message = "User Created Succesfully!",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User did not create succesfully!",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }


        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is no user with that email",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManagerResponse
                {
                    Message = "Invalid Password",
                    IsSuccess = false,
                };
            }

            var claim = new[]
            {
                 new Claim("Email",model.Email),
                 new Claim(ClaimTypes.NameIdentifier,user.Id),
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claim,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256

                ));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new UserManagerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate= token.ValidTo
            };


        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new UserManagerResponse { IsSuccess = false,Message= "User not found" };

            }

            var decodedToken = WebEncoders.Base64UrlDecode(token);

            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if(result.Succeeded)
            {
                return new UserManagerResponse
                {
                    Message = "Email confirmed succesfully!",
                    IsSuccess = true
                };


            }
            throw new NotImplementedException();
        }

        public  async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = " No User allocated with the email!"
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var encodedEmailToken = Encoding.UTF8.GetBytes(token);
            //valid token with plain string without no special charactors
            var validToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await _mailService.SendEmailAsync(email, "Reset Password",
                "Follow the instructions to reset your password</h1>" +
                $"<p>to reset your password <a href='{url}'>Click here</a></p>");



            throw new NotImplementedException();
        }
    }
}
