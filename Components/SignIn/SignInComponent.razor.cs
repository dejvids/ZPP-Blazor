using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.SignIn
{
    public partial class SignInComponent
    {
        [Inject]
        public SignInService SignInService { get; set; }
        public string Result { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsAlertVisible { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (IsSigned)
            {
                UriHelper.NavigateTo("/konto");
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
            var content = new StringContent(JsonSerializer.Serialize(user));
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
            var obj = JsonSerializer.Deserialize<SignInResult>(response);

            if (obj == null)
            {
                ErrorMessage = "Błąd logowania";
            }

            if (obj.Success)
            {
                await SignInService.HandleSignIn(obj);
                // UriHelper.NavigateTo("/profil");
                IsSigned = true;
                await JSRuntime.InvokeAsync<bool>("reload", "/konto");
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
