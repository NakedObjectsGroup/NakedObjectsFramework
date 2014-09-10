// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace MvcTestApp.Tests.Helpers {
    [PresentationHint("classHint")]
    [Bounded]
    public class HintTestClass {
        private IList<HintTestClass> hintCollection = new List<HintTestClass>();

        [Hidden, Key]
        public int Id { get; set; }

        [Title]
        [PresentationHint("propertyHint")]
        public string TestString { get; set; }

        [PresentationHint("collectionHint")]
        public IList<HintTestClass> HintCollection {
            get { return hintCollection; }
            set { hintCollection = value; }
        }

        [PresentationHint("actionHint")]
        public void SimpleAction() {}

        public void ActionWithParms([PresentationHint("parmHint1")] int parm1, [PresentationHint("parmHint2")] int parm2) {}
    }
}