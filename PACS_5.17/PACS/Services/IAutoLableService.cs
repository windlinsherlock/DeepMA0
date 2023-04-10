using PACS.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public interface IAutoLableService
    {
        public Task<ApiResponse> Classify(string FileID);
    }
}
