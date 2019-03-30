using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Extensions;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components
{
    public class BaseComponent : BlazorComponent
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
        protected IUriHelper UriHelper { get; set; }

        protected bool IsSigned { get; set; }

        protected override async Task OnInitAsync()
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
                }
            }
            else
            {
                this.IsSigned = false;
            }
            Console.WriteLine("Base Component issigned = "+ IsSigned);
             this.StateHasChanged();
        }
    }
}
