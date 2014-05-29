namespace NakedObjects.Xat.Generic {
    public interface ITestProperty<TOwner, TReturn> : ITestProperty{
       
        ITestObject<TReturn> ContentAsObject { get; }
        ITestCollection<TOwner, TReturn> ContentAsCollection { get; }
       
        ITestProperty<TOwner, TReturn> SetObject(ITestObject<TReturn> testObject);
        ITestProperty<TOwner, TReturn> RemoveFromCollection(ITestObject<TReturn> testObject);

        /// <summary>
        /// Removes an existing object reference from the specified field. Mirrors the 'Remove Reference' menu
        /// option that each object field offers by default.
        /// </summary>
        ITestProperty<TOwner, TReturn> ClearObject();

        ITestProperty<TOwner, TReturn> SetValue(string textEntry);
        ITestProperty<TOwner, TReturn> ClearValue();
        ITestProperty<TOwner, TReturn> TestField(string setValue, string expected);
        ITestProperty<TOwner, TReturn> TestField(ITestObject expected);
        ITestProperty<TOwner, TReturn> AssertCannotParse(string text);
        ITestProperty<TOwner, TReturn> AssertFieldEntryInvalid(string text);
        ITestProperty<TOwner, TReturn> AssertIsInvisible();
        ITestProperty<TOwner, TReturn> AssertIsVisible();
        ITestProperty<TOwner, TReturn> AssertIsMandatory();
        ITestProperty<TOwner, TReturn> AssertIsOptional();
        ITestProperty<TOwner, TReturn> AssertIsDescribedAs(string expected);
        ITestProperty<TOwner, TReturn> AssertIsModifiable();
        ITestProperty<TOwner, TReturn> AssertIsUnmodifiable();
        ITestProperty<TOwner, TReturn> AssertIsNotEmpty();
        ITestProperty<TOwner, TReturn> AssertIsEmpty();
        ITestProperty<TOwner, TReturn> AssertValueIsEqual(string expected);
        ITestProperty<TOwner, TReturn> AssertObjectIsEqual(ITestNaked expected);
        ITestProperty<TOwner, TReturn> AssertIsValidToSave();
    }
}