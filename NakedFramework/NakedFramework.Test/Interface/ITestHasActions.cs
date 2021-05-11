// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Test.Interface {
    public interface ITestHasActions : ITestNaked {
        ITestAction[] Actions { get; }

        /// <summary>
        ///     The friendlyName means the name as would be presented to the user, including
        ///     spaces and capitalization. (See also: GetActionFor)
        /// </summary>
        ITestAction GetAction(string friendlyName);

        /// <summary>
        ///     Equivalent to GetAction but using the Id (methodName in code), rather than the friendly name of the action.
        ///     Designed to work with the nameof() operator in C# 6.
        /// </summary>
        ITestAction GetActionById(string methodName);

        /// <summary>
        ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetAction(string friendlyName, params Type[] parameterTypes);

        /// <summary>
        ///     Equivalent to GetAction but using the Id (methodName in code), rather than the friendly name of the action.
        ///     Designed to work with the nameof() operator in C# 6.
        ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetActionById(string methodName, params Type[] parameterTypes);

        ITestAction GetAction(string friendlyName, string subMenu);

        /// <summary>
        ///     Equivalent to GetAction but using the Id (methodName in code), rather than the friendly name of the action.
        ///     Designed to work with the nameof() operator in C# 6.
        /// </summary>
        ITestAction GetActionById(string methodName, string subMenu);

        /// <summary>
        ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetAction(string friendlyName, string subMenu, params Type[] parameterTypes);

        /// <summary>
        ///     Equivalent to GetAction but using the Id (methodName in code), rather than the friendly name of the action.
        ///     Designed to work with the nameof() operator in C# 6.
        ///     It is not necessary to specify the parameter types unless you need to disambiguate overloaded action methods
        /// </summary>
        ITestAction GetActionById(string methodName, string subMenu, params Type[] parameterTypes);

        /// <summary>
        ///     Test action order against string of form: "Action1, Action2"
        ///     or "Action1, (SubMenu1:Action1, Action2)"
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The current object</returns>
        ITestHasActions AssertActionOrderIs(string order);

        ITestMenu GetMenu();
    }
}