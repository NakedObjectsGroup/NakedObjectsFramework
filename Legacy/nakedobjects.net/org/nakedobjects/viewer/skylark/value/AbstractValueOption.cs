// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.AbstractValueOption
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.nakedobjects.@object;

namespace org.nakedobjects.viewer.skylark.value
{
  public abstract class AbstractValueOption : AbstractUserAction
  {
    [JavaFlags(0)]
    public AbstractValueOption(string name)
      : base(name)
    {
    }

    [JavaFlags(4)]
    public virtual NakedValue getValue(View view) => ((ValueContent) view.getContent()).getObject();

    [JavaFlags(4)]
    public virtual void updateParent(View view)
    {
      NakedObjects.getObjectPersistor().saveChanges();
      view.markDamaged();
      view.getParent().invalidateContent();
    }

    [JavaFlags(4)]
    public virtual bool isEmpty(View view) => ((ValueContent) view.getContent()).isEmpty();
  }
}
