// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.loader.IdentityAdapterHashMap
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.loader;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.loader
{
  [JavaInterfaces("1;org/nakedobjects/object/loader/IdentityAdapterMap;")]
  public class IdentityAdapterHashMap : IdentityAdapterMap
  {
    private static readonly org.apache.log4j.Logger LOG;
    private Hashtable adapters;

    public IdentityAdapterHashMap()
      : this(1000)
    {
    }

    public IdentityAdapterHashMap(int capacity) => this.adapters = new Hashtable(capacity);

    public virtual void add(Oid oid, NakedReference adapter)
    {
      if (IdentityAdapterHashMap.LOG.isDebugEnabled())
        IdentityAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("add ").append((object) oid).append(" as ").append((object) adapter).ToString());
      this.adapters.put((object) oid, (object) adapter);
    }

    public virtual void debugData(DebugString debug)
    {
      Enumeration enumeration = this.oids();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        Oid oid = (Oid) enumeration.nextElement();
        NakedReference adapter = (NakedReference) this.getAdapter(oid);
        DebugString debugString = debug;
        int num2;
        num1 = (num2 = num1) + 1;
        int number = num2;
        debugString.append(number, 5);
        debug.append((object) " '");
        debug.append((object) oid.ToString(), 25);
        debug.append((object) "'    ");
        debug.appendln(adapter.ToString());
      }
    }

    public virtual NakedObject getAdapter(Oid oid) => (NakedObject) this.adapters.get((object) oid);

    public virtual string getDebugTitle() => "Identity adapter map";

    public virtual bool isIdentityKnown(Oid oid) => this.adapters.containsKey((object) oid);

    public virtual Enumeration oids() => this.adapters.keys();

    public virtual void remove(Oid oid)
    {
      if (IdentityAdapterHashMap.LOG.isDebugEnabled())
        IdentityAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("remove ").append((object) oid).ToString());
      this.adapters.remove((object) oid);
    }

    public virtual void reset()
    {
      if (IdentityAdapterHashMap.LOG.isDebugEnabled())
        IdentityAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(nameof (reset)).ToString());
      this.adapters.clear();
    }

    public virtual void shutdown()
    {
      if (IdentityAdapterHashMap.LOG.isDebugEnabled())
        IdentityAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(nameof (shutdown)).ToString());
      this.adapters.clear();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static IdentityAdapterHashMap()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      IdentityAdapterHashMap identityAdapterHashMap = this;
      ObjectImpl.clone((object) identityAdapterHashMap);
      return ((object) identityAdapterHashMap).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
