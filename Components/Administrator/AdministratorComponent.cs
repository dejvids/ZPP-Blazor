using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Enums;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.Administrator
{
    public class AdministratorComponent : AppComponent
    {

        [Inject]
        public IUsersService _usersService { get; set; }
        protected List<UserDetail> Users { get; private set; } = new List<UserDetail>();
        public bool Loaded { get; set; }
        protected UserDetail SelectedUser { get; set; } = new UserDetail();
        protected bool ShowUserRole { get; set; }
        protected List<Role> AvalibleRoles = new List<Role> { Role.Student, Role.Wykładowca };
        protected Role SelectedRole { get; set; }
        public AdministratorComponent()
        {

        }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            Console.WriteLine("Init admin");
            await LoadUsers();

        }
        private async Task LoadUsers()
        {
            Users = new List<UserDetail>();
            try
            {
                Users = await _usersService.GeUsersAsync(1);
                Loaded = true;
                StateHasChanged();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async Task SetRole()
        {
            Console.WriteLine("Set user role");
            if ((int)SelectedRole == SelectedUser.RoleId)
            {
                return;
            }

            var grantContent = new GrantRoleDto()
            {
                UserId = SelectedUser.Id,
                RoleId = (int)SelectedRole,
            };

            try
            {
                await _usersService.SetUserRole(grantContent);
                await LoadUsers();
                StateHasChanged();
                Console.WriteLine("Role granted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        protected void ShowSelectRoleDialog(UserDetail user)
        {
            if (user.RoleId == (int)Role.Student || user.RoleId == (int)Role.Wykładowca)
            {
                SelectedUser = user;
                SelectedRole = (Role)SelectedUser.RoleId;
                ShowUserRole = true;
                StateHasChanged();
            }
        }

        protected void ShowDeleteConfirmation(UserDetail user)
        {

        }

    }
}
