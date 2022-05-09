// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this filePath except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Component;

namespace NakedFramework.ModelBuilding.Component;

public class ModelBuilder : IModelBuilder {
    private readonly IMetamodelBuilder initialMetamodel;
    private readonly IModelIntegrator integrator;
    private readonly ILogger<ModelBuilder> logger;
    private readonly IEnumerable<IReflector> reflectors;

    public ModelBuilder(IEnumerable<IReflector> reflectors, IModelIntegrator integrator, IMetamodelBuilder initialMetamodel, ILogger<ModelBuilder> logger) {
        this.reflectors = reflectors;
        this.integrator = integrator;
        this.initialMetamodel = initialMetamodel;
        this.logger = logger;
    }

    public void Build(string filePath = null) {
        IImmutableDictionary<string, ITypeSpecBuilder> specDictionary = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        specDictionary = reflectors.OrderBy(r => r.Order).Aggregate(specDictionary, (current, reflector) => reflector.Reflect(current));
        Validate(specDictionary);
        specDictionary.ForEach(i => initialMetamodel.Add(i.Value.Type, i.Value));
        integrator.Integrate();

        if (filePath is not null) {
            try {
                initialMetamodel.SaveToFile(filePath);
            }
            catch (Exception e) {
                logger.LogError($"Failed to save metamodel to file: {filePath} : {e}");
            }
        }
    }

    public void RestoreFromFile(string filePath) {
        try {
            ISpecificationCache cache = new ImmutableInMemorySpecCache(filePath);
            initialMetamodel.ReplaceCache(cache);
        }
        catch (Exception e) {
            logger.LogError($"Failed to restore metamodel from file: {filePath} : {e}");
            throw;
        }
    }

    private static void Validate(IImmutableDictionary<string, ITypeSpecBuilder> specDictionary) {
        var unIntrospectedSpecs = specDictionary.Values.Where(v => v.IsPlaceHolder || v.IsPendingIntrospection);

        if (unIntrospectedSpecs.Any()) {
            var names = unIntrospectedSpecs.Aggregate("", (s, sp) => $"{s}{sp.Type},");
            throw new ReflectionException($"Unintrospected specs: {names}");
        }
    }
}