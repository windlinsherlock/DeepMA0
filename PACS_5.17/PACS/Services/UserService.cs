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
    /// <summary>
    /// 用户相关API
    /// </summary>
    class UserService : IUserService
    {
        private readonly HttpRestClient client; 
        private readonly string serviceName = "User";

        public UserService(HttpRestClient client)
        {
            this.client = client;
        }


        public async Task<ApiResponse> Login(LoginDTO user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Login";
            request.Parameter = new { UserName = user.UserName, Password = user.Password };

            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> Register(LoginDTO user)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Register";
            request.Parameter = new { UserName = user.UserName, Password = user.Password };

            var result = await client.ExecuteAsync(request);

            return result;
        }
    }
}
