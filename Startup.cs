using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using ZPP_Blazor.Services;

namespace ZPP_Blazor
{
    public class Startup
    {
        private static string m_devBaseAddress = "http://localhost:5000";
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddTransient<SignInService>();
            services.AddTransient<ILectureService,LectureService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IOpinionService, OpinionService>();
            services.AddSingleton<AppState>();
            services.AddScoped<HttpClient>(s =>
            {
                return new HttpClient
                {
                    BaseAddress = new System.Uri(m_devBaseAddress)
                };
            });
        }
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
