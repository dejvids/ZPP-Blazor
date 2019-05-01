using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public interface IUsersService
    {
        Task<List<UserDetail>> GeUsersAsync(int page);
        Task SetUserRole(GrantRoleDto roleDto);
    }
}
