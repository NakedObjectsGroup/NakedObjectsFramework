// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class DebugAdapter : DebugInfo
  {
    private readonly Naked @object;

    public DebugAdapter(Naked @object) => this.@object = @object;

    public virtual void debugData(DebugString debug)
    {
      if (this.@object is NakedObject)
      {
        this.dumpObject(this.@object, debug);
      }
      else
      {
        if (!(this.@object is NakedCollection))
          return;
        this.dumpObject(this.@object, debug);
      }
    }

    public virtual string getDebugTitle() => "Adapter";

    private void dumpObject(Naked @object, DebugString info)
    {
      if (@object == null)
        return;
      Dump.adapter(@object, info);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugAdapter debugAdapter = this;
      ObjectImpl.clone((object) debugAdapter);
      return ((object) debugAdapter).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
