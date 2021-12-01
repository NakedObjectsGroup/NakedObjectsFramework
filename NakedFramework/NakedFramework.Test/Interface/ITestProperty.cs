// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Test.Interface; 

public interface ITestProperty {
    string Name { get; }
    string Id { get; }
    string Title { get; }
    ITestNaked Content { get; }
    ITestObject ContentAsObject { get; }
    ITestCollection ContentAsCollection { get; }
    ITestNaked GetDefault();
    ITestNaked[] GetChoices();
    ITestNaked[] GetCompletions(string autoCompleteParm);
    ITestProperty SetObject(ITestObject testObject);

    /// <summary>
    ///     Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
    ///     option that each object field offers by default.
    /// </summary>
    ITestProperty ClearObject();

    ITestProperty SetValue(string textEntry);
    ITestProperty ClearValue();
    ITestProperty AssertFieldEntryInvalid(string text);
    ITestProperty AssertFieldEntryIsValid(string text);
    ITestProperty AssertSetObjectInvalid(ITestObject testObject);
    ITestProperty AssertSetObjectIsValid(ITestObject testObject);
    ITestProperty AssertIsInvisible();
    ITestProperty AssertIsVisible();
    ITestProperty AssertIsMandatory();
    ITestProperty AssertIsDescribedAs(string expected);
    ITestProperty AssertIsModifiable();
    ITestProperty AssertIsUnmodifiable();
    ITestProperty AssertIsNotEmpty();
    ITestProperty AssertIsEmpty();
    ITestProperty AssertValueIsEqual(string expected);
    ITestProperty AssertTitleIsEqual(string expected);
    ITestProperty AssertObjectIsEqual(ITestNaked expected);
    ITestProperty AssertIsValidToSave();
    ITestProperty AssertLastMessageIs(string message);
}