// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.transaction.OneToOneTransaction
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using System.ComponentModel;

namespace org.nakedobjects.@object.transaction
{
  public class OneToOneTransaction : AbstractOneToOnePeer
  {
    private static readonly Logger LOG;

    public OneToOneTransaction(OneToOnePeer local)
      : base(local)
    {
    }

    public override void clearAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void setAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void setValue(NakedObject inObject, object value)
    {
      // ISSUE: unable to decompile the method.
    }

    private void abort(NakedObjectPersistor objectManager)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static OneToOneTransaction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
