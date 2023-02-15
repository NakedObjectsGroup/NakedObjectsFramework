namespace NakedFramework.RATL.Classic.Interface;

public interface ITestHasProperties
{
    ITestProperty[] Properties { get; }
    ITestObject Save();
    ITestObject Refresh();
    ITestProperty GetPropertyByName(string name);
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