using PACS.Shared.Contact;
using PACS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Services
{
    interface IUserService
    {
        Task<ApiResponse> Login(LoginDTO user);

        Task<ApiResponse> Register(LoginDTO user);
    }
}
