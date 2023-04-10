using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.DTOs
{
    public class RoleDTO
    {
        [DisplayName("角色名")]
        public string RoleName { get; set; }
    }
}
