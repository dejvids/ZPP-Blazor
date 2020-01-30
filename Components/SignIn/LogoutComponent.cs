using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.SignIn
{
    [Route("/wyloguj")]
    public class LogoutComponent : AppComponent
    {
        public LogoutComponent()
        { }
        protected override async Task OnInitializedAsync()
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
