// Decompiled with JetBrains decompiler
// Type: org.xml.sax.helpers.AttributeListImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using System;

namespace org.xml.sax.helpers
{
  [JavaInterfaces("1;org/xml/sax/AttributeList;")]
  [Obsolete(null, false)]
  public class AttributeListImpl : AttributeList
  {
    [JavaFlags(0)]
    public Vector names;
    [JavaFlags(0)]
    public Vector types;
    [JavaFlags(0)]
    public Vector values;

    public AttributeListImpl()
    {
      this.names = new Vector();
      this.types = new Vector();
      this.values = new Vector();
    }

    public AttributeListImpl(AttributeList atts)
    {
      this.names = new Vector();
      this.types = new Vector();
      this.values = new Vector();
      this.setAttributeList(atts);
    }

    public virtual void setAttributeList(AttributeList atts)
    {
      int length = atts.getLength();
      this.clear();
      for (int i = 0; i < length; ++i)
        this.addAttribute(atts.getName(i), atts.getType(i), atts.getValue(i));
    }

    public virtual void addAttribute(string name, string type, string value)
    {
      this.names.addElement((object) name);
      this.types.addElement((object) type);
      this.values.addElement((object) value);
    }

    public virtual void removeAttribute(string name)
    {
      int num = this.names.indexOf((object) name);
      if (num < 0)
        return;
      this.names.removeElementAt(num);
      this.types.removeElementAt(num);
      this.values.removeElementAt(num);
    }

    public virtual void clear()
    {
      this.names.removeAllElements();
      this.types.removeAllElements();
      this.values.removeAllElements();
    }

    public virtual int getLength() => this.names.size();

    public virtual string getName(int i)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string getType(int i)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string getValue(int i)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual string getType(string name) => this.getType(this.names.indexOf((object) name));

    public virtual string getValue(string name) => this.getValue(this.names.indexOf((object) name));

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AttributeListImpl attributeListImpl = this;
      ObjectImpl.clone((object) attributeListImpl);
      return ((object) attributeListImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
