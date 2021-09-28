// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.TimeBasedOidGenerator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/OidGenerator;")]
  public class TimeBasedOidGenerator : OidGenerator
  {
    private long next;

    public virtual string name() => "Simple OID Generator";

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Oid next(Naked @object)
    {
      long next;
      this.next = (next = this.next) + 1L;
      return (Oid) new SerialOid(next);
    }

    public virtual void shutdown()
    {
    }

    public virtual void init()
    {
    }

    public TimeBasedOidGenerator() => this.next = new Date().getTime();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      TimeBasedOidGenerator basedOidGenerator = this;
      ObjectImpl.clone((object) basedOidGenerator);
      return ((object) basedOidGenerator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
