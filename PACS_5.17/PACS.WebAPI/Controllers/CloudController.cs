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


namespace PACS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CloudController : ControllerBase
    {
        private readonly ICloudService cloudService;
        private readonly JwtSettings jwtSettings;

        public CloudController(ICloudService cloudService, JwtSettings jwtSettings)
        {
            this.cloudService = cloudService;
            this.jwtSettings = jwtSettings;
        }

        #region 公共文件夹相关API
        /// <summary>
        /// 获取所有的云端公开文件夹
        /// </summary>
        /// <returns></returns>
        [HttpGet("PublicFileFolder")]
        public async Task<IActionResult> GetAllPublicFolders()
        {
            // 解析token
            string authHeader = this.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
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

                ClaimsPrincipal a = jwtTokenHandler.ValidateToken(token, tokenValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validated);

                string userId = a.Claims.ElementAt(1).Value;

                var result = cloudService.GetPublicFolders();

                

                return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
            }


            return BadRequest(new ApiResponse("", false));
        }

        /// <summary>
        /// 获取特定公共文件夹下的文件列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("PublicFileFolder/{id}")]
        public async Task<IActionResult> GetPublicFiles(string id)
        {
            // 解析token
            string authHeader = this.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
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

                ClaimsPrincipal a = jwtTokenHandler.ValidateToken(token, tokenValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validated);

                string userId = a.Claims.ElementAt(1).Value;

                var result = cloudService.GetFileItems(id);

                if (result == null)
                {
                    return BadRequest(new ApiResponse("未授权", false));
                }

                List<FileItemDTO> files = new List<FileItemDTO>();

                files.AddRange(result);

                return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(files)));
            }


            return BadRequest(new ApiResponse("请登录", false));
        }

        /// <summary>
        /// 保存公共文件夹
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("PublicFileFolder/{id}")]
        public async Task<IActionResult> CopyPublicFileFolders(string id,FileFolderDTO dto)
        {
            // 解析token
            string authHeader = this.Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
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

                var a = jwtTokenHandler.ValidateToken(token, tokenValidationParams, out Microsoft.IdentityModel.Tokens.SecurityToken validated);

                // 获取token中的UserId
                string userId = a.Claims.ElementAt(1).Value;

                // 创建文件夹
                var result = await cloudService.CopyFolder(userId, id,dto.AccessModifier);

                return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
            }


            return BadRequest(new ApiResponse("", false));
        }
        #endregion

        #region 个人文件夹相关API
        /// <summary>
        /// 用户获取自己的云端文件夹列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("FileFolder")]
        public async Task<IActionResult> GetMyFolders()
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            string userId = claims.Claims.ElementAt(1).Value;

            var result = cloudService.GetFolders(userId);

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }

        /// <summary>
        /// 用户创建一个云端文件夹
        /// </summary>
        /// <param name="file">文件夹名</param>
        /// <returns></returns>
        [HttpPost("FileFolder")]
        public async Task<IActionResult> CreateFileFolder(FileFolderDTO folder)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if(claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }
               
            // 获取其中的UserId
            string userId = claims.Claims.ElementAt(1).Value;

            // 创建文件夹
            var result = await cloudService.CreateFolder(userId, folder.FolderName, folder.AccessModifier);

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }

        /// <summary>
        /// 逻辑删除文件夹
        /// </summary>
        /// <param name="id">待删除文件夹的Id</param>
        /// <returns></returns>
        [HttpDelete("FileFolder/{id}")]
        public async Task<IActionResult> DeleteFileFolder(string id)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            string userId = claims.Claims.ElementAt(1).Value;

            var result = await cloudService.DeleteFolder(userId, id);

            if (result)
            {
                return Ok(new ApiResponse("删除成功", true));
            }

            return Ok(new ApiResponse("删除失败", false));
        }

        /// <summary>
        /// 向指定文件夹上传文件
        /// </summary>
        /// <param name="FolderId">文件夹Id</param>
        /// <returns>文件</returns>
        [HttpPost("FileFolder/{id}")]
        public async Task<IActionResult> UploadFile(string id)
        {

            if (this.Request.Form.Files.Count > 0)
            {

                var fileList = new List<FileItemDTO>();

                foreach (var file in this.Request.Form.Files)
                {
                    var item = await cloudService.CreatFileItem(id, file);

                    FileItemDTO dto = new FileItemDTO(item);
                    fileList.Add(dto);

                }


                FileFolderModel fileModel = cloudService.GetFolder(id);


                return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(fileList)));
            }
            else
            {
                return BadRequest(new ApiResponse("", false));

            }
        }


        /// <summary>
        /// 将特定文件从文件夹中移除
        /// </summary>
        /// <param name="id">文件夹Id</param>
        /// <param name="fileId">文件Id</param>
        /// <returns></returns>
        [HttpDelete("FileFolder/{id}/FileItem")]
        public async Task<IActionResult> DeleteFile(string id,string fileId)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("", false));
            }

            string userId = claims.Claims.ElementAt(1).Value;

            var permission = await cloudService.IsAllowed(userId, id);

            if (!permission)
            {
                return Unauthorized(new ApiResponse("无权访问"));
            }

            var result = await cloudService.DeleteFileItem(id,fileId);

            if (result)
            {
                return Ok(new ApiResponse("删除成功", true));
            }
            else
            {
                return Ok(new ApiResponse("删除失败", false));
            }
        }

        /// <summary>
        /// 获取特定文件夹下的文件列表
        /// </summary>
        /// <param name="id">文件夹Id</param>
        /// <returns>文件概览FileItemDTO</returns>
        [HttpGet("FileFolder/{id}")]
        public async Task<IActionResult> GetFiles(string id)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
            {
                return BadRequest(new ApiResponse("请登录", false));
            }



            string userId = claims.Claims.ElementAt(1).Value;

            var result = cloudService.GetFileItems(id, userId);

            if (result == null)
            {
                return BadRequest(new ApiResponse("未授权", false));
            }

            List<FileItemDTO> files = new List<FileItemDTO>();

            files.AddRange(result);


            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(files)));
        }


        #endregion

        #region 文件相关API
        /// <summary>
        /// 下载特定文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FileItem/{id}")]
        public async Task<IActionResult> DownloadFile(string id)
        {
            var result = cloudService.GetFileItem(id);

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result.Image)));
        }

        

        /// <summary>
        /// 上传标注结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("FileItem/FileMask")]
        public async Task<IActionResult> UploadMask(FileMaskDTO dto)
        {
            var claims = GetCliams(this.Request.Headers["Authorization"]);
            if (claims == null)
                return BadRequest(new ApiResponse("", false));

            string userId = claims.Claims.ElementAt(1).Value;

            var result = await cloudService.CreatFileMask(userId,dto);

            return Ok(new ApiResponse(true,null));
        }


        #endregion




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
