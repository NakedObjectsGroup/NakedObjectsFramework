// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.jaxp.DefaultValidationErrorHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using org.xml.sax;
using org.xml.sax.helpers;
using System.ComponentModel;

namespace org.apache.crimson.jaxp
{
  [JavaFlags(32)]
  public class DefaultValidationErrorHandler : DefaultHandler
  {
    private static int ERROR_COUNT_LIMIT;
    private int errorCount;

    [JavaThrownExceptions("1;org/xml/sax/SAXException;")]
    public override void error(SAXParseException e)
    {
      if (this.errorCount >= DefaultValidationErrorHandler.ERROR_COUNT_LIMIT)
        return;
      if (this.errorCount == 0)
      {
        ((PrintStream) java.lang.System.err).println("Warning: validation was turned on but an org.xml.sax.ErrorHandler was not");
        ((PrintStream) java.lang.System.err).println("set, which is probably not what is desired.  Parser will use a default");
        ((PrintStream) java.lang.System.err).println(new StringBuffer().append("ErrorHandler to print the first ").append(DefaultValidationErrorHandler.ERROR_COUNT_LIMIT).append(" errors.  Please call").ToString());
        ((PrintStream) java.lang.System.err).println("the 'setErrorHandler' MethodInfo to fix this.");
      }
      string str1 = e.getSystemId() ?? "null";
      string str2 = new StringBuffer().append("Error: URI=").append(str1).append(" Line=").append(e.getLineNumber()).append(": ").append(e.getMessage()).ToString();
      ((PrintStream) java.lang.System.err).println(str2);
      ++this.errorCount;
    }

    [JavaFlags(0)]
    public DefaultValidationErrorHandler() => this.errorCount = 0;

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static DefaultValidationErrorHandler()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
