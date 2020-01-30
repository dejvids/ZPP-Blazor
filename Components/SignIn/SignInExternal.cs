using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZPP_Blazor.Models;
using ZPP_Blazor.Services;

namespace ZPP_Blazor.Components.SignIn
{
    [Route("/signin-external")]
    public class SignInExternal : BaseComponent
    {
        [Inject]
        SignInService SignInService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var uri = new Uri(UriHelper.Uri);
            var accessToken = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("token", out var _token) ? _token.First() : "";
            var expires = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("expires", out var _expires) ? _expires.First() : "";
            var role = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("role", out var _role) ? _role.First() : "";

            long.TryParse(expires, out long expiresTimeStanmp);

            await SignInService.SetUserToken(new JsonWebToken { AccessToken = accessToken, Expires = expiresTimeStanmp, Role = role });
            UriHelper.NavigateTo("/konto");          
        }

    }
}
