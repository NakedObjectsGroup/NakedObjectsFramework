// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Util;
using NakedObjects.Reflect.Spec;

namespace NakedObjects.Reflect.Installer {
    public class DotNetReflectorInstaller : IReflectorInstaller {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DotNetReflectorInstaller));
        private readonly List<IReflectorEnhancementInstaller> enhancements = new List<IReflectorEnhancementInstaller>();

        public bool OptionalByDefault { get; set; }

        //public IReflector CreateReflector() {

        //    var facetDecoratorSet = new FacetDecoratorSet();

        //    var reflector = new DotNetReflector(new DefaultClassStrategy(), new FacetFactorySet(), facetDecoratorSet, TODO);

        //    if (enhancements.Count == 0) {
        //        Log.Debug("No enhancements set up");
        //    }
        //    else {
        //        AddEnhancement(facetDecoratorSet, reflector);
        //    }

        //    if (OptionalByDefault) {
        //      //  ((FacetFactorySetImpl)reflector.FacetFactorySet).ReplaceAndRegisterFactory(typeof(MandatoryDefaultFacetFactory), new OptionalDefaultFacetFactory(NakedObjectsContext.Reflector));
        //    }

        //    return reflector;

        //}

        #region IReflectorInstaller Members

        public IReflector CreateReflector() {
            throw new NotImplementedException();
        }

        public void AddEnhancement(IReflectorEnhancementInstaller enhancement) {
            enhancements.Add(enhancement);
        }

        public string Name {
            get { return "dotnet"; }
        }

        #endregion

        private void AddEnhancement(FacetDecoratorSet facetDecoratorSet, IReflector reflector) {

        
            //foreach (IReflectionDecoratorInstaller installer in enhancements.Where(x => x is IReflectionDecoratorInstaller)) {
            //    installer.CreateDecorators(reflector).ForEach(decorator => {
            //        foreach (Type type in decorator.ForFacetTypes) {
            //            if (!facetDecoratorSet.facetDecorators.ContainsKey(type)) {
            //                facetDecoratorSet.facetDecorators[type] = new List<IFacetDecorator>();
            //            }
            //            facetDecoratorSet.facetDecorators[type].Add(decorator);
            //        }
            //    });
            //}
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}