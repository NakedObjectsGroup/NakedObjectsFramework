// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.ModelBuilding.Component {
    public class ModelBuilder : IModelBuilder {
        private readonly IMetamodelBuilder initialMetamodel;
        private readonly IModelIntegrator integrator;
        private readonly IEnumerable<IReflector> reflectors;

        public ModelBuilder(IEnumerable<IReflector> reflectors, IModelIntegrator integrator, IMetamodelBuilder initialMetamodel) {
            this.reflectors = reflectors;
            this.integrator = integrator;
            this.initialMetamodel = initialMetamodel;
        }

        public void Build() {
            IImmutableDictionary<string, ITypeSpecBuilder> specDictionary = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

            // todo make unordered 

            specDictionary = reflectors.OrderBy(r => r.Order).Aggregate(specDictionary, (current, reflector) => reflector.Reflect(current));
            Validate(specDictionary);

            specDictionary.ForEach(i => initialMetamodel.Add(i.Value.Type, i.Value));

            integrator.Integrate();
        }

        private static void Validate(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
            var unIntrospectedSpecs = specDictionary.Values.Where(v => v.IsPlaceHolder || v.IsPendingIntrospection);

            if (unIntrospectedSpecs.Any()) {
                var names = unIntrospectedSpecs.Aggregate("", (s, sp) => $"{s}{sp.Type},");
                throw new ReflectionException($"Unintrospected specs: {names}");
            }
        }
    }
}