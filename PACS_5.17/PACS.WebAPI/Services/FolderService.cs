using PACS.Shared.Entities;
using PACS.Shared.Parameters;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public class FolderService : IBaseService<FileFolderModel>
    {
        public Task<ApiResponse> AddAsync(FileFolderModel model)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse> DeleteAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse> GetAllAsync(QueryParameter query)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse> GetSingleAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ApiResponse> UpdateAsync(FileFolderModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
