// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Persist;

namespace NakedFramework.Persistor.EF6.Component;

public class EF6DestroyObjectCommand : IDestroyObjectCommand {
    private readonly EF6LocalContext context;
    private readonly INakedObjectAdapter nakedObjectAdapter;

    public EF6DestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter, EF6LocalContext context) {
        this.context = context;
        this.nakedObjectAdapter = nakedObjectAdapter;
    }

    public override string ToString() => $"EF6 DestroyObjectCommand [object={nakedObjectAdapter}]";

    #region IDestroyObjectCommand Members

    public void Execute() {
        context.WrappedObjectContext.DeleteObject(nakedObjectAdapter.Object);
        context.DeletedNakedObjects.Add(nakedObjectAdapter);
    }

    public INakedObjectAdapter OnObject() => nakedObjectAdapter;

    #endregion
}