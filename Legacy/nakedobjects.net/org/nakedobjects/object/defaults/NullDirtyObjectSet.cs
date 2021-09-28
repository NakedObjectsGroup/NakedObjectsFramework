// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.NullDirtyObjectSet
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/DirtyObjectSet;")]
  public class NullDirtyObjectSet : DirtyObjectSet
  {
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addDirty(NakedObject @object)
    {
    }

    public virtual void init()
    {
    }

    public virtual void shutdown()
    {
    }

    public override string ToString() => new org.nakedobjects.utility.ToString((object) this).ToString();

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      NullDirtyObjectSet nullDirtyObjectSet = this;
      ObjectImpl.clone((object) nullDirtyObjectSet);
      return ((object) nullDirtyObjectSet).MemberwiseClone();
    }
  }
}
