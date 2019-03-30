using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public class SignInService
    {
        SessionStorage _sessionStorage;
        LocalStorage _localStorage;

        public SignInService(SessionStorage sessionStorage, LocalStorage localStorage)
        {
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
        }
        public async Task<bool> HandleSignIn(SignInResult result)
        {
            if(_sessionStorage == null)
            {
                Console.WriteLine("Sessionstorage is null");
                return false;
            }
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
            //await _sessionStorage.SetItem<JsonWebToken>("token", token);
            await _localStorage.SetItem<JsonWebToken>("token", token);
        }
    }
}
