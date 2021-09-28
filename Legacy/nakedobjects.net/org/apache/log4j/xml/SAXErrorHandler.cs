// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.SAXErrorHandler
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.helpers;
using org.xml.sax;

namespace org.apache.log4j.xml
{
  [JavaInterfaces("1;org/xml/sax/ErrorHandler;")]
  public class SAXErrorHandler : ErrorHandler
  {
    public virtual void error(SAXParseException ex)
    {
      LogLog.error(new StringBuffer().append("Parsing error on line ").append(ex.getLineNumber()).append(" and column ").append(ex.getColumnNumber()).ToString());
      LogLog.error(ex.getMessage(), (Throwable) ex.getException());
    }

    public virtual void fatalError(SAXParseException ex) => this.error(ex);

    public virtual void warning(SAXParseException ex)
    {
      LogLog.warn(new StringBuffer().append("Parsing error on line ").append(ex.getLineNumber()).append(" and column ").append(ex.getColumnNumber()).ToString());
      LogLog.warn(ex.getMessage(), (Throwable) ex.getException());
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SAXErrorHandler saxErrorHandler = this;
      ObjectImpl.clone((object) saxErrorHandler);
      return ((object) saxErrorHandler).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
