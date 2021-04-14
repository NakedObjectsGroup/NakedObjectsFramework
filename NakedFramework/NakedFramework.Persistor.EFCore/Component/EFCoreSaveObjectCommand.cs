// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Persist;

namespace NakedFramework.Persistor.EFCore.Component {
    public class EFCoreSaveObjectCommand : ISaveObjectCommand {
        private readonly EFCoreLocalContext context;
        private readonly INakedObjectAdapter nakedObjectAdapter;

        public EFCoreSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter, EFCoreLocalContext context)
        {
            this.context = context;
            this.nakedObjectAdapter = nakedObjectAdapter;
        }

        public override string ToString() => $"SaveObjectCommand [object={nakedObjectAdapter}]";

        public void Execute() => context.CurrentUpdateRootObjectAdapter = nakedObjectAdapter;

        public INakedObjectAdapter OnObject() => nakedObjectAdapter;

    }
}