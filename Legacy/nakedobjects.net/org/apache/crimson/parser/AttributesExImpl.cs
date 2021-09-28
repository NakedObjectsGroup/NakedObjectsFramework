// Decompiled with JetBrains decompiler
// Type: org.apache.crimson.parser.AttributesExImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using java.util;
using org.xml.sax.helpers;

namespace org.apache.crimson.parser
{
  [JavaFlags(48)]
  [JavaInterfaces("1;org/apache/crimson/parser/AttributesEx;")]
  public sealed class AttributesExImpl : AttributesImpl, AttributesEx
  {
    private Vector specified;
    private Vector defaults;
    private string idAttributeName;

    [JavaFlags(0)]
    public AttributesExImpl()
    {
      this.specified = new Vector();
      this.defaults = new Vector();
    }

    public override void clear()
    {
      base.clear();
      this.specified.removeAllElements();
      this.defaults.removeAllElements();
      this.idAttributeName = (string) null;
    }

    public virtual void addAttribute(
      string uri,
      string localName,
      string qName,
      string type,
      string value,
      string defaultValue,
      bool isSpecified)
    {
      this.addAttribute(uri, localName, qName, type, value);
      this.defaults.addElement((object) defaultValue);
      this.specified.addElement(!isSpecified ? (object) null : (object) Boolean.TRUE);
    }

    public virtual bool isSpecified(int i) => this.specified.elementAt(i) == Boolean.TRUE;

    public virtual string getDefault(int i)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string getIdAttributeName() => this.idAttributeName;

    [JavaFlags(0)]
    public virtual void setIdAttributeName(string name) => this.idAttributeName = name;
  }
}
