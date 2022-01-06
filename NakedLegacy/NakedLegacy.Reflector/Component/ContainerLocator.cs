using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedLegacy.Types.Container;

namespace NakedLegacy.Reflector.Component;

public static class ContainerLocator {
    private static IServiceProvider serviceProvider;

    public static void Initialize(IServiceProvider sp) => serviceProvider = sp;

    public static IContainer GetContainer() {
        var scopeSp = serviceProvider.CreateScope().ServiceProvider;
        var framework = scopeSp.GetService<INakedFramework>();
        return new Container(framework);
    }
}