using NakedFramework.Core.Error;

namespace NakedFramework.RATL.Classic.NonDocumenting; 

internal class TestParameterObject {
    private readonly object domainObject;

    public TestParameterObject(object domainObject) => this.domainObject = domainObject;

    #region ITestValue Members

    public string Title =>throw new NotImplementedException();// NakedObject.TitleString();

    //public INakedObject NakedObject {
    //    get => PersistorUtils.CreateAdapter(domainObject);
    //    set => throw new UnexpectedCallException();
    //}

    #endregion
}

