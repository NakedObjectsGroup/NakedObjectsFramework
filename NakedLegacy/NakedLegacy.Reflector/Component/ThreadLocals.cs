using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedLegacy.Types.Container;

namespace NakedLegacy.Reflector.Component;

public static class ThreadLocals {
    private static IServiceProvider serviceProvider;

    public static void Initialize(IServiceProvider sp) => serviceProvider = sp;

    private static IContainer GetContainer() {
        var scopeSp = serviceProvider.CreateScope().ServiceProvider;
        var framework = scopeSp.GetService<INakedFramework>();
        return new Container(framework);
    }

    public static IContainer Container => GetContainer();
}