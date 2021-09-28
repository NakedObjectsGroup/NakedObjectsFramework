// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.application.valueholder.BusinessValueHolder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.nakedobjects.application.valueholder
{
  [JavaInterfaces("1;org/nakedobjects/application/TitledObject;")]
  public abstract class BusinessValueHolder : TitledObject
  {
    public virtual bool userChangeable() => true;

    public abstract bool isEmpty();

    public abstract bool isSameAs(BusinessValueHolder @object);

    public virtual string titleString() => this.title().ToString();

    public abstract Title title();

    public override string ToString() => this.titleString();

    public virtual object getValue() => (object) this;

    [JavaThrownExceptions("1;org/nakedobjects/application/ValueParseException;")]
    public abstract void parseUserEntry(string text);

    public abstract void restoreFromEncodedString(string data);

    public abstract string asEncodedString();

    public abstract void copyObject(BusinessValueHolder @object);

    public abstract void clear();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      BusinessValueHolder businessValueHolder = this;
      ObjectImpl.clone((object) businessValueHolder);
      return ((object) businessValueHolder).MemberwiseClone();
    }
  }
}
