

using OpenQA.Selenium;
using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ObjectView : ActionResult
    {
        public ObjectView(IWebElement element, Helper helper, Pane pane = Pane.Single) : base(element, helper, pane) { }

        public override ObjectView AssertTitleIs(string title) => throw new NotImplementedException();

        //Type is the unqualified type name (held in a span but not displayed in default client)
        public ObjectView AssertHasType(string typeName) => throw new NotImplementedException();
        //Properties, here, includes any collections. The list of names should be specified in display order
        public ObjectView AssertPropertiesAre(params string[] propertyNames) => throw new NotImplementedException();

        public Property GetProperty(string propertyName) => throw new NotImplementedException();

        public ObjectCollection GetCollection(string collectionName) => throw new NotImplementedException();

        public ObjectView DragTitleAndDropOnto(ReferenceInputField field) => throw new NotImplementedException();

        public string GetTitle() => throw new NotImplementedException();

        //Works only when there is already an open dialog in view - otherwise open one through action menu
        public Dialog GetOpenedDialog() => throw new NotImplementedException();
    }
}
