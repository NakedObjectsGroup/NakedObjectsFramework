using System;
using NakedLegacy;

namespace NakedLegacy.Rest.Test.Data.AppLib;

public static class ThreadLocals {
    [ThreadStatic]
    private static IServiceProvider serviceProvider;

    private static Func<IServiceProvider, IContainer> containerFactory;

    public static IContainer Container => GetContainer();

    public static void Initialize(IServiceProvider sp, Func<IServiceProvider, IContainer> cf) {
        serviceProvider = sp;
        containerFactory = cf;
    }

    public static void Reset() {
        serviceProvider = null;
        containerFactory = null;
    }

    private static IContainer GetContainer() => containerFactory(serviceProvider);
}