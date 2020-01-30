using Blazor.Extensions.Storage;
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
        string _developBaseAddress = @"https://localhost:5001";
        string _prodBaseAddress = @"https://zpp-api.azurewebsites.net";
        [Inject]
        protected HttpClient Http { get; set; }
        [Inject]
        protected SessionStorage SessionStorage { get; set; }

        [Inject]
        protected LocalStorage LocalStorage { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        protected bool IsSigned { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Http.BaseAddress = new Uri(_prodBaseAddress);
            AppCtx.BaseAddress = _prodBaseAddress;

            this.IsSigned = false;

            if (LocalStorage == null)
            {
                Console.WriteLine("localstorage is null");
            }
            var jwt = await LocalStorage.GetItem<JsonWebToken>("token");
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
                    await LocalStorage.RemoveItem("token");
                }
            }
            else
            {
                this.IsSigned = false;
            }
             this.StateHasChanged();
        }
    }
}
