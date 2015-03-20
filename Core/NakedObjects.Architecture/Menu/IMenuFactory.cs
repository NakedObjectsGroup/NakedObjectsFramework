// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Menu;
using System;

namespace NakedObjects.Architecture.Menu {
    /// <summary>
    /// Injected service that provides implementations of IMenu.
    /// </summary>
    public interface IMenuFactory {
        //Creates a new menu with the Type specified. If the optional name
        //parameter is not specified, then the menu takes its name from the type.
        IMenu NewMenu<T>(bool addAllActions = false, string name = null);

        //Creates a new menu with the Type specified. If the optional name
        //parameter is not specified, then the menu takes its name from the type.
        IMenu NewMenu(Type type, bool addAllActions = false, string name = null);
    }
}