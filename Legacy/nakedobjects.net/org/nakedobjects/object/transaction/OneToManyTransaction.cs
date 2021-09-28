// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.transaction.OneToManyTransaction
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
  public class OneToManyTransaction : AbstractOneToManyPeer
  {
    private static readonly Logger LOG;

    public OneToManyTransaction(OneToManyPeer peer)
      : base(peer)
    {
    }

    public override void addAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void removeAllAssociations(NakedObject inObject)
    {
      // ISSUE: unable to decompile the method.
    }

    private void abort(NakedObjectPersistor objectManager)
    {
      // ISSUE: unable to decompile the method.
    }

    public override void removeAssociation(NakedObject inObject, NakedObject associate)
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static OneToManyTransaction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
