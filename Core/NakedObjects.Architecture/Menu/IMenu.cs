// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Menu {
    //IMenu is to IMenuImmutable, as IObjectSpec is to IObjectSpecImmutable -  the runtime equivalent with injected services
    public interface IMenu : IMenuImmutable {
        //Adds specified action as the next menu item
        //Returns this menu (for fluent programming)
        IMenu AddActionFrom<TObject>(string actionName, string renamedTo = null);

        //Adds all actions from the service not previously added individually,
        //in the order they are specified in the service.
        //Returns this menu (for fluent programming)
        IMenu AddAllRemainingActionsFrom<TService>();

        //Returns the new menu, which will already have been added to the hosting menu
        IMenu CreateSubMenu(string subMenuName);
    }
}