using PACS.Service;
using PACS.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "Admin";

        public AdminService(HttpRestClient client)
        {
            this.client = client;
        }

        public async Task<ApiResponse> Folder()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileFolder";
            

            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> Folder(string folderId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileFolder/{folderId}";


            var result = await client.ExecuteAsync(request);

            return result;
        }


        public async Task<ApiResponse> Mask(string FileItemId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/FileItem/{FileItemId}/FileMask";


            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> User()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/User";


            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> Role()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/Role";


            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> SetItemMask(string FileItemId, string MaskId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Put;
            request.Route = $"api/{serviceName}/FileItem/{FileItemId}";
            request.Parameter = new Shared.DTOs.FileMaskDTO { FileMaskId = MaskId};


            var result = await client.ExecuteAsync(request);

            return result;
        }

    }
}
