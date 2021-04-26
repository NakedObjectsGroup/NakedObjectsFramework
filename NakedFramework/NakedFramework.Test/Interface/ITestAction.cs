// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedFramework.Xat.Interface {
    public interface ITestAction {
        string Name { get; }
        string SubMenu { get; }
        string LastMessage { get; }
        ITestParameter[] Parameters { get; }
        bool MatchParameters(Type[] typestoMatch);
        ITestObject InvokeReturnObject(params object[] parameters);
        ITestCollection InvokeReturnCollection(params object[] parameters);
        void Invoke(params object[] parameters);
        ITestCollection InvokeReturnPagedCollection(int page, params object[] parameters);
        ITestAction AssertIsDisabled();
        ITestAction AssertIsEnabled();
        ITestAction AssertIsInvalidWithParms(params object[] parameters);
        ITestAction AssertIsValidWithParms(params object[] parameters);
        ITestAction AssertIsVisible();
        ITestAction AssertIsInvisible();
        ITestAction AssertIsDescribedAs(string expected);
        ITestAction AssertLastMessageIs(string message);
        ITestAction AssertLastMessageContains(string message);

        /// <summary>
        ///     Test how the action will be rendered to the user.
        /// </summary>
        ITestAction AssertHasFriendlyName(string friendlyName);
    }
}