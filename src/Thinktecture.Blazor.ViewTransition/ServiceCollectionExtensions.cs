using Microsoft.Extensions.DependencyInjection;

namespace Thinktecture.Blazor.ViewTransition
{
    public static class ServiceCollectionExtensions
    {
        public static void AddViewTransitionServices(this IServiceCollection services)
        {
            services.AddScoped<IViewTransitionService, ViewTransitionService>();
        }
    }
}
