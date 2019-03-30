using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
using Microsoft.JSInterop;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Components.Me
{
    public class MeComponent : AppComponent
    {

        public User User { get; set; }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            Console.WriteLine("Navigated to me");
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (Http == null)
            {
                Console.WriteLine("Http null");
                return;
            }
            Console.WriteLine("AuthorizatioN: "+Http.DefaultRequestHeaders.Authorization);
            var response = await Http.GetAsync(@"/api/me");
            Console.WriteLine("request sent");

            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Load user OK");
                string jsonContent = await response.Content.ReadAsStringAsync();
                User = Json.Deserialize<User>(jsonContent);
                AppCtx.CurrentUser = User;
            }

            else if (response == null)
            {
                Console.WriteLine("No response");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Unauthorized");
                UriHelper.NavigateTo("/");
            }
        }
    }
}