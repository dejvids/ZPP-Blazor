using System;
using System.Threading.Tasks;
using ZPP_Blazor.Models;
using Blazored.LocalStorage;

namespace ZPP_Blazor.Services
{
    public class SignInService
    {
        ILocalStorageService _localStorage;

        public SignInService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async Task<bool> HandleSignIn(SignInResult result)
        {
            if (result.Success)
            {
                await SetUserToken(result.Token);
                return true;
            }
            Console.WriteLine(result.Message);
            return false;
        }

        public async Task SetUserToken(JsonWebToken token)
        {
            AppCtx.AccessToken = token?.AccessToken;
            await _localStorage.SetItemAsync("token", token);
        }
    }
}
