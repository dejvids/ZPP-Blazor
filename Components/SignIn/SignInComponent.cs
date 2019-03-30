using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Blazor.Services;
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
    public class SignInComponent : BaseComponent
    {
        [Inject]
        public SignInService SignInService { get; set; }
        public string Result { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsAlertVisible { get; set; }

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            if (IsSigned)
            {
                UriHelper.NavigateTo("/profil");
            }
            ErrorMessage = string.Empty;
            IsAlertVisible = false;
        }
        public async Task SignInApp()
        {
            ErrorMessage = string.Empty;
            IsAlertVisible = false;
            Console.WriteLine("Logowanie");
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Podaj login i hasło";
                IsAlertVisible = true;
                return;
            }
            var user = new LoginUser { Login = Login, Password = Password };
            Console.WriteLine("User: " + user.Login + " " + user.Password);
            var content = new StringContent(Json.Serialize(user), System.Text.Encoding.UTF8, "application/json");
            var result = await Http.PostAsync(@"/api/sign-in", content);
            Console.WriteLine(result);
            if (SignInService is null)
            {
                Console.WriteLine("Sign in null");
                return;
            }
            if (result == null)
            {
                Console.WriteLine("Result null");
                return;
            }
            var response = await result.Content?.ReadAsStringAsync();
            if (response == null)
            {
                ErrorMessage = "Logowanie zakończone niepowodzeniem.";
            }
            var obj = Json.Deserialize<SignInResult>(response);

            if (obj == null)
            {
                ErrorMessage = "Błąd logowania";
            }

            if (obj.Success)
            {
                await SignInService.HandleSignIn(obj);
                // UriHelper.NavigateTo("/profil");
                IsSigned = true;
                await JSRuntime.Current.InvokeAsync<bool>("reload", "/profil");
            }
            else
            {
                ErrorMessage = obj.Message;
                IsAlertVisible = true;
            }
        }



        public void SignInFacebook()
        {
            Console.WriteLine("Facebook login");

            UriHelper.NavigateTo($"{AppCtx.BaseAddress}/sign-in-facebook/blazor");
            // await HandleSignIn(result);
        }

        public void SignInGoogle()
        {
            Console.WriteLine("Google login");
            UriHelper.NavigateTo($"{AppCtx.BaseAddress}/sign-in-google/blazor");
        }
    }
}
