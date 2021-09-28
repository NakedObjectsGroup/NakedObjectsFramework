// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.URLString
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.net;

namespace org.nakedobjects.application.valueholder
{
  public class URLString : BusinessValueHolder
  {
    private string urlString;

    public URLString()
      : this("")
    {
    }

    public URLString(string urlString) => this.urlString = urlString;

    public URLString(URLString urlString) => this.urlString = StringImpl.createString(urlString.ToString());

    public override void clear() => this.urlString = (string) null;

    public override void copyObject(BusinessValueHolder @object) => this.urlString = @object is URLString ? ((URLString) @object).urlString : throw new IllegalArgumentException("Can only copy the value of  a URLString object");

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is URLString))
        return false;
      URLString urlString = (URLString) obj;
      return urlString.isEmpty() && this.isEmpty() || StringImpl.equals(urlString.urlString, (object) this.urlString);
    }

    public virtual string getObjectHelpText() => "A URLString object.";

    public override bool isEmpty() => this.urlString == null;

    public override bool isSameAs(BusinessValueHolder @object) => @object is URLString && StringImpl.equals(((URLString) @object).urlString, (object) this.urlString);

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public override void parseUserEntry(string urlString)
    {
      try
      {
        URL url = new URL(urlString);
        this.urlString = urlString;
      }
      catch (MalformedURLException ex)
      {
        throw new ValueParseException("Invalid URL", (Throwable) ex);
      }
    }

    public virtual void reset() => this.urlString = "";

    public override void restoreFromEncodedString(string data)
    {
      if (data == null)
        this.clear();
      else
        this.urlString = data;
    }

    public override string asEncodedString() => this.isEmpty() ? (string) null : this.urlString;

    public virtual void setValue(string urlString) => this.urlString = urlString;

    public virtual void setValue(URLString urlString) => this.urlString = urlString.urlString;

    public virtual string stringValue() => this.urlString;

    public override Title title() => new Title(this.urlString);
  }
}
