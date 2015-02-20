// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Xat {
    public interface ITestHasActions : ITestNaked {
        ITestAction[] Actions { get; }
        ITestAction GetAction(string name);

        /// <summary>
        /// It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetAction(string name, params Type[] parameterTypes);

        ITestAction GetAction(string name, string subMenu);

        /// <summary>
        /// It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetAction(string name, string subMenu, params Type[] parameterTypes);

        string GetObjectActionOrder();

        /// <summary>
        /// Test action order against string of form: "Action1, Action2"
        /// or "Action1, (SubMenu1:Action1, Action2)"
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The current object</returns>
        ITestHasActions AssertActionOrderIs(string order);

        ITestMenu GetMenu();
    }
}