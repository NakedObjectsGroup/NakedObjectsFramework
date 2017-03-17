using System.Web.Http;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

public class CorsConfig {
    public static void RegisterCors(HttpConfiguration httpConfig) {
        var corsConfig = new WebApiCorsConfiguration();

        // this adds the CorsMessageHandler to the HttpConfiguration’s

        // MessageHandlers collection

        corsConfig.RegisterGlobal(httpConfig);

        // this allow all CORS requests to the Products controller

        // from the http://foo.com origin.

        corsConfig.ForResources("RestfulObjects").
            ForOrigins("http://localhost:5001").
           AllowAll().
           AllowResponseHeaders("Warning", "Set-Cookie", "ETag").
           AllowCookies();

    }
}