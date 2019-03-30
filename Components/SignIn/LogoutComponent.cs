using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.SignIn
{
    [Route("/wyloguj")]
    public class LogoutComponent : AppComponent
    {
        public LogoutComponent()
        { }
        protected override async Task OnInitAsync()
        {    
            AppCtx.AccessToken = string.Empty;
            AppCtx.CurrentUser = null;
            Http.DefaultRequestHeaders.Authorization = null;
            if(LocalStorage == null)
            {
                Console.WriteLine("Localstorage is null");
                return;
            }
            await LocalStorage.RemoveItem("token");
            
            UriHelper.NavigateTo("/");   
        }

    }
}
