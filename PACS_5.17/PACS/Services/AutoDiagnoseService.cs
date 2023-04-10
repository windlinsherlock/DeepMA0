using PACS.Commons;
using PACS.Commons.Models;
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
    public class AutoDiagnoseService:IAutoDiagnoseService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "AutoDiagnose";
        UserConfiguration userConfiguration;

        public AutoDiagnoseService(HttpRestClient client,UserConfiguration userConfiguration)
        {
            //这里填入云端标记系统的url
            //this.client = new HttpRestClient("http://192.168.1.104:10000", userConfiguration);
            this.client = client;
            this.userConfiguration = userConfiguration;
        }
        public async Task<ApiResponse> AutoDiagnose(byte[] image)
        {
            //这里填入云端标记系统的url
            HttpRestClient client = new HttpRestClient("http://192.168.1.101:10000", userConfiguration);
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}";
            request.AutoDiagnoseImage = image;
            var result = await client.ExecuteAsync(request);

            return result;
        }

        public async Task<ApiResponse> SaveAutoDiagnoseItem(AutoDiagnoseItemDTO autoDiagnoseItemDTO)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/AutoDiagnoseItem";
            request.Parameter = autoDiagnoseItemDTO;
            //request.AutoDiagnoseImage = autoDiagnoseItem.Image;
            //request.ThermodynamicChart = autoDiagnoseItem.ThermodynamicChartBytes;
            //request.LabelImage = autoDiagnoseItem.LabelImageBytes;
            var result = await client.ExecuteAsync(request);

            return result;
        }
        public async Task<ApiResponse> GetAutoDiagnoseItem(string autoDiagnoseItemId)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/AutoDiagnoseItem/{autoDiagnoseItemId}";
            var result = await client.ExecuteAsync(request);

            return result;
        }
        public async Task<ApiResponse> GetAutoDiagnoseFolders()
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Get;
            request.Route = $"api/{serviceName}/AutoDiagnoseFolders";
            var result = await client.ExecuteAsync(request);

            return result;
        }


    }
}
