using Lumen.Security;
using Microsoft.Owin.Extensions;
using Owin;

namespace Lumen.AspNetMvc.Security.Owin
{
    public static class ApplicationAuthenticationMiddlewareExtensions
    {
        public static IAppBuilder UseApplicationAuthentication<TMiddleware, TUser>(this IAppBuilder app)
            where TMiddleware : ApplicationAuthenticationMiddleware<TUser>
            where TUser : class, IUser
        {
            Ensure.NotNull(app, "app");

            app.Use<TMiddleware>();
            app.UseStageMarker(PipelineStage.Authenticate);

            return app;
        }
    }
}