// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Menu;
using RestfulObjects.Test.Data;

namespace MvcTestApp {
    public class MyMainMenuDefinition : IMainMenuDefinition {
        #region IMainMenuDefinition Members

        public IMenuBuilder[] MainMenus(IMenuFactory factory) {
            var menu1 = factory.NewMenu<RestDataRepository>(true);
            var menu2 = factory.NewMenu<WithActionService>(true);
            var menu3 = factory.NewMenu<ContributorService>(true);
            var menu4 = factory.NewMenu<TestTypeCodeMapper>(true);


            return new IMenuBuilder[] {menu1, menu2, menu3, menu4};
        }

        #endregion
    }
}