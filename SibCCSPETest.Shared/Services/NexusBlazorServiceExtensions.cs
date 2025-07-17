using Microsoft.Extensions.DependencyInjection;
using SibCCSPETest.Shared.Components;

namespace SibCCSPETest.Shared.Services
{
    public static class NexusBlazorServiceExtensions
    {
        public static void AddNexusBlazor(this IServiceCollection services)
        {
            services.AddSingleton<INexusNotificationService, NexusNotificationService>();
            services.AddSingleton<INexusDialogService, NexusDialogService>();
        }
    }
}
