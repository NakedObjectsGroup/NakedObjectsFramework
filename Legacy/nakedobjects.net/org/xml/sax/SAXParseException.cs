// Decompiled with JetBrains decompiler
// Type: org.xml.sax.SAXParseException
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;

namespace org.xml.sax
{
  public class SAXParseException : SAXException
  {
    private string publicId;
    private string systemId;
    private int lineNumber;
    private int columnNumber;

    public SAXParseException(string message, Locator locator)
      : base(message)
    {
      if (locator != null)
        this.init(locator.getPublicId(), locator.getSystemId(), locator.getLineNumber(), locator.getColumnNumber());
      else
        this.init((string) null, (string) null, -1, -1);
    }

    public SAXParseException(string message, Locator locator, Exception e)
      : base(message, e)
    {
      if (locator != null)
        this.init(locator.getPublicId(), locator.getSystemId(), locator.getLineNumber(), locator.getColumnNumber());
      else
        this.init((string) null, (string) null, -1, -1);
    }

    public SAXParseException(
      string message,
      string publicId,
      string systemId,
      int lineNumber,
      int columnNumber)
      : base(message)
    {
      this.init(publicId, systemId, lineNumber, columnNumber);
    }

    public SAXParseException(
      string message,
      string publicId,
      string systemId,
      int lineNumber,
      int columnNumber,
      Exception e)
      : base(message, e)
    {
      this.init(publicId, systemId, lineNumber, columnNumber);
    }

    private void init(string publicId, string systemId, int lineNumber, int columnNumber)
    {
      this.publicId = publicId;
      this.systemId = systemId;
      this.lineNumber = lineNumber;
      this.columnNumber = columnNumber;
    }

    public virtual string getPublicId() => this.publicId;

    public virtual string getSystemId() => this.systemId;

    public virtual int getLineNumber() => this.lineNumber;

    public virtual int getColumnNumber() => this.columnNumber;
  }
}
