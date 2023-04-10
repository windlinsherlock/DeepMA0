using Microsoft.AspNetCore.Identity;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using PACS.WebAPI.Areas.Identity.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public interface IAdminService
    {
        public List<IdentityRole> AllRole();

        public List<AppUser> AllUser();

        public List<FileFolderModel> AllFileFolder();

        public List<FileItemDTO> AllFileItem(string FolderId);

        public List<FileMaskModel> AllFileMask(string FileItemId);

        public Task<bool> SetItemMask(string FileItemId, string MaskId);

        public Task<bool> AddToRole(string userId, string role);
    }
}
