using System.Text.Json;
using ZPP_Blazor.Models;

namespace ZPP_Blazor
{
    public static class AppCtx
    {
        public static User CurrentUser {get; set;}
        public static string AccessToken {get; set;}

        public static string BaseAddress { get; set; }
        public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }
}