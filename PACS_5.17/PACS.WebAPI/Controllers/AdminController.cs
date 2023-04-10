using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PACS.Shared.DTOs;
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
    /*[Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]*/
    public class AdminController : ControllerBase
    {
        private readonly IAdminService adminService;
        private readonly JwtSettings jwtSettings;

        public AdminController(IAdminService adminService, JwtSettings jwtSettings)
        {
            this.adminService = adminService;
            this.jwtSettings = jwtSettings;
        }

        #region 用户相关API
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("User")]
        public async Task<IActionResult> User()
        {
            var result = adminService.AllUser();
            var users = new List<UserDTO>();

            foreach (var user in result)
            {
                users.Add(new UserDTO { Id = user.Id, Name = user.UserName, Email = user.Email, EmailConfirmed = user.EmailConfirmed, LockoutEnabled = user.LockoutEnabled, AccessFailedCount = user.AccessFailedCount });
            }

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(users)));
        }
        #endregion


        #region 角色相关API
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        [HttpGet("Role")]
        public async Task<IActionResult> Role()
        {
            var result = adminService.AllRole();
            var roles = new List<RoleDTO>();

            foreach (var role in result)
            {
                roles.Add(new RoleDTO { RoleName = role.Name });
            }

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(roles)));
        }
        #endregion

        #region 文件相关API

        /// <summary>
        /// 获取所有文件夹
        /// </summary>
        /// <returns></returns>
        [HttpGet("FileFolder")]
        public async Task<IActionResult> Folder()
        {
            var result = adminService.AllFileFolder();

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }

        [HttpGet("FileFolder/{id}")]
        public async Task<IActionResult> Folder(string id)
        {
            var result = adminService.AllFileItem(id);

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }


        [HttpGet("FileItem/{id}/FileMask")]
        public async Task<IActionResult> FileMask(string id)
        {
            var result = adminService.AllFileMask(id);

            return Ok(new ApiResponse(true, Newtonsoft.Json.JsonConvert.SerializeObject(result)));
        }

        [HttpPut("FileItem/{id}")]
        public async Task<IActionResult> SetItemMask(string id,FileMaskDTO dto)
        {
            bool result = await adminService.SetItemMask(id,dto.FileMaskId);

            return Ok(new ApiResponse("",result));
        }

        #endregion
    }
}
