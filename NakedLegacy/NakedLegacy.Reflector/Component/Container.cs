using System;
using System.Collections;
using NakedFramework.Architecture.Framework;
using NakedLegacy.Types.Container;

namespace NakedLegacy.Reflector.Component;

public class Container : IContainer {
    private readonly INakedFramework framework;

    public Container(INakedFramework framework) => this.framework = framework;

    public IEnumerable AllInstances(Type ofType) => framework.Persistor.Instances(ofType);
}