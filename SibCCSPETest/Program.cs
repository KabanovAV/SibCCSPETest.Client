using SibCCSPETest.Shared;
using SibCCSPETest.Shared.Services;

namespace SibCCSPETest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.ConfigureHttpClient();
            builder.Services.ConfigureAPI();
            builder.Services.AddNexusBlazor();

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