using PACS.Shared.Contact;
using PACS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public interface ICloudService
    {
        public Task<ApiResponse> GetPublicFolders();
        public Task<ApiResponse> GetFolders();
        public Task<ApiResponse> CreateFolder(string Name,string AccessModifier);

        public Task<ApiResponse> DeleteFolder(string FolderId);

        public Task<ApiResponse> CopyFolder(string FolderId,bool re);
        public Task<ApiResponse> UploadFile(List<string> path, string FolderId);
        public Task<ApiResponse> GetFileItem(string FileItemId);
        public Task<ApiResponse> GetFiles(string FolderId);
        public Task<ApiResponse> GetPublicFiles(string FolderId);

        public Task<ApiResponse> UploadMask(FileMaskDTO dto);
    }
}
