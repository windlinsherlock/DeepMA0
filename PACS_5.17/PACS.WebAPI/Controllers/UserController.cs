using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Data;
using PACS.WebAPI.Services;
using PACS.Shared.DTOs;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;

namespace PACS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEmailSender emailSender;

        public UserController(IUserService userService, IEmailSender emailSender)
        {
            this.userService = userService;
            this.emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginDTO user)
        {
            var result = await userService.RegisterAsync(user.UserName, user.Password);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse(false, result.Errors));
            }

            var address = this.HttpContext.Request.Host;

            var callbackUrl = "http://" + address + "/api/User/ConfirmEmail?" + "userId=" + result.Id + "&code=" + result.Code;

            await emailSender.SendEmailAsync(user.UserName, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok(new ApiResponse("请前往邮箱进行认证",true));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO user)
        {
            Shared.Contact.TokenResult result = await userService.LoginAsync(user.UserName, user.Password);
            Console.WriteLine(this.Request);

            if (!result.Success)
            {
                string err = "";
                foreach(var error in result.Errors)
                {
                    err += error;
                }
                return Ok(new ApiResponse(err,false,null));
            }

            string userId = result.UserId;
            result.UserId = null;

            return Ok(new ApiResponse(userId + "/" + result.IsAdmin,true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest();

            var result = await userService.ConfirmEmailAsync(userId, code);

            if(result == null)
                return BadRequest();

            string StatusMessage = result.Succeeded ? "邮箱认证成功" : "认证内容错误";
            return Ok(StatusMessage);
        }

    }

}
