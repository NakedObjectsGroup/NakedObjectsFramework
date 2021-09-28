// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.loader.PojoAdapterHashMap
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
using System;
using System.ComponentModel;

namespace org.nakedobjects.@object.loader
{
  [JavaInterfaces("1;org/nakedobjects/object/loader/PojoAdapterHash;")]
  public class PojoAdapterHashMap : PojoAdapterHash
  {
    private static readonly org.apache.log4j.Logger LOG;
    [JavaFlags(4)]
    public Hashtable pojos;

    public PojoAdapterHashMap()
      : this(1000)
    {
    }

    public PojoAdapterHashMap(int capacity)
    {
      this.pojos = new Hashtable();
      this.pojos = new Hashtable(capacity);
    }

    public virtual void add(object pojo, Naked adapter)
    {
      if (PojoAdapterHashMap.LOG.isDebugEnabled())
        PojoAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" add ").append(pojo).append(" as ").append((object) adapter).ToString());
      this.pojos.put(pojo, (object) adapter);
    }

    public virtual bool containsPojo(object pojo) => this.pojos.containsKey(pojo);

    public virtual Enumeration elements() => this.pojos.elements();

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!PojoAdapterHashMap.LOG.isInfoEnabled())
          return;
        PojoAdapterHashMap.LOG.info((object) "finalizing hash of pojos");
      }
      catch (Exception ex)
      {
      }
    }

    public virtual void debugData(DebugString debug)
    {
      Enumeration enumeration = this.pojos.keys();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        object obj = enumeration.nextElement();
        NakedReference nakedReference = (NakedReference) this.pojos.get(obj);
        DebugString debugString = debug;
        int num2;
        num1 = (num2 = num1) + 1;
        int number = num2;
        debugString.append(number, 5);
        debug.append((object) " '");
        debug.append((object) obj.ToString(), 25);
        debug.append((object) "'    ");
        debug.appendln(nakedReference.ToString());
      }
    }

    public virtual string getDebugTitle() => "POJO Adapter Hashtable";

    public virtual Naked getPojo(object pojo) => (Naked) this.pojos.get(pojo);

    public virtual void reset()
    {
      if (PojoAdapterHashMap.LOG.isDebugEnabled())
        PojoAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(nameof (reset)).ToString());
      this.pojos.clear();
    }

    public virtual void shutdown()
    {
      if (PojoAdapterHashMap.LOG.isDebugEnabled())
        PojoAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(nameof (shutdown)).ToString());
      this.pojos.clear();
    }

    public virtual void remove(NakedObject @object)
    {
      if (PojoAdapterHashMap.LOG.isDebugEnabled())
        PojoAdapterHashMap.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("remove ").append((object) @object).ToString());
      this.pojos.remove(@object.getObject());
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static PojoAdapterHashMap()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      PojoAdapterHashMap pojoAdapterHashMap = this;
      ObjectImpl.clone((object) pojoAdapterHashMap);
      return ((object) pojoAdapterHashMap).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
