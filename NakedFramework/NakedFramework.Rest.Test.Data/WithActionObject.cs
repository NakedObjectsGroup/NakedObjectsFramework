// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace RestfulObjects.Test.Data {
    public class WithActionObject : WithAction {
        [Key]
        [Title]
        [ConcurrencyCheck]
        [DefaultValue(0)]
        public virtual int Id { get; set; }

        public virtual MostSimple AnOverloadedAction0() => Container.Instances<MostSimple>().Single(x => x.Id == 1);

        public virtual MostSimple AnOverloadedAction1(string parm) => Container.Instances<MostSimple>().Single(x => x.Id == 1);

        [Edit]
        public WithActionObject AnActionWithEditAnnotation(int id) => new() {Id = id};

        [DisplayAsProperty]
        public MostSimple AnObjectActionWithDisplayAsPropertyAnnotation() => Container.Instances<MostSimple>().Single(x => x.Id == 1);

        [DisplayAsProperty]
        public IList<MostSimple> AnObjectActionWithDisplayAsPropertyAnnotation1() => Container.Instances<MostSimple>().Take(1).ToList();
    }
}