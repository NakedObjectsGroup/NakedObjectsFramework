// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.AbstractContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.basic
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Content;")]
  public abstract class AbstractContent : Content
  {
    public virtual void contentMenuOptions(UserActionSet options)
    {
    }

    public virtual bool isCollection() => false;

    public virtual bool isDerived() => false;

    public virtual bool isObject() => false;

    public virtual bool isPersistable() => false;

    public virtual bool isValue() => false;

    public virtual void viewMenuOptions(UserActionSet options)
    {
    }

    public virtual string windowTitle() => "";

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      AbstractContent abstractContent = this;
      ObjectImpl.clone((object) abstractContent);
      return ((object) abstractContent).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    public abstract Consent canDrop(Content sourceContent);

    public abstract void debugDetails(DebugString debug);

    public abstract Naked drop(Content sourceContent);

    public abstract string getDescription();

    public abstract string getHelp();

    public abstract string getIconName();

    public abstract Image getIconPicture(int iconHeight);

    public abstract string getId();

    public abstract Naked getNaked();

    public abstract NakedObjectSpecification getSpecification();

    public abstract bool isTransient();

    [JavaThrownExceptions("1;org/nakedobjects/object/InvalidEntryException;")]
    public abstract void parseTextEntry(string entryText);

    public abstract string title();
  }
}
