using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using PACS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PACS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoDiagnoseController : ControllerBase
    {
        private readonly IAutoDiagnoseService autoDiagnoseService;
        private readonly JwtSettings jwtSettings;

        public AutoDiagnoseController(IAutoDiagnoseService autoDiagnoseService, JwtSettings jwtSettings)
        {
            this.autoDiagnoseService = autoDiagnoseService;
            this.jwtSettings = jwtSettings;
        }

        /// <summary>
        /// 获取指定id的自动诊断结果
        /// </summary>
        /// <param name="autoDiagnoseItemId"></param>
        /// <returns>文件</returns>
        [HttpGet("AutoDiagnoseItem/{autoDiagnoseItemId}")]
        public async Task<IActionResult> GetAutoDiagnoseItem(string autoDiagnoseItemId)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            // 获取其中的UserId
            string userId = claims.Claims.ElementAt(1).Value;

            var item = await autoDiagnoseService.GetAutoDiagnoseItem(userId, autoDiagnoseItemId);
            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(item)));


        }

        /// <summary>
        /// 向指定文件夹上传文件
        /// </summary>
        /// <param name="FolderId">文件夹Id</param>
        /// <returns>文件</returns>
        [HttpPost("AutoDiagnoseItem")]
        public async Task<IActionResult> CreateAutoDiagnoseItem(AutoDiagnoseItemDTO autoDiagnoseItemDTO)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            // 获取其中的UserId
            string userId = claims.Claims.ElementAt(1).Value;
            bool item = await autoDiagnoseService.CreateAutoDiagnoseItem(userId, autoDiagnoseItemDTO);
            if (item)
            {
                return Ok(new ApiResponse("保存成功", true));
            }
            else
            {
                return BadRequest(new ApiResponse("文件重名", false));
            }
        }

        /// <summary>
        /// 获取用户的所有自动诊断文件夹
        /// </summary>
        /// <param name="autoDiagnoseItemId"></param>
        /// <returns>文件</returns>
        [HttpGet("AutoDiagnoseFolders")]
        public async Task<IActionResult> GetAutoDiagnoseFolders()
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            // 获取其中的UserId
            string userId = claims.Claims.ElementAt(1).Value;

            var item = autoDiagnoseService.GetAutoDiagnoseFolders(userId);
            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(item)));
            


        }

        ///// <summary>
        ///// 向指定文件夹上传文件
        ///// </summary>
        ///// <param name="FolderId">文件夹Id</param>
        ///// <returns>文件</returns>
        //[HttpPost("AutoDiagnoseItem")]
        //public async Task<IActionResult> CreateAutoDiagnoseItem(string fileName)
        //{
        //    var claims = GetCliams(this.Request.Headers["Authorization"]);
        //    if (claims == null)
        //    {
        //        return BadRequest(new ApiResponse("", false));
        //    }

        //    // 获取其中的UserId
        //    string userId = claims.Claims.ElementAt(1).Value;
        //    IFormFile[] formFiles = new IFormFile[3];

        //    if (this.Request.Form.Files.Count > 0)
        //    {

        //        foreach (var file in this.Request.Form.Files)
        //        {
        //            string name = file.FileName;
        //            switch (name)
        //            {
        //                case "image":
        //                    formFiles[0] = file;
        //                    break;
        //                case "thermodynamicChart":
        //                    formFiles[1] = file;
        //                    break;
        //                case "labelImage":
        //                    formFiles[2] = file;
        //                    break;
        //            }
        //        }
        //        bool item = await autoDiagnoseService.CreateAutoDiagnoseItem(userId, fileName, formFiles);
        //        if (item)
        //        {
        //            return Ok(new ApiResponse("保存成功", true));
        //        }
        //        else
        //        {
        //            return BadRequest(new ApiResponse("文件重名", false));
        //        }

        //    }
        //    else
        //    {
        //        return BadRequest(new ApiResponse("上传失败", false));

        //    }
        //}

        /// <summary>
        /// 从token中获取用户的claim
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        private ClaimsPrincipal GetCliams(String authHeader)
        {
            if (authHeader == null || !authHeader.StartsWith("Bearer"))
                return null;

            string token = authHeader.Substring("Bearer ".Length).Trim();
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParams = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),
            };

            return jwtTokenHandler.ValidateToken(token, tokenValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validated);
        }
    }
}
