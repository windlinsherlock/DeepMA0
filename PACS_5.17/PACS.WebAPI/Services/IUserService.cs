using Microsoft.AspNetCore.Identity;
using PACS.Shared.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public interface IUserService
    {

        Task<TokenResult> LoginAsync(string username, string password);

        Task<RegisterResult> RegisterAsync(string username, string password);

        public Task<IdentityResult> ConfirmEmailAsync(string userId, string code);
    }

    public class RegisterResult{
        public bool Success => Errors == null || !Errors.Any();

        public IEnumerable<string> Errors { get; set; }

        public string Id { get; set; }
        public string Code { get; set; }
    }
}
