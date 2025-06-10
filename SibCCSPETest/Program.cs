using MudBlazor.Services;
using SibCCSPETest.ServiceBase;
using System.Net.Http.Headers;

namespace SibCCSPETest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddMudServices();

            builder.Services.AddHttpClient("HttpClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7183");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<IAPIService, APIService>();

            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies([typeof(Admin._Imports).Assembly, typeof(User._Imports).Assembly]);

            app.Run();
        }
    }
}