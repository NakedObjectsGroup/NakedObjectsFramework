// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Reflect.Peer;
using NakedObjects.Reflect.Spec;
using NakedObjects.Reflect.DotNet.Reflect.Strategy;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect {
    public class DotNetReflectorInstaller : IReflectorInstaller {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DotNetReflectorInstaller));
        private readonly List<IReflectorEnhancementInstaller> enhancements = new List<IReflectorEnhancementInstaller>();

        #region IReflectorInstaller Members

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

        public IReflector CreateReflector() {
            throw new System.NotImplementedException();
        }

        public void AddEnhancement(IReflectorEnhancementInstaller enhancement) {
            enhancements.Add(enhancement);
        }

        public string Name {
            get { return "dotnet"; }
        }

        #endregion

        private void AddEnhancement(FacetDecoratorSet facetDecoratorSet, IReflector reflector) {
            foreach (IReflectionDecoratorInstaller installer in enhancements.Where(x => x is IReflectionDecoratorInstaller)) {
                installer.CreateDecorators(reflector).ForEach(facetDecoratorSet.Add);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}