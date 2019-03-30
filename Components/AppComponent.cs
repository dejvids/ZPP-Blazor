using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components
{
    public class AppComponent : BaseComponent
    {
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            //var token = AppCtx.AccessToken ?? (await SessionStorage.GetItem<JsonWebToken>("token"))?.AccessToken ?? (await LocalStorage.GetItem<JsonWebToken>("token"))?.AccessToken;
            var token = (await LocalStorage.GetItem<JsonWebToken>("token"))?.AccessToken;
            Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
