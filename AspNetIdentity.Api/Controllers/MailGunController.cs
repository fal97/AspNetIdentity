using AspNetIdentity.Api.Dtos;
using AspNetIdentity.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailGunController : ControllerBase
    {
        private IMailGunService mailGunService;
        private IConfiguration configuration;

        public MailGunController(IMailGunService mailGunService, IConfiguration configuration)
        {
            this.mailGunService = mailGunService;
            this.configuration = configuration;
        }


        [HttpPost("SendMail")]
        public async Task<IActionResult> SendEmai(UserEmailOptions userEmailOptions)
        {

            var result = await mailGunService.SendMailAsync(userEmailOptions);

            if (result.IsSuccessful)
            {
                return Ok(result);
            }

            return BadRequest();

        }

        [HttpGet]
        public async Task<IActionResult> GetEmailStats(string senderEmail)
        {
            var result = await mailGunService.GetMailStats(senderEmail);

            return Ok(result);

        }



    }
}
