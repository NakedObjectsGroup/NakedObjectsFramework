// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.persistence.SimpleOidGenerator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.persistence;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.persistence
{
  [JavaInterfaces("1;org/nakedobjects/object/persistence/OidGenerator;")]
  public class SimpleOidGenerator : OidGenerator
  {
    private long next;
    private long start;

    public SimpleOidGenerator() => this.start = 0L;

    public SimpleOidGenerator(long start) => this.start = start;

    public virtual void init() => this.next = this.start;

    public virtual string name() => "Exploration OID Generator";

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

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      SimpleOidGenerator simpleOidGenerator = this;
      ObjectImpl.clone((object) simpleOidGenerator);
      return ((object) simpleOidGenerator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
