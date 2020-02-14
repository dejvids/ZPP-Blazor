using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ZPP_Blazor.Components.SignIn
{
    [Route("/wyloguj")]
    public class LogoutComponent : AppComponent
    {
        [Inject]
        public AppState AppState { get; set; }
        public LogoutComponent()
        { }
        protected override async Task OnInitializedAsync()
        {

            AppCtx.AccessToken = string.Empty;
            AppCtx.CurrentUser = null;
            Http.DefaultRequestHeaders.Authorization = null;
            if (LocalStorage == null)
            {
                Console.WriteLine("Localstorage is null");
                return;
            }
            await LocalStorage.RemoveItemAsync("token");

            AppState.SetSignInStatus(false);
            UriHelper.NavigateTo("/");
        }

    }
}
