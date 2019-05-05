using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
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
        private Role _selectedRole;

        public bool DeleteConfVisible { get; set; }

        protected List<Company> Companies { get; set; } = new List<Company>();
        protected Company SelectedCompany { get; set; } = new Company();
        protected Role SelectedRole
        {
            get { return _selectedRole; }
            set { _selectedRole = value; SelectedCompany = Companies.FirstOrDefault(); StateHasChanged(); }
        }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            await LoadUsers();
            await LoadCompanies();

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

        private async Task LoadCompanies()
        {
            try
            {
                var result = await Http.GetAsync("/api/companies");
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Companies = Json.Deserialize<List<Company>>(await result.Content.ReadAsStringAsync());
                    SelectedCompany = Companies.FirstOrDefault();
                    StateHasChanged();
                }
                else
                {
                    Console.WriteLine(await result.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async Task SetRole()
        {
            Console.WriteLine(SelectedCompany.Name + " " + SelectedUser.CompanyName);
            if (SelectedRole == Role.Wykładowca && SelectedCompany.Id == SelectedUser.CompanyId)
            {
                ShowUserRole = false;
                StateHasChanged();
                return;
            }
            if ((int)SelectedRole == SelectedUser.RoleId && SelectedUser.RoleId == (int)Role.Student)
            {
                ShowUserRole = false;
                StateHasChanged();
                return;
            }
            Console.WriteLine("OK");
            var grantContent = new GrantRoleDto()
            {
                UserId = SelectedUser.Id,
                RoleId = (int)SelectedRole,
                CompanyId = SelectedCompany.Id
            };

            try
            {
                await _usersService.SetUserRole(grantContent);
                ShowUserRole = false;
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
                SelectedCompany = Companies.FirstOrDefault(c => c.Id == SelectedUser.CompanyId);
                SelectedRole = (Role)SelectedUser.RoleId;
                ShowUserRole = true;
                StateHasChanged();
            }
        }

        protected void ShowDeleteConfirmation(UserDetail user)
        {
            DeleteConfVisible = true;
            SelectedUser = user;
            StateHasChanged();
        }

        protected async Task DeleteUser()
        {
            Console.WriteLine("Deleting user");
            try
            {
                await Http.DeleteAsync($"/api/users/{SelectedUser.Id}");
                await LoadUsers();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                this.DeleteConfVisible = false;
                StateHasChanged();
            }
        }

    }
}
