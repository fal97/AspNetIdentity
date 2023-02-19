using AspNetIdentity.Api.Models;
using AspNetIdentity.Api.Services;
using AspNetIdentity.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        private IConfiguration _configuration;

        public AuthController(IUserService userService, IMailService mailService,IConfiguration configuration)
        {
            _userService = userService;
            _configuration= configuration;
            _mailService = mailService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);

                if(result.IsSuccess)
                {
                    await _mailService.SendEmailAsync(model.Email,"New Login","<h1>hey! new login to your account noticed</h1><p>new login to your account at "+ DateTime.Now + " </p>");
                    return Ok(result);
                }
                return BadRequest(result);

            }

            return BadRequest("Some properties not valid");//status code 400
        }

        //api/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess) return Ok(result);

                return BadRequest(result);


            }

            return BadRequest("Some properties are not valid");
        }

        //api/auth/confirmemail?userid&token
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if(string.IsNullOrWhiteSpace(userId)|| string.IsNullOrWhiteSpace(token))
            {
                return NotFound();
            }

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if(result.IsSuccess) {
                return Redirect($"{_configuration["AppUrl"]}/confirmEmail.html");
            }

            return BadRequest(); 

        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest();
        }

    }
}
