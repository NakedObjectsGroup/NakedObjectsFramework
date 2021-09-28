// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.value.adapter.AbstractNakedValue
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.value.adapter;

namespace org.nakedobjects.@object.value.adapter
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedValue;")]
  public abstract class AbstractNakedValue : NakedValue
  {
    private NakedObjectSpecification specification;

    public virtual NakedObjectSpecification getSpecification()
    {
      if (this.specification == null)
        this.specification = NakedObjects.getSpecificationLoader().loadSpecification(this.getValueClass());
      return this.specification;
    }

    public abstract string getValueClass();

    public virtual Oid getOid() => (Oid) null;

    public virtual void copyObject(Naked @object)
    {
    }

    public virtual void clearAssociation(OneToOneAssociation specification, NakedObject @ref)
    {
    }

    public virtual Naked execute(Action action, Naked[] parameters) => (Naked) null;

    public virtual Hint getHint(Action action, Naked[] parameters) => (Hint) null;

    public virtual Hint getHint(NakedObjectField field, Naked value) => (Hint) null;

    public virtual void clearViewDirty()
    {
    }

    public virtual int getMinumumLength() => 0;

    public virtual int getMaximumLength() => 0;

    public virtual bool canClear() => false;

    public virtual void clear()
    {
    }

    public virtual bool isEmpty() => false;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractNakedValue abstractNakedValue = this;
      ObjectImpl.clone((object) abstractNakedValue);
      return ((object) abstractNakedValue).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract sbyte[] asEncodedString();

    public abstract string getIconName();

    public abstract object getObject();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public abstract void parseTextEntry(string text);

    public abstract void restoreFromEncodedString(sbyte[] data);

    public abstract string titleString();
  }
}
