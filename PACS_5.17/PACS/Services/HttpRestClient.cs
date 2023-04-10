using PACS.Shared.Contact;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Service
{
    /// <summary>
    /// 操作RestSharp中间件实现HTTP通信
    /// </summary>
    public class HttpRestClient
    {
        private readonly string apiUrl;
        protected readonly RestClient client;

        private Commons.UserConfiguration userConfiguration;

        public HttpRestClient(string apiUrl, Commons.UserConfiguration userConfiguration)
        {
            this.apiUrl = apiUrl;
            this.userConfiguration = userConfiguration;
            client = new RestClient(apiUrl);
        }

        

        public async Task<ApiResponse> ExecuteAsync(BaseRequest baseRequest)
        {
            // 若用户已登录，则附上用户的JWT
            if (userConfiguration.Token != null)
                client.Authenticator = new RestSharp.Authenticators.JwtAuthenticator(userConfiguration.Token.AccessToken);

            var request = new RestRequest(baseRequest.Route, baseRequest.Method);

            

            if (baseRequest.Parameter != null)
                request.AddBody(baseRequest.Parameter);

            if (baseRequest.FilePath != null)
            {
                foreach (var filePath in baseRequest.FilePath)
                    request.AddFile(filePath.Split("/").Last(), filePath);
            }

            //添加自动标注图片
            if (baseRequest.AutoDiagnoseImage != null)
            {
                 request.AddFile("AutoDiagnoseImage", baseRequest.AutoDiagnoseImage,"fileName");
            }
            //添加自动诊断热力图
            if (baseRequest.ThermodynamicChart != null)
            {
                request.AddFile("ThermodynamicChart", baseRequest.ThermodynamicChart, "fileName");
            }
            //添加自动诊断标注图
            if (baseRequest.LabelImage != null)
            {
                request.AddFile("LabelImage", baseRequest.LabelImage, "fileName");
            }


            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ApiResponse>(response.Content);
            }
            else
                return new ApiResponse()
                {
                    Status = false,
                    Result = null,
                    Message = response.ErrorMessage
                };
        }

        public async Task<ApiResponse<T>> ExecuteAsync<T>(BaseRequest baseRequest)
        {
            // 若用户已登录，则附上用户的JWT
            if (userConfiguration.Token != null)
                client.Authenticator = new RestSharp.Authenticators.JwtAuthenticator(userConfiguration.Token.AccessToken);

            var request = new RestRequest(baseRequest.Route, baseRequest.Method);

            if (baseRequest.Parameter != null)
                request.AddParameter("param", JsonConvert.SerializeObject(baseRequest.Parameter), ParameterType.RequestBody);
            //client.BaseUrl = new Uri(apiUrl + baseRequest.Route);
            var response = await client.ExecuteAsync(request); 
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<ApiResponse<T>>(response.Content);

            else
                return new ApiResponse<T>()
                {
                    Status = false, 
                    Message = response.ErrorMessage
                };
        }
    }
}
