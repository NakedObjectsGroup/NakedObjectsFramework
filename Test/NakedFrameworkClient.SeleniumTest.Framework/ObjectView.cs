

using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectView : ActionResult
    {
        //Properties, here, includes any collections. The list of names should be specified in display order
        public ObjectView AssertPropertiesAre(params string[] propertyNames) => throw new NotImplementedException();

        public Property GetProperty(string propertyName) => throw new NotImplementedException();

        public ObjectCollection GetCollection(string collectionName) => throw new NotImplementedException();

        public ObjectView CopyReferenceToClipboard() => throw new NotImplementedException();
    }
}
