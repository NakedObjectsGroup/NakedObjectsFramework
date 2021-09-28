// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugObjectGraph
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
  public class DebugObjectGraph : DebugInfo
  {
    private readonly Naked @object;

    public DebugObjectGraph(Naked @object) => this.@object = @object;

    public virtual void debugData(DebugString debug)
    {
      if (this.@object is NakedObject)
      {
        this.dumpGraph(this.@object, debug);
      }
      else
      {
        if (!(this.@object is NakedCollection))
          return;
        this.dumpGraph(this.@object, debug);
      }
    }

    public virtual string getDebugTitle() => "Object Graph";

    private void dumpGraph(Naked @object, DebugString info)
    {
      if (@object == null)
        return;
      Dump.graph(@object, info);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugObjectGraph debugObjectGraph = this;
      ObjectImpl.clone((object) debugObjectGraph);
      return ((object) debugObjectGraph).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
