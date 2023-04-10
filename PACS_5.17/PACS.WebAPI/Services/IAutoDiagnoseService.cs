using Microsoft.AspNetCore.Http;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public interface IAutoDiagnoseService
    {
        Task<bool> CreateAutoDiagnoseItem(string userId,AutoDiagnoseItemDTO autoDiagnoseItemDTO);
        Task<AutoDiagnoseItemDTO> GetAutoDiagnoseItem(string userId, string autoDiagnoseItemId);

        List<AutoDiagnoseFolderDTO> GetAutoDiagnoseFolders(string userId);

    }
}
