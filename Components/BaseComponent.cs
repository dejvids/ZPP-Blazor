using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Extensions;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components
{
    public class BaseComponent : ComponentBase
    {
        string _developBaseAddress = @"http://localhost:5000";
        string _prodBaseAddress = @"https://zpp-api.azurewebsites.net";
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected ILocalStorageService LocalStorage { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        public AppState AppState { get; set; }
        protected bool IsSigned { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //Http.BaseAddress = new Uri(_prodBaseAddress);
            AppCtx.BaseAddress = _developBaseAddress;
            AppCtx.BaseAddress = _prodBaseAddress;

            this.IsSigned = false;

            if (LocalStorage == null)
            {
#if Debug
                Console.WriteLine("localstorage is null"); 
#endif
            }
            var jwt = await LocalStorage.GetItemAsync<JsonWebToken>("token");
            if (jwt != null)
            {
#if Debug
                Console.WriteLine("Expires:" + jwt.Expires); 
#endif
                var currentTimeStamp = DateTime.UtcNow.ToTimestamp();
                if (jwt.Expires > currentTimeStamp)
                {
#if Debug
                    Console.WriteLine("Czas tokena: " + jwt.Expires + "Czas UTC " + currentTimeStamp); 
#endif
                    this.IsSigned = true;
                }
                else
                {
                    this.IsSigned = false;
                    await LocalStorage.RemoveItemAsync("token");
                }
            }
            else
            {
                this.IsSigned = false;
            }
            AppState.SetSignInStatus(IsSigned);
        }
    }
}
