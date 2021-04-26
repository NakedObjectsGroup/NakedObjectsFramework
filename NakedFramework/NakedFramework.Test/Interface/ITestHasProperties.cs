// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Xat.Interface {
    public interface ITestHasProperties {
        ITestProperty[] Properties { get; }
        ITestObject Save();
        ITestObject Refresh();

        /// <summary>
        ///     Given the 'friendly name' of a property (as would be rendered to the user)
        ///     returns the matching test property.
        /// </summary>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        ITestProperty GetPropertyByName(string friendlyName);

        /// <summary>
        ///     Given the Id (the actual member name in code), returns the matching test property (if any).
        ///     Designed to work with C# 6 nameof() operator.
        /// </summary>
        ITestProperty GetPropertyById(string id);

        ITestObject AssertCanBeSaved();
        ITestObject AssertCannotBeSaved();
        ITestObject AssertIsTransient();
        ITestObject AssertIsPersistent();
        string GetPropertyOrder();

        /// <summary>
        ///     Test action order against string of form: "Property1, Property2"
        /// </summary>
        /// <param name="order"></param>
        /// <returns>The current object</returns>
        ITestHasProperties AssertPropertyOrderIs(string order);
    }
}