// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.ValueContent
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.basic;

namespace org.nakedobjects.viewer.skylark
{
  public abstract class ValueContent : AbstractContent
  {
    public abstract void clear();

    public abstract void entryComplete();

    public override Image getIconPicture(int iconHeight) => throw new NotImplementedException();

    public abstract NakedValue getObject();

    public abstract bool isEmpty();

    public override bool isPersistable() => false;

    public override bool isTransient() => false;

    public override bool isValue() => true;

    public abstract override void parseTextEntry(string entryText);

    public abstract Consent isEditable();
  }
}
