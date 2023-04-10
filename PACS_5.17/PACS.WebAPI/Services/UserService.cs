using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using PACS.Shared.Contact;
using PACS.WebAPI.Areas.Identity.Data;
using PACS.WebAPI.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PACS.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        private readonly JwtSettings jwtSettings;

        private readonly IdentityContext identityContext;

        private IEmailSender emailSender;

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            JwtSettings jwtSettings, IdentityContext identityContext,
            IEmailSender emailSender)
        {

            identityContext.Database.EnsureCreated();
            this.userManager = userManager;
            this.signInManager = signInManager;

            this.jwtSettings = jwtSettings;

            this.identityContext = identityContext;
            this.emailSender = emailSender;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<RegisterResult> RegisterAsync(string username, string password)
        {
            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                return new RegisterResult()
                {
                    Errors = new[] { "该用户名已被占用！" }, //用户已存在
                };
            }

            var newUser = new AppUser() { UserName = username, Email = username };
            var isCreated = await userManager.CreateAsync(newUser, password);

            if (!isCreated.Succeeded)
            {
                return new RegisterResult()
                {
                    Errors = isCreated.Errors.Select(p => p.Description)
                };
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));            

            return new RegisterResult(){ Code = code,Id = newUser.Id };
        }


        public async Task<TokenResult> LoginAsync(string username, string password)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, false, true);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(username);
                var token = GenerateJwtToken(user);

                return token;
            }
            if (result.IsLockedOut)
            {
                return new TokenResult()
                {
                    Errors = new[] { "该用户被锁定，请五分钟后再次尝试登录！" },
                };
            }
            else if (result.IsNotAllowed)
            {
                return new TokenResult()
                {
                    Errors = new[] { "请前往邮箱进行认证！" },
                };
            }
            else
            {
                
                return new TokenResult()
                {
                    Errors = new[] { "用户名或密码错误！" },
                };
            }
            
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);

            return result;
        }


        private TokenResult GenerateJwtToken(AppUser user)
        {
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecurityKey);

            // 加入角色
            var rolesId = identityContext.UserRoles.Where(i => i.UserId.Equals(user.Id)).Select(i => i.RoleId);
            var roles = identityContext.Roles.Where(i => rolesId.Contains(i.Id)).Select(i => i.Name);
            var claims = new List<Claim>();

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                

                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(ClaimTypes.Sid, user.Id),
                }),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(jwtSettings.ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            foreach(var claim in claims)
            {
                tokenDescriptor.Subject.AddClaim(claim);
            }
            

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(securityToken);

            bool isAdmin = false;
            if (roles.Any())
            {
                if(roles.Contains("Admin"))
                    isAdmin = true;
            }

            return new TokenResult()
            {
                AccessToken = token,
                TokenType = "Bearer",
                UserId = user.Id,
                IsAdmin = isAdmin,
            };
        }
    }
}
