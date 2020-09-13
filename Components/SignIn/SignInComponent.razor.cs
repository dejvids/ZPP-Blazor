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

        [Inject]
        public AppState AppState { get; set; }

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
#if Debgu
            Console.WriteLine("Logowanie"); 
#endif
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Podaj login i hasło";
                IsAlertVisible = true;
                return;
            }
            var user = new LoginUser { Login = Login, Password = Password };
#if Debug
            Console.WriteLine("User: " + user.Login + " " + user.Password); 
#endif
            var content = new StringContent(JsonSerializer.Serialize(user), System.Text.Encoding.UTF8, "application/json");
            var result = await Http.PostAsync(@"/api/sign-in", content);
#if Debug
            Console.WriteLine(result); 
#endif

            if (result == null)
            {
#if Debug
                Console.WriteLine("Result null"); 
#endif
                return;
            }
            var response = await result.Content?.ReadAsStringAsync();
#if Debug
            Console.WriteLine(response); 
#endif
            if (response == null)
            {
                ErrorMessage = "Logowanie zakończone niepowodzeniem.";
            }

            SignInResult obj = null;
            try
            {
                obj = JsonSerializer.Deserialize<SignInResult>(response, AppCtx.JsonOptions);
            }
            catch (Exception ex)
            {
#if Debug
                Console.WriteLine(ex.Message); 
#endif
            }
#if Debug
            Console.WriteLine($"obj:{obj}"); 
#endif

            if (obj == null)
            {
                ErrorMessage = "Błąd logowania";
            }

            if (obj.Success)
            {
                await SignInService.HandleSignIn(obj);
                // UriHelper.NavigateTo("/profil");
                IsSigned = true;
                AppState.SetSignInStatus(true);
                UriHelper.NavigateTo("/konto");
            }
            else
            {
                ErrorMessage = obj.Message;
                IsAlertVisible = true;
            }
        }



        public void SignInFacebook()
        {
#if Debug
            Console.WriteLine("Facebook login"); 
#endif
            string url = $"{AppCtx.BaseAddress}/sign-in-facebook/blazor";
#if Debug
            Console.WriteLine(url); 
#endif
            UriHelper.NavigateTo(url);
            // await HandleSignIn(result);
        }

        public void SignInGoogle()
        {
            Console.WriteLine("Google login");
            UriHelper.NavigateTo($"{AppCtx.BaseAddress}/sign-in-google/blazor");
        }
    }
}
