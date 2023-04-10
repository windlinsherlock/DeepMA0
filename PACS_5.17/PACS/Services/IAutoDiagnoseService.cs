using PACS.Shared.Contact;
using PACS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    public interface IAutoDiagnoseService
    {
        public Task<ApiResponse> AutoDiagnose(byte[] image);

        public  Task<ApiResponse> SaveAutoDiagnoseItem(AutoDiagnoseItemDTO autoDiagnoseItemDTO);
        public Task<ApiResponse> GetAutoDiagnoseItem(string autoDiagnoseItemId);

        public  Task<ApiResponse> GetAutoDiagnoseFolders();
    }
}
