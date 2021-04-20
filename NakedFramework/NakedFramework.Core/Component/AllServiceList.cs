using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Component;

namespace NakedFramework.Core.Component {
    public class AllServiceList : IAllServiceList {
        public AllServiceList(IEnumerable<IServiceList> services) {
            foreach (var serviceList in services) {
                Services = Services.Union(serviceList.Services).ToArray();
            }
        }

        public Type[] Services { get; } = Array.Empty<Type>();
    }
}