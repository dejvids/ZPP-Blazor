using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ZPP_Blazor.Models;

namespace ZPP_Blazor.Services
{
    public class UsersService : IUsersService
    {
        private HttpClient _http;
        public UsersService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<UserDetail>> GeUsersAsync(int page)
        {
            try
            {

                var result = await _http.GetAsync($"/api/users/page/{page}");
                if (result == null)
                {
                    throw new Exception("Błą serwera");
                }
                if(result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(await result.Content.ReadAsStringAsync());
                }
                Console.WriteLine(result);
                string jsonCOntent = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<UserDetail>>(jsonCOntent, AppCtx.JsonOptions);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task SetUserRole(GrantRoleDto roleDto)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(roleDto), System.Text.Encoding.UTF8, "application/json");
                var result = await _http.PutAsync("/api/users/set-role", content);
                if(result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(await result.Content.ReadAsStringAsync());
                }
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
