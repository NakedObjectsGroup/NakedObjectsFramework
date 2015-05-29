// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Surface.Interface;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Implementation {
    public class InternalFormatLinkOidFactory : ILinkOidFactory {
        private readonly ILifecycleManager lifecycleManager;

        public InternalFormatLinkOidFactory(ILifecycleManager lifecycleManager) {
            this.lifecycleManager = lifecycleManager;
        }

        public ILinkObjectId GetLinkOid(params string[] id) {
            if (id.Count() != 1) {
                throw new ObjectResourceNotFoundNOSException(id.Aggregate((s, t) => s + " " + t));
            }
            if (string.IsNullOrEmpty(id.First())) {
                return null;
            }

            return new MVCObjectId(id.First());
        }

        private string Encode(IEncodedToStrings encoder) {
            return encoder.ToShortEncodedStrings().Aggregate((a, b) => a + ";" + b);
        }
     
        // todo is this best place for this
        //private string GetObjectId(INakedObjectAdapter nakedObject) {
        //    if (nakedObject.Spec.IsViewModel) {
        //        // todo this always repopulates oid now - see core - look into optimizing
        //        framework.LifecycleManager.PopulateViewModelKeys(nakedObject);
        //    }
        //    else if (nakedObject.Oid == null) {
        //        return "";
        //    }

        //    return GetObjectId(nakedObject.Oid);
        //}

        private string GetObjectId(IOidSurface oid) {
            return Encode(((IEncodedToStrings)oid.Value));
        }

        public ILinkObjectId GetLinkOid(INakedObjectSurface nakedObject) {

            if (nakedObject.IsViewModel) {
                var vm = ((NakedObjectWrapper) nakedObject).WrappedNakedObject;
                lifecycleManager.PopulateViewModelKeys(vm);
            }

            var oid = nakedObject.Oid;
            var id = GetObjectId(oid);
            return GetLinkOid(id);
        }
    }
}