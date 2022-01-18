using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AdventureWorksLegacy.AppLib;

//public static class ThreadLocals {
//    [ThreadStaticAttribute]
//    private static IServiceProvider? serviceProvider;

//    private static Func<IServiceProvider, IContainer>? containerFactory;

//    public static IContainer Container => GetContainer();

//    public static void Initialize(IServiceProvider sp, Func<IServiceProvider, IContainer> cf) {
//        serviceProvider = sp;
//        containerFactory = cf;
//    }

//    public static void Reset() {
//        serviceProvider = null;
//        containerFactory = null;
//    }

//    private static IContainer GetContainer() => containerFactory(serviceProvider!);
//}

public static class ThreadLocals {
    private static IHttpContextAccessor? httpContextAccessor;

    private static Func<IServiceProvider, IContainer>? containerFactory;

    public static IContainer Container => GetContainer();

    public static void Initialize(IServiceProvider sp, Func<IServiceProvider, IContainer> cf) {
        httpContextAccessor = sp.GetService<IHttpContextAccessor>();
        containerFactory = cf;
    }

    public static void InitializeContainer(IServiceProvider scopedSp) {
        var httpContext = httpContextAccessor.HttpContext;
        httpContext.Items["Container"] = containerFactory(scopedSp);
    }

    private static IContainer GetContainer() {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext.Items["Container"] as IContainer;
    }
}