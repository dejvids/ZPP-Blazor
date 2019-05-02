using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.Shared
{
    [Route("/konto")]
    public class UserComponent:AppComponent
    {
        protected async override Task OnInitAsync()
        {
            Console.WriteLine("User component");
            await base.OnInitAsync();
            Console.WriteLine("Current user role is " + AppCtx.CurrentUser.Role);
            if(AppCtx.CurrentUser.Role.Equals("student", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/student");
                return;
            }
            else if(AppCtx.CurrentUser.Role.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/wykladowca");
                return;
            }
            else if(AppCtx.CurrentUser.Role.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                UriHelper.NavigateTo("/admin");
                return;
            }
            UriHelper.NavigateTo("/");
        }
    }
}
