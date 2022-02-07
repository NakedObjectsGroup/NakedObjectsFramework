using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace NOF2.Demo.AppLib;

public static class ThreadLocals {
    private static IHttpContextAccessor? httpContextAccessor;

    private static Func<IServiceProvider, IContainer>? containerFactory;

    public static IContainer Container => GetContainer();

    public static void Initialize(IServiceProvider sp, Func<IServiceProvider, IContainer> cf) {
        httpContextAccessor = sp.GetService<IHttpContextAccessor>();
        containerFactory = cf;
    }

    public static void SetContainer(IServiceProvider scopedSp) {
        var httpContext = httpContextAccessor.HttpContext;
        httpContext.Items["Container"] = containerFactory(scopedSp);
    }

    private static IContainer GetContainer() {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext.Items["Container"] as IContainer;
    }
}