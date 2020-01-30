using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components
{
    public class AppComponent : BaseComponent
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //var token = AppCtx.AccessToken ?? (await SessionStorage.GetItem<JsonWebToken>("token"))?.AccessToken ?? (await LocalStorage.GetItem<JsonWebToken>("token"))?.AccessToken;
            if (IsSigned)
            {
                var token = (await LocalStorage.GetItem<JsonWebToken>("token"));
                Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                AppCtx.CurrentUser = new User()
                {
                    Role = token.Role
                };

            }
            else
            {
                UriHelper.NavigateTo("/logowanie");
            }
        }
    }
}
