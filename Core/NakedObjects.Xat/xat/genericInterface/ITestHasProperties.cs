namespace NakedObjects.Xat.Generic {
    public interface ITestHasProperties<T> {
        ITestProperty[] Properties { get; }
        ITestObject<T> Save();
        ITestProperty GetPropertyByName(string name);
        ITestProperty GetPropertyById(string id);
        ITestObject<T> AssertCanBeSaved();
        ITestObject<T> AssertCannotBeSaved();
        ITestObject<T> AssertIsTransient();
        ITestObject<T> AssertIsPersistent();
    }
}