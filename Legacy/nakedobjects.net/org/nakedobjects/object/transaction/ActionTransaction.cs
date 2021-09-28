// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.transaction.ActionTransaction
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
  public class ActionTransaction : AbstractActionPeer
  {
    private static readonly Logger LOG;

    public ActionTransaction(ActionPeer decorated)
      : base(decorated)
    {
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectiveActionException;")]
    public override Naked execute(NakedReference @object, Naked[] parameters)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static ActionTransaction()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
