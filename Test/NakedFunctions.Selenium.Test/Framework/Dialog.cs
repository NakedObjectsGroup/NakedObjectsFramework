using System;

namespace NakedFunctions.Selenium.Test.Framework
{
    public class Dialog : Element
    {
        public Dialog AssertOKIsEnabled() => throw new NotImplementedException();

       public  Dialog AssertOKIsDisabled(string withToolTip) => throw new NotImplementedException();

        public T ClickOK<T>() where T : ActionResult, new() => throw new NotImplementedException();

        public T RightClickOK<T>() where T : ActionResult, new() => throw new NotImplementedException();

        public T GetField<T>(string fieldName) where T : InputField, new() => throw new NotImplementedException();
      
    }
}