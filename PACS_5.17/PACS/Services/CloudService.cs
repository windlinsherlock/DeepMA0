using PACS.Commons;
using PACS.Service;
using PACS.Shared.Contact;
using PACS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public class CloudService : ICloudService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Cloud";
        UserConfiguration userConfiguration;

        public CloudService(HttpRestClient client, UserConfiguration userConfiguration)
        {
            this.client = client;
            this.userConfiguration = userConfiguration;
        }

        /// <summary>
        /// 获取公共文件夹目录
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetPublicFolders()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/PublicFileFolder";


            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 获取特定公共文件夹下的文件目录
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetPublicFiles(string FolderId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/PublicFileFolder/{FolderId}";

            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 将公共文件夹保存至我的文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> CopyFolder(string FolderId,bool re)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/PublicFileFolder/{FolderId}";
            request.Parameter = new { AccessModifier = re ? "public" : "private" };

            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 获取用户的所有文件夹目录
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> GetFolders()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileFolder";
            

            var result = await client.ExecuteAsync(request);

            return result;
        }



        /// <summary>
        /// 创建一个云端文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <param name="AccessModifier"></param>
        /// <returns></returns>
        public async Task<ApiResponse> CreateFolder(string name,string AccessModifier)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/FileFolder";
            request.Parameter = new { FolderName = name, AccessModifier = AccessModifier };

            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> DeleteFolder(string FolderId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Delete;
            request.Route = $"api/{serviceName}/FileFolder/{FolderId}";

            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 向特定文件夹上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> UploadFile(List<string> path,string FolderId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/FileFolder/{FolderId}";
            request.FilePath = path;

            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 获取文件夹下的文件列表
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetFiles(string FolderId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileFolder/{FolderId}";

            var result = await client.ExecuteAsync(request);

            return result;
        }

       

        /// <summary>
        /// 获取特定文件
        /// </summary>
        /// <param name="FileItemId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetFileItem(string FileItemId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileItem/{FileItemId}";

            var result = await client.ExecuteAsync(request);

            return result;
        }

        /// <summary>
        /// 上传标注结果
        /// </summary>
        /// <param name="path"></param>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public async Task<ApiResponse> UploadMask(FileMaskDTO dto)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/FileItem/FileMask";
            request.Parameter = dto;

            var result = await client.ExecuteAsync(request);

            return result;
        }

    }
}
