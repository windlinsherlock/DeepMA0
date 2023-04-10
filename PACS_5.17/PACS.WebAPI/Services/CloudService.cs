using Microsoft.EntityFrameworkCore;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;

using PACS.WebAPI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace PACS.WebAPI.Services
{
    public class CloudService : ICloudService
    {
        private readonly IdentityContext identityContext;

        public CloudService(IdentityContext identityContext)
        {
            this.identityContext = identityContext;
        }

        public async Task<bool> IsAllowed(string UserId, string FolderId)
        {
            try
            {
                var folder = await identityContext.FileFolders.FirstAsync(i => i.FileFolderId.Equals(FolderId));
                if (folder.CreatedBy.Equals(UserId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        /// <summary>
        /// 获取所有公开文件夹
        /// </summary>
        /// <returns></returns>
        public List<FileFolderModel> GetPublicFolders()
        {
            var result = identityContext.FileFolders.Where(i => i.AccessModifier.Equals("public")).ToList();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// 获取用户的所有文件夹
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <returns>用户的所有文件夹</returns>
        public List<FileFolderModel> GetFolders(string UserId)
        {
            var result = identityContext.UserFileFolders.Where(i => i.UserId.Equals(UserId));

            if (result == null)
            {
                return null;
            }

            List<FileFolderModel> model = new List<FileFolderModel>();

            List<string> folders = result.Select(i => i.FileFolderId).ToList();

            model.AddRange(identityContext.FileFolders.Where(i => folders.Contains(i.FileFolderId)).ToList());

            return model;
        }

        /// <summary>
        /// 根据文件夹Id查询文件夹
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public FileFolderModel GetFolder(string FolderId)
        {
            FileFolderModel model = identityContext.FileFolders.Single(i => i.FileFolderId == FolderId);

            return model;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Id">用户Id</param>
        /// <param name="Name">文件夹名</param>
        /// <param name="AccessModifier">文件夹访问修饰符</param>
        /// <returns>文件夹</returns>
        public async Task<FileFolderModel> CreateFolder(string Id, string Name,string AccessModifier)
        {

            var item = new FileFolderModel(Name);
            item.CreatedBy = Id;
            item.AccessModifier = AccessModifier;


            var result = await identityContext.FileFolders.AddAsync(item);
            identityContext.UserFileFolders.Add(new Commons.Areas.Identity.Data.UserFileFolders { FileFolderId = result.Entity.FileFolderId, UserId = Id });

            await identityContext.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// 拷贝公共文件夹
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public async Task<FileFolderModel> CopyFolder(string Id, string folderId, string AccessModifier)
        {
            if (AccessModifier.Equals("public"))
            {
                var origin = identityContext.FileFolders.Single(i => i.FileFolderId.Equals(folderId));
                

                identityContext.UserFileFolders.Add(new Commons.Areas.Identity.Data.UserFileFolders { UserId = Id, FileFolderId = origin.FileFolderId });

                await identityContext.SaveChangesAsync();
                
                return origin;
            }

            else
            {
                var origin = identityContext.FileFolders.Single(i => i.FileFolderId.Equals(folderId));
                var relation = identityContext.FolderFiles.Where(i => i.FileFolderId.Equals(folderId)).Select(i => i.FileId).ToList();

                var folder = new FileFolderModel(origin.Name);
                folder.CreatedBy = Id;
                folder.AccessModifier = "private";


                var result = await identityContext.FileFolders.AddAsync(folder);
                identityContext.UserFileFolders.Add(new Commons.Areas.Identity.Data.UserFileFolders { FileFolderId = result.Entity.FileFolderId, UserId = Id });

                List<FolderFiles> files = new List<FolderFiles>();

                foreach (var item in relation)
                {
                    files.Add(new FolderFiles { FileFolderId = folder.FileFolderId, FileId = item });
                }

                identityContext.FolderFiles.AddRange(files);


                await identityContext.SaveChangesAsync();
                return folder;
            }
        }



        /// <summary>
        /// 逻辑删除文件夹
        /// </summary>
        /// <param name="Id">用户Id</param>
        /// <param name="FileFolderId">文件夹Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteFolder(string Id, string FileFolderId)
        {
            try
            {
                var item = await identityContext.UserFileFolders.FirstAsync(i => i.FileFolderId.Equals(FileFolderId) && i.UserId.Equals(Id));
                identityContext.UserFileFolders.Remove(item);

                var folder = this.identityContext.FileFolders.Find(FileFolderId);
                if (folder != null)
                {
                    this.identityContext.FileFolders.Remove(folder);
                }
                var re = this.identityContext.FolderFiles.Where((i) => i.FileFolderId.Equals(FileFolderId)).ToList();
                this.identityContext.FolderFiles.RemoveRange(re);

                await identityContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="FileFolderId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FileItemModel> CreatFileItem(string FileFolderId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            // 存储图片
            var fileStream = file.OpenReadStream();
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            fileStream.CopyTo(stream);


            var item = new FileItemModel(file.FileName, stream.ToArray());

            try
            {
                var folder = identityContext.FileFolders.Single(i => i.FileFolderId.Equals(FileFolderId));
            }
            catch (Exception ex) // 所请求的文件夹不存在
            {
                return null;
            }

            try
            {
                var result = identityContext.FileItems.Single(i => i.Md5 == item.Md5);
                // 该文件已存在
                if (result != null)
                {
                    try // 则直接在该文件夹加入
                    {
                        identityContext.FolderFiles.Add(new FolderFiles { FileId = result.FileItemId, FileFolderId = FileFolderId });
                    }
                    catch // 文件夹中已存在该文件
                    {
                        await identityContext.SaveChangesAsync();
                        return item;
                    }
                }
            }
            catch (Exception ex) // 该文件不存在
            {
                var entity = identityContext.FileItems.Add(item).Entity;
                item.FileItemId = entity.FileItemId;
                identityContext.FolderFiles.Add(new FolderFiles { FileId = entity.FileItemId, FileFolderId = FileFolderId });
            }


            fileStream.Dispose();
            stream.Dispose();

            await identityContext.SaveChangesAsync();
            return item;
        }

        /// <summary>
        /// 删除文件夹中的文件
        /// </summary>
        /// <param name="FileFolderId">文件夹Id</param>
        /// <param name="FileId">文件Id</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileItem(string FileFolderId, string FileId)
        {
            try
            {
                var item = identityContext.FolderFiles.First(i => i.FileFolderId.Equals(FileFolderId) && i.FileId.Equals(FileId));
                identityContext.FolderFiles.Remove(item);
                await identityContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据图片Id获取图片
        /// </summary>
        /// <param name="FileItemId">图片Id</param>
        /// <returns>图片</returns>
        public FileItemModel GetFileItem(string FileItemId)
        {
            var item = identityContext.FileItems.AsNoTracking().Single(i => i.FileItemId.Equals(FileItemId));
            return item;
        }

        /*public byte[] GetFileItem(string FileItemId,bool i)
        {
            //var item = identityContext.FileItems.AsNoTracking().Single(i => i.FileItemId.Equals(FileItemId));

            var item = Shared.Commons.Util.LoadImage(FileItemId);

            return item;
        }*/

        /// <summary>
        /// 获取公共文件夹下的所有文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<FileItemDTO> GetFileItems(string FolderId)
        {
            var fileItems = identityContext.FolderFiles.Where(i => i.FileFolderId.Equals(FolderId)).Select(i => i.FileId).ToList();
            var files = identityContext.FileItems.Where(i => fileItems.Contains(i.FileItemId)).Select(i => new FileItemDTO(i)).ToList();
            return files;
        }

        /// <summary>
        /// 获取用户权限内的文件夹下的所有文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<FileItemDTO> GetFileItems(string FolderId, string userId)
        {
            // 验证用户是否有权限访问该文件夹
            try
            {
                var exsit = identityContext.UserFileFolders.Where(i => i.FileFolderId == FolderId && i.UserId == userId).ToList();
                if (exsit.Any())
                {
                    var fileItems = identityContext.FolderFiles.Where(i => i.FileFolderId.Equals(FolderId)).Select(i => i.FileId).ToList();
                    var files = identityContext.FileItems.Where(i => fileItems.Contains(i.FileItemId)).Select(i => new FileItemDTO(i)).ToList();

                    return files;
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<FileMaskModel> CreatFileMask(string userId, FileMaskDTO dto)
        {
            try
            {
                var FileItem = identityContext.FileItems.Single(i => i.FileItemId.Equals(dto.FileItemId));
                if (FileItem == null)
                {
                    return null;
                }
                else
                {
                    FileMaskModel mask = null;
                    // 获取标注
                    try
                    {
                        // 如果存在则进行更新
                        mask = identityContext.FileMasks.Single(i => i.CreatedBy.Equals(userId) && i.FileItemId.Equals(FileItem.FileItemId));

                        mask.Content = dto.Content;

                        FileItem.MaskId = mask.FileMaskId;

                        // 更新标记结果计数
                        if (mask.IsPositive != dto.IsPositive)
                        {
                            if (dto.IsPositive)
                            {
                                FileItem.Positive = FileItem.Positive + 1;
                                FileItem.Negative = FileItem.Negative - 1;
                            }
                            else
                            {
                                FileItem.Positive = FileItem.Positive - 1;
                                FileItem.Negative = FileItem.Negative + 1;
                            }
                            mask.IsPositive = dto.IsPositive;
                        }

                        
                    }
                    catch
                    {
                        // 否则新建
                        mask = new FileMaskModel { FileMaskId = dto.FileMaskId, FileItemId = dto.FileItemId, Content = dto.Content, IsPositive = dto.IsPositive, CreatedBy = userId };
                        identityContext.FileMasks.Add(mask);

                        // 改变文件的标注计数
                        if (mask.IsPositive)
                        {
                            FileItem.Positive = FileItem.Positive + 1;
                        }
                        else
                        {
                            FileItem.Negative = FileItem.Negative + 1;
                        }
                        
                    }

                    await identityContext.SaveChangesAsync();

                    return mask;


                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private FileItemModel GetFileItemAsync(string md5)
        {
            var item = identityContext.FileItems.AsNoTracking().Single(i => i.Md5.Equals(md5));
            return item;
        }
    }
}
