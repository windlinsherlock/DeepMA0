using PACS.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public interface IAdminService
    {
        Task<ApiResponse> User();

        Task<ApiResponse> Role();

        Task<ApiResponse> Folder();

        Task<ApiResponse> Folder(string folderId);

        Task<ApiResponse> Mask(string FileItemId);

        Task<ApiResponse> SetItemMask(string FileItemId,string MaskId);
    }
}
