using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PACS.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoLabelController : ControllerBase
    {
        private readonly IAutoLableService autoLableService;

        public AutoLabelController(IAutoLableService autoLableService)
        {
            this.autoLableService = autoLableService;
        }

        [HttpPost("Classify/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Classify(string id)
        {
            bool result = await autoLableService.ClassifyAsync(id);
            return Ok(new ApiResponse(true, result));
        }

    }
}
