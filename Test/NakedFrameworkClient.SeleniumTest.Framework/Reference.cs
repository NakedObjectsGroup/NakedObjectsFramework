using System;

namespace NakedFrameworkClient.SeleniumTestFramework
{
    public class Reference
    {
        public virtual Reference AssertTitleIs(string title) => throw new NotImplementedException();

        public ObjectView Click(MouseClick button = MouseClick.MainButton) => throw new NotImplementedException();
   
        public Reference CopyReferenceToClipboard() => throw new NotImplementedException();
    }
}