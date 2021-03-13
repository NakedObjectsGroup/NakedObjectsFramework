using System;

namespace NakedFrameworkClient.TestFramework
{
    public class Dialog : Element
    {
        public Dialog AssertHasFields(params string[] fieldNames) => throw new NotImplementedException();

        public T GetField<T>(string fieldName) where T : InputField, new() => throw new NotImplementedException();

        public Dialog AssertOKIsEnabled() => throw new NotImplementedException();

        public  Dialog AssertOKIsDisabled(string withToolTip) => throw new NotImplementedException();

        public ObjectView ClickOKToViewObject(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public ListView ClickOKToViewList(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

    }
}