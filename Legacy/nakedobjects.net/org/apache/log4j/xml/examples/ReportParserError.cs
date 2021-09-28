// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.examples.ReportParserError
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.apache.log4j.helpers;
using org.xml.sax;

namespace org.apache.log4j.xml.examples
{
  [JavaInterfaces("1;org/xml/sax/ErrorHandler;")]
  public class ReportParserError : ErrorHandler
  {
    [JavaFlags(0)]
    public virtual void report(string msg, SAXParseException e) => LogLog.error(new StringBuffer().append(msg).append(e.getMessage()).append("\n\tat line=").append(e.getLineNumber()).append(" col=").append(e.getColumnNumber()).append(" of ").append("SystemId=\"").append(e.getSystemId()).append("\" PublicID = \"").append(e.getPublicId()).append('"').ToString());

    public virtual void warning(SAXParseException e) => this.report("WARNING: ", e);

    public virtual void error(SAXParseException e) => this.report("ERROR: ", e);

    public virtual void fatalError(SAXParseException e) => this.report("FATAL: ", e);

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      ReportParserError reportParserError = this;
      ObjectImpl.clone((object) reportParserError);
      return ((object) reportParserError).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
