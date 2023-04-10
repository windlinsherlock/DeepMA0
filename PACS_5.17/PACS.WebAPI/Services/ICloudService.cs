using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public interface ICloudService
    {
        public Task<bool> IsAllowed(string UserId,string FolderId);

        public List<FileFolderModel> GetPublicFolders();

        public List<FileFolderModel> GetFolders(string UserId);

        public FileFolderModel GetFolder(string FolderId);

        public Task<FileFolderModel> CreateFolder(string Id,string Name,string AccessModifier);

        public Task<FileFolderModel> CopyFolder(string Id, string FolderId,string AccessModifier);

        public Task<bool> DeleteFolder(string Id,string FileFolderId);

        public Task<bool> DeleteFileItem(string FileFolderId, string FileId);

        public Task<FileItemModel> CreatFileItem(string Id, Microsoft.AspNetCore.Http.IFormFile file);

        public FileItemModel GetFileItem(string Id);

        public List<FileItemDTO> GetFileItems(string FolderId);
        public List<FileItemDTO> GetFileItems(string FolderId, string userId);

        public Task<FileMaskModel> CreatFileMask(string userId, FileMaskDTO dto);
    }
}
