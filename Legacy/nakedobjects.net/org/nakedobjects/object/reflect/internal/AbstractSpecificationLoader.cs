// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.AbstractSpecificationLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Threading;

namespace org.nakedobjects.@object.reflect.@internal
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectSpecificationLoader;")]
  public abstract class AbstractSpecificationLoader : NakedObjectSpecificationLoader
  {
    private static readonly org.apache.log4j.Logger LOG;
    private SpecificationCache cache;
    private Hashtable loading_cache;
    private ReflectionPeerBuilder reflectionPeerBuilder;

    [JavaFlags(17)]
    public virtual NakedObjectSpecification loadSpecification(Class cls)
    {
      Assert.assertNotNull((object) cls);
      NakedObjectSpecification spec = this.cache.get(cls);
      if (spec != null)
        return spec;
      if (!cls.isPrimitive())
        return this.load(cls);
      object cache = (object) this.cache;
      \u003CCorArrayWrapper\u003E.Enter(cache);
      try
      {
        spec = (NakedObjectSpecification) new PrimitiveSpecification(cls.getName());
        this.cache.cache(cls, spec);
      }
      finally
      {
        Monitor.Exit(cache);
      }
      return spec;
    }

    [JavaFlags(17)]
    public virtual NakedObjectSpecification loadSpecification(
      string className)
    {
      Assert.assertNotNull("specification class must be specified", (object) className);
      try
      {
        return this.load(Class.forName(className));
      }
      catch (ClassNotFoundException ex)
      {
        if (AbstractSpecificationLoader.LOG.isWarnEnabled())
          AbstractSpecificationLoader.LOG.warn((object) new StringBuffer().append("not a class ").append(className).append("; 'null' specification created").ToString());
        NoMemberSpecification memberSpecification = new NoMemberSpecification(className);
        this.cache.cache(className, (NakedObjectSpecification) memberSpecification);
        return (NakedObjectSpecification) memberSpecification;
      }
    }

    private NakedObjectSpecification load(Class cls)
    {
      NakedObjectSpecification objectSpecification1 = this.cache.get(cls);
      if (objectSpecification1 != null)
        return objectSpecification1;
      object cache = (object) this.cache;
      \u003CCorArrayWrapper\u003E.Enter(cache);
      try
      {
        NakedObjectSpecification objectSpecification2 = this.cache.get(cls);
        if (objectSpecification2 != null)
          return objectSpecification2;
        NakedObjectSpecification objectSpecification3 = (NakedObjectSpecification) this.loading_cache.get((object) cls);
        if (objectSpecification3 != null)
          return objectSpecification3;
        string name = cls.getName();
        NakedObjectSpecification spec;
        if (Class.FromType(typeof (InternalNakedObject)).isAssignableFrom(cls) || Class.FromType(typeof (Exception)).isAssignableFrom(cls))
        {
          if (AbstractSpecificationLoader.LOG.isInfoEnabled())
            AbstractSpecificationLoader.LOG.info((object) new StringBuffer().append("initialising specification for ").append(name).append(" using internal reflector").ToString());
          spec = (NakedObjectSpecification) new InternalSpecification(cls, this.reflectionPeerBuilder);
        }
        else
          spec = this.install(cls, this.reflectionPeerBuilder);
        if (spec == null)
        {
          if (AbstractSpecificationLoader.LOG.isInfoEnabled())
            AbstractSpecificationLoader.LOG.info((object) new StringBuffer().append("unrecognised class ").append(name).append("; 'null' specification created").ToString());
          spec = (NakedObjectSpecification) new NoMemberSpecification(name);
        }
        this.loading_cache.put((object) cls, (object) spec);
        spec.introspect();
        this.loading_cache.remove((object) cls);
        this.cache.cache(cls, spec);
        return spec;
      }
      finally
      {
        Monitor.Exit(cache);
      }
    }

    [JavaFlags(1028)]
    public abstract NakedObjectSpecification install(
      Class cls,
      ReflectionPeerBuilder builder);

    public virtual NakedObjectSpecification[] allSpecifications() => this.cache.allSpecifications();

    public virtual void debugData(DebugString debug)
    {
      NakedObjectSpecification[] objectSpecificationArray = this.allSpecifications();
      DebugString debugString = new DebugString();
      for (int index = 0; index < objectSpecificationArray.Length; ++index)
      {
        NakedObjectSpecification objectSpecification = objectSpecificationArray[index];
        debugString.append(!objectSpecification.isAbstract() ? (object) "." : (object) "A");
        debugString.append(!objectSpecification.isCollection() ? (object) "." : (object) "C");
        debugString.append(!objectSpecification.isLookup() ? (object) "." : (object) "L");
        debugString.append(!objectSpecification.isObject() ? (object) "." : (object) "O");
        debugString.append(!objectSpecification.isValue() ? (object) "." : (object) "V");
        debugString.append((object) "  ");
        debugString.appendln(objectSpecification.getFullName());
      }
    }

    public virtual string getDebugTitle() => "Specification Loader";

    public virtual ReflectionPeerFactory[] ReflectionPeerFactories
    {
      set => this.setReflectionPeerFactories(value);
    }

    public virtual SpecificationCache Cache
    {
      set => this.setCache(value);
    }

    public virtual void setCache(SpecificationCache cache) => this.cache = cache;

    public virtual void setReflectionPeerFactories(ReflectionPeerFactory[] factories)
    {
      this.reflectionPeerBuilder = new ReflectionPeerBuilder();
      this.reflectionPeerBuilder.setFactories(factories);
    }

    public virtual void shutdown()
    {
      if (AbstractSpecificationLoader.LOG.isInfoEnabled())
        AbstractSpecificationLoader.LOG.info((object) new StringBuffer().append("shutting down ").append((object) this).ToString());
      this.cache.clear();
    }

    public virtual void init()
    {
      if (AbstractSpecificationLoader.LOG.isInfoEnabled())
        AbstractSpecificationLoader.LOG.info((object) new StringBuffer().append("initialising ").append((object) this).ToString());
      Assert.assertNotNull("ReflectionPeerBuilder needed", (object) this.reflectionPeerBuilder);
      if (this.cache != null)
        return;
      this.cache = (SpecificationCache) new SimpleSpecificationCache();
    }

    public AbstractSpecificationLoader() => this.loading_cache = new Hashtable();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static AbstractSpecificationLoader()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      AbstractSpecificationLoader specificationLoader = this;
      ObjectImpl.clone((object) specificationLoader);
      return ((object) specificationLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
