using System;

namespace NakedFrameworkClient.TestFramework
{
    public class ActionWithoutDialog 
        {
        public  ActionWithoutDialog AssertIsEnabled() => throw new NotImplementedException();

        public ObjectView ClickToViewObject(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

        public ListView ClickToViewList(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();

    }
}
