using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public interface IAutoLableService
    {
        Task<bool> ClassifyAsync(string filename);
    }
}
