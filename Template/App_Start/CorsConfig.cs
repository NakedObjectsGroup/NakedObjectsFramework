using System.Web.Http;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace NakedObjects.Template
{
    public class CorsConfig
    {
        public static void RegisterCors(HttpConfiguration httpConfig)
        {
            var corsConfig = new WebApiCorsConfiguration();
            corsConfig.RegisterGlobal(httpConfig);
            corsConfig.ForResources("RestfulObjects").
                ForOrigins("http://localhost:49998").
               AllowAll().
               AllowResponseHeaders("Warning", "Set-Cookie", "ETag").
               AllowCookies();

        }
    }
}