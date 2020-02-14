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

            this.IsSigned = false;

            if (LocalStorage == null)
            {
                Console.WriteLine("localstorage is null");
            }
            var jwt = await LocalStorage.GetItemAsync<JsonWebToken>("token");
            if (jwt != null)
            {
                Console.WriteLine("Expires:" + jwt.Expires);
                var currentTimeStamp = DateTime.UtcNow.ToTimestamp();
                if (jwt.Expires > currentTimeStamp)
                {
                    Console.WriteLine("Czas tokena: " + jwt.Expires + "Czas UTC " + currentTimeStamp);
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
