using Microsoft.AspNetCore.Identity;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly JwtSettings jwtSettings;

        private readonly IdentityContext identityContext;


        public AdminService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
            JwtSettings jwtSettings, IdentityContext identityContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.jwtSettings = jwtSettings;
            this.identityContext = identityContext;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public List<IdentityRole> AllRole()
        {
            return identityContext.Roles.ToList();
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public List<AppUser> AllUser()
        {
            return identityContext.Users.ToList();
        }

        public List<FileFolderModel> AllFileFolder()
        {
            return identityContext.FileFolders.ToList();
        }
        public List<FileItemDTO> AllFileItem(string FolderId)
        {
            var fileItems = identityContext.FolderFiles.Where(i => i.FileFolderId.Equals(FolderId)).Select(i => i.FileId).ToList();
            var files = identityContext.FileItems.Where(i => fileItems.Contains(i.FileItemId)).Select(i => new FileItemDTO(i,true)).ToList();
            return files;
        }

        public List<FileMaskModel> AllFileMask(string FileItemId)
        {
            var fileMask = identityContext.FileMasks.Where(i => i.FileItemId.Equals(FileItemId)).ToList();
            return fileMask;
        }

        public async Task<bool> SetItemMask(string FileItemId, string MaskId)
        {
            try
            {
                var item = identityContext.FileItems.Find(FileItemId);
                item.MaskId = MaskId;
                await identityContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            
        }


        /// <summary>
        /// 为用户添加特定角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<bool> AddToRole(string userId,string role)
        {
            var user = identityContext.Users.Single(i => i.Id.Equals(userId));
            await userManager.AddToRoleAsync(user, role);
            identityContext.SaveChanges();
            return true;
        }

        public async Task<string> GeneratePasswordResetToken(string userId)
        {
            var user = identityContext.Users.Single(i => i.Id.Equals(userId));
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
