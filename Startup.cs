using Microsoft.AspNetCore.Blazor.Builder;
using Blazor.Extensions.Storage;
using Microsoft.Extensions.DependencyInjection;
using ZPP_Blazor.Services;

namespace ZPP_Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Blazor.Extensions.Storage
            // Both SessionStorage and LocalStorage are registered
            services.AddStorage();
            services.AddTransient<SignInService>();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
