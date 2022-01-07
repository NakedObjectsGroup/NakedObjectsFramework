using System;
using Microsoft.Extensions.DependencyInjection;
using NakedLegacy.Types.Container;

namespace AdventureWorksLegacy.AppLib;

public static class ThreadLocals {
    private static IServiceProvider serviceProvider;
    private static Func<IServiceProvider, IContainer> containerFactory;

    public static void Initialize(IServiceProvider sp, Func<IServiceProvider, IContainer> cf) {
        serviceProvider = sp;
        containerFactory = cf;
    }

    private static IContainer GetContainer() {
        var scopeSp = serviceProvider.CreateScope().ServiceProvider;
        return containerFactory(scopeSp);
    }

    public static IContainer Container => GetContainer();
}