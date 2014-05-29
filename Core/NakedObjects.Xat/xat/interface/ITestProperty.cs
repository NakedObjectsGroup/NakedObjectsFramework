// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
namespace NakedObjects.Xat {
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
        ITestProperty RemoveFromCollection(ITestObject testObject);
        string LastMessage { get; }

        /// <summary>
        /// Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
        /// option that each object field offers by default.
        /// </summary>
        ITestProperty ClearObject();

        ITestProperty SetValue(string textEntry);
        ITestProperty ClearValue();
        ITestProperty TestField(string setValue, string expected);
        ITestProperty TestField(ITestObject expected);
        ITestProperty AssertCannotParse(string text);
        ITestProperty AssertFieldEntryInvalid(string text);
        ITestProperty AssertFieldEntryIsValid(string text);

        ITestProperty AssertSetObjectInvalid(ITestObject testObject);
        ITestProperty AssertSetObjectIsValid(ITestObject testObject);

        ITestProperty AssertIsInvisible();
        ITestProperty AssertIsVisible();
        ITestProperty AssertIsMandatory();
        ITestProperty AssertIsOptional();
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
        ITestProperty AssertLastMessageContains(string message);
    }
}