using System;
using System.Collections.Generic;

namespace NakedObjects.DependencyInjection.FacetFactory {
    public class FacetFactoryTypesProvider {
        private static List<Type> FacetFactoryTypesList { get; } = new List<Type>();

        public Type[] FacetFactoryTypes => FacetFactoryTypesList.ToArray();

        public static void AddType(Type type) => FacetFactoryTypesList.Add(type);
    }
}