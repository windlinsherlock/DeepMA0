using PACS.Shared.Entities;
using PACS.WebAPI.Data;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Xml.Linq;
using System.Linq;
using PACS.Shared.DTOs;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PACS.WebAPI.Services
{
    public class AutoDiagnoseService:IAutoDiagnoseService
    {
        private readonly IdentityContext identityContext;

        public AutoDiagnoseService(IdentityContext identityContext)
        {
            this.identityContext = identityContext;
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        //public async Task<bool> CreateAutoDiagnoseItem(string UserId,string fileName, IFormFile[] files)
        //{
        //    AutoDiagnoseItemModel autoDiagnoseItem = new AutoDiagnoseItemModel();
        //    autoDiagnoseItem.UserId = UserId;
        //    autoDiagnoseItem.Name = fileName;
        //    for(int i = 0; i < files.Length; i++)
        //    {
        //        var file = files[i];
        //        var fileStream = file.OpenReadStream();
        //        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        //        fileStream.CopyTo(stream);
        //        switch (i)
        //        {
        //            case 0:
        //                autoDiagnoseItem.Image=stream.ToArray();
        //                break;
        //            case 1:
        //                autoDiagnoseItem.ThermodynamicChart= stream.ToArray();
        //                break;
        //            case 2:
        //                autoDiagnoseItem.LabelImage=stream.ToArray();
        //                break;
        //        }
        //        fileStream.Dispose();
        //        stream.Dispose();

        //    }
        //    var item = autoDiagnoseItem;
        //    try
        //    {
        //        var result = identityContext.AutoDiagnoseItems.Single(i => i.Name == item.Name&&i.UserId==item.UserId);
        //        // 该文件名未存在
        //        if (result == null)
        //        {
        //            try //
        //            {
        //                identityContext.AutoDiagnoseItems.Add(item);
        //            }
        //            catch // 表中已存在该文件
        //            {
        //                await identityContext.SaveChangesAsync();
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            await identityContext.SaveChangesAsync();
        //            return false;
        //        }
        //    }
        //    catch (Exception ex) // 表中无数据，single会抛出异常，此时直接添加即可。
        //    {
        //        identityContext.AutoDiagnoseItems.Add(item);
        //    }
        //    await identityContext.SaveChangesAsync();
        //    return true;
        //}

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="autoDiagnoseItemDTO"></param>
        /// <returns></returns>
        public async Task<bool> CreateAutoDiagnoseItem(string userId,AutoDiagnoseItemDTO autoDiagnoseItemDTO)
        {
            AutoDiagnoseItemModel autoDiagnoseItem = autoDiagnoseItemDTO.TurnToModel();
            autoDiagnoseItem.UserId = userId;
            var item = autoDiagnoseItem;
            try
            {
                var result = identityContext.AutoDiagnoseItems.Single(i => i.Name == item.Name && i.UserId == item.UserId);
                // 该文件名未存在
                if (result == null)
                {
                    try //
                    {
                        var entity=identityContext.AutoDiagnoseItems.Add(item);
                    }
                    catch // 表中已存在该文件
                    {
                        await identityContext.SaveChangesAsync();
                        return false;
                    }
                }
                else
                {
                    await identityContext.SaveChangesAsync();
                    return false;
                }
            }
            catch (Exception ex) // 表中无数据，single会抛出异常，此时直接添加即可。
            {
                identityContext.AutoDiagnoseItems.Add(item);
            }
            await identityContext.SaveChangesAsync();
            return true;
        }

        public async Task<AutoDiagnoseItemDTO> GetAutoDiagnoseItem(string userId, string autoDiagnoseItemId)
        {
            var item= identityContext.AutoDiagnoseItems.Single(i => i.AutoDiagnoseItemId.Equals(autoDiagnoseItemId)&&i.UserId.Equals(userId));
            return new AutoDiagnoseItemDTO(item);
        }

        public List<AutoDiagnoseFolderDTO> GetAutoDiagnoseFolders(string userId)
        {
            List<AutoDiagnoseFolderDTO> item = identityContext.AutoDiagnoseItems.Where(i => i.UserId.Equals(userId)).Select(i => new AutoDiagnoseFolderDTO(i.AutoDiagnoseItemId, i.Name)).ToList();
            return item;
            //var result = identityContext.AutoDiagnoseItems.Where(i => i.UserId.Equals(userId));

            //if (result == null)
            //{
            //    return null;
            //}

            //List<AutoDiagnoseFolderDTO> folders = new List<AutoDiagnoseFolderDTO>();

            //List<string> folders = result.Select(i => i.FileFolderId).ToList();

            //model.AddRange(identityContext.AutoDiagnoseItems.Where(i => folders.Contains(i.FileFolderId)).ToList());

            //return model;
        }
    }
}
