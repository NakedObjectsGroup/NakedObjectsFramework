// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Framework;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public class SpecFactory {
        private INakedObjectsFramework framework;
        private bool isInitialised;
        private ILogger<SpecFactory> logger;
        private ILoggerFactory loggerFactory;

        public void Initialize(INakedObjectsFramework newFramework, ILoggerFactory newLoggerFactory, ILogger<SpecFactory> newLogger) {
            framework = newFramework ?? throw new InitialisationException($"{nameof(newFramework)} is null");
            loggerFactory = newLoggerFactory ?? throw new InitialisationException($"{nameof(newLoggerFactory)} is null");
            logger = newLogger ?? throw new InitialisationException($"{nameof(newLogger)} is null");
            isInitialised = true;
        }

        private void CheckInitialised() {
            if (isInitialised) { return; }

            throw new InitialisationException($"{nameof(SpecFactory)} not initialised");
        }

        public IActionParameterSpec CreateParameter(IActionParameterSpecImmutable parameterSpecImmutable, IActionSpec actionSpec, int index) {
            CheckInitialised();
            var specification = parameterSpecImmutable.Specification;
            return specification switch {
                _ when specification.IsParseable => new ActionParseableParameterSpec(index, actionSpec, parameterSpecImmutable, framework),
                _ when specification.IsObject => new OneToOneActionParameter(index, actionSpec, parameterSpecImmutable, framework),
                _ when specification.IsCollection => new OneToManyActionParameter(index, actionSpec, parameterSpecImmutable, framework),
                _ => throw new UnknownTypeException(logger.LogAndReturn($"{specification}"))
            };
        }

        private IAssociationSpec CreateAssociation(IAssociationSpecImmutable specImmutable) {
            CheckInitialised();
            return specImmutable switch {
                IOneToOneAssociationSpecImmutable oneToOneAssociationSpecImmutable => new OneToOneAssociationSpec(oneToOneAssociationSpecImmutable, framework),
                IOneToManyAssociationSpecImmutable oneToManyAssociationSpecImmutable => new OneToManyAssociationSpec(oneToManyAssociationSpecImmutable, framework),
                _ => throw new ReflectionException(logger.LogAndReturn($"Unknown spec type: {specImmutable}"))
            };
        }

        public IActionSpec[] CreateActionSpecs(IList<IActionSpecImmutable> specImmutables) => specImmutables.Select(CreateActionSpec).ToArray();

        public IAssociationSpec[] CreateAssociationSpecs(IList<IAssociationSpecImmutable> specImmutables) => specImmutables.Select(CreateAssociationSpec).ToArray();

        public IActionSpec CreateActionSpec(IActionSpecImmutable specImmutable) {
            CheckInitialised();
            return new ActionSpec( 
                framework,
                this,
                specImmutable,
                loggerFactory,
                loggerFactory.CreateLogger<ActionSpec>());
        }

        public IAssociationSpec CreateAssociationSpec(IAssociationSpecImmutable specImmutable) {
            CheckInitialised();
            return CreateAssociation(specImmutable);
        }

        public ITypeSpec CreateTypeSpec(ITypeSpecImmutable specImmutable) =>
            specImmutable switch {
                IObjectSpecImmutable osi => CreateObjectSpec(osi),
                IServiceSpecImmutable ssi => CreateServiceSpec(ssi),
                _ => throw new InitialisationException(logger.LogAndReturn($"Unexpected Spec Type {specImmutable.Type}"))
            };

        private IServiceSpec CreateServiceSpec(IServiceSpecImmutable specImmutable) {
            CheckInitialised();
            return new ServiceSpec(this,  specImmutable, framework);
        }

        private IObjectSpec CreateObjectSpec(IObjectSpecImmutable specImmutable) {
            CheckInitialised();
            return new ObjectSpec(this,
                                  specImmutable,
                                  framework, 
                                  loggerFactory.CreateLogger<ObjectSpec>());
        }
    }
}