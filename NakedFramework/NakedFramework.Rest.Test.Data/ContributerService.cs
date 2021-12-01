// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace RestfulObjects.Test.Data; 

public class ContributorService {
    public IDomainObjectContainer Container { set; protected get; }

    public virtual MostSimple ANonContributedAction() {
        return Container.Instances<MostSimple>().Single(x => x.Id == 1);
    }

    // do nothing
    public virtual MostSimple ACollectionContributedActionNoParms([ContributedAction("submenu")] IQueryable<MostSimple> ms) => ms.FirstOrDefault();

    public virtual MostSimple ACollectionContributedActionParm([ContributedAction] IQueryable<MostSimple> ms, int id) {
        return ms.SingleOrDefault(i => i.Id == id);
    }
}