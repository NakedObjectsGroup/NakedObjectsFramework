// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.LocatorImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("1;org/xml/sax/Locator;")]
  public class LocatorImpl : Locator
  {
    private string publicId;
    private string systemId;
    private int lineNumber;
    private int columnNumber;

    public LocatorImpl()
    {
    }

    public LocatorImpl(Locator locator)
    {
      this.setPublicId(locator.getPublicId());
      this.setSystemId(locator.getSystemId());
      this.setLineNumber(locator.getLineNumber());
      this.setColumnNumber(locator.getColumnNumber());
    }

    public virtual string getPublicId() => this.publicId;

    public virtual string getSystemId() => this.systemId;

    public virtual int getLineNumber() => this.lineNumber;

    public virtual int getColumnNumber() => this.columnNumber;

    public virtual void setPublicId(string publicId) => this.publicId = publicId;

    public virtual void setSystemId(string systemId) => this.systemId = systemId;

    public virtual void setLineNumber(int lineNumber) => this.lineNumber = lineNumber;

    public virtual void setColumnNumber(int columnNumber) => this.columnNumber = columnNumber;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      LocatorImpl locatorImpl = this;
      ObjectImpl.clone((object) locatorImpl);
      return ((object) locatorImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
