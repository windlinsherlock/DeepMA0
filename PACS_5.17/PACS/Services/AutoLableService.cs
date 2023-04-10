using PACS.Commons;
using PACS.Service;
using PACS.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public class AutoLableService : IAutoLableService
    {
        private readonly HttpRestClient client;
        private readonly string serviceName = "AutoLabel";
        UserConfiguration userConfiguration;

        public AutoLableService(HttpRestClient client, UserConfiguration userConfiguration)
        {
            this.client = client;
            this.userConfiguration = userConfiguration;
        }

        public async Task<ApiResponse> Classify(string FileID)
        {
            BaseRequest request = new BaseRequest();
            request.Method = RestSharp.Method.Post;
            request.Route = $"api/{serviceName}/Classify/{FileID}";

            var result = await client.ExecuteAsync(request);

            return new ApiResponse {Status = true,Result=false };
            return result;
        }
    }
}
