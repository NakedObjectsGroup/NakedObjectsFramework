// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.loader.ObjectLoaderImpl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.defaults;
using org.nakedobjects.@object.loader;
using org.nakedobjects.@object.value.adapter;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.loader
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectLoader;")]
  public class ObjectLoaderImpl : NakedObjectLoader
  {
    private static readonly org.apache.log4j.Logger LOG;
    private ObjectFactory objectFactory;
    private PojoAdapterMap pojoAdapterMap;
    private IdentityAdapterMap identityAdapterMap;
    private AdapterFactory adapterFactory;

    private void addIdentityMapping(Oid oid, NakedReference adapter)
    {
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" adding identity ").append((object) oid).append(" for ").append((object) adapter).ToString());
      this.identityAdapterMap.add(oid, adapter);
    }

    public virtual NakedObject createAdapterForTransient(object @object)
    {
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("creating adapter (transient) for ").append(@object).ToString());
      NakedObject objectAdapter = this.createObjectAdapter(@object);
      Assert.assertEquals((object) objectAdapter, (object) this.pojoAdapterMap.getPojo(@object));
      ((AbstractNakedReference) objectAdapter).changeState(ResolveState.TRANSIENT);
      return objectAdapter;
    }

    public virtual NakedValue createAdapterForValue(object value)
    {
      Assert.assertFalse("can't create an adapter for a NOF adapter", value is Naked);
      Assert.assertFalse("can't create an adapter for a NO Specification", value is NakedObjectSpecification);
      NakedValue nakedValue;
      if (\u003CVerifierFix\u003E.isInstanceOfString(value))
      {
        nakedValue = (NakedValue) new StringAdapter(\u003CVerifierFix\u003E.genCastToString(value));
      }
      else
      {
        switch (value)
        {
          case Date _:
            nakedValue = (NakedValue) new DateAdapter((Date) value);
            break;
          case Boolean _:
            nakedValue = (NakedValue) new BooleanAdapter((Boolean) value);
            break;
          case Character _:
            nakedValue = (NakedValue) new CharAdapter((Character) value);
            break;
          case Byte _:
            nakedValue = (NakedValue) new ByteAdapter((Byte) value);
            break;
          case Short _:
            nakedValue = (NakedValue) new ShortAdapter((Short) value);
            break;
          case Integer _:
            nakedValue = (NakedValue) new IntAdapter((Integer) value);
            break;
          case Long _:
            nakedValue = (NakedValue) new LongAdapter((Long) value);
            break;
          case Float _:
            nakedValue = (NakedValue) new FloatAdapter((Float) value);
            break;
          case Double _:
            nakedValue = (NakedValue) new DoubleAdapter((Double) value);
            break;
          default:
            nakedValue = this.adapterFactory.createValueAdapter(value);
            break;
        }
      }
      return nakedValue;
    }

    public virtual NakedCollection createAdapterForCollection(
      object collection,
      NakedObjectSpecification specification)
    {
      Assert.assertFalse("Can't create an adapter for a NOF adapter", collection is Naked);
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("creating adapter (collection) for ").append(collection).ToString());
      NakedCollection collectionAdapter = this.adapterFactory.createCollectionAdapter(collection, specification);
      if (collectionAdapter != null)
      {
        this.pojoAdapterMap.add(collection, (Naked) collectionAdapter);
        if (ObjectLoaderImpl.LOG.isDebugEnabled())
          ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("created ").append((object) collectionAdapter).append(" for ").append(collection).ToString());
        collectionAdapter.changeState(ResolveState.TRANSIENT);
        Assert.assertNotNull((object) collectionAdapter);
      }
      return collectionAdapter;
    }

    private NakedObject createObjectAdapter(object @object)
    {
      Assert.assertNotNull(@object);
      Assert.assertFalse("POJO Map already contains object", @object, this.pojoAdapterMap.containsPojo(@object));
      Assert.assertFalse("Can't create an adapter for a NOF adapter", @object is Naked);
      NakedObject nakedObject1 = (NakedObject) new PojoAdapter(@object);
      this.pojoAdapterMap.add(@object, (Naked) nakedObject1);
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
      {
        org.apache.log4j.Logger log = ObjectLoaderImpl.LOG;
        StringBuffer stringBuffer = new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" created PojoAdapter@");
        NakedObject nakedObject2 = nakedObject1;
        string hexString = Integer.toHexString(!(nakedObject2 is string) ? ObjectImpl.hashCode((object) nakedObject2) : StringImpl.hashCode((string) nakedObject2));
        string str = stringBuffer.append(hexString).append(" for ").append(@object).ToString();
        log.debug((object) str);
      }
      return nakedObject1;
    }

    public virtual NakedObject createTransientInstance(
      NakedObjectSpecification specification)
    {
      Assert.assertTrue("must be an object", specification.isObject());
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" creating transient instance of ").append((object) specification).ToString());
      object @object = this.objectFactory.createObject(specification);
      NakedObject adapterForTransient = this.createAdapterForTransient(@object);
      this.objectFactory.setUpAsNewLogicalObject(@object);
      return adapterForTransient;
    }

    public virtual NakedCollection recreateCollection(
      NakedObjectSpecification specification)
    {
      Assert.assertFalse("must not be an object", specification.isObject());
      Assert.assertFalse("must not be a value", specification.isValue());
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" recreating collection ").append((object) specification).ToString());
      return this.createAdapterForCollection(this.objectFactory.createObject(specification), specification);
    }

    public virtual NakedObject recreateTransientInstance(
      NakedObjectSpecification specification)
    {
      Assert.assertTrue("must be an object", (object) specification, specification.isObject());
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" recreating transient instance of for ").append((object) specification).ToString());
      return this.createAdapterForTransient(this.objectFactory.createObject(specification));
    }

    public virtual NakedValue createValueInstance(NakedObjectSpecification specification)
    {
      Assert.assertTrue("must be a value", specification.isValue());
      return this.createAdapterForValue(this.objectFactory.createValueObject(specification));
    }

    public virtual NakedObject getAdapterFor(object @object)
    {
      Assert.assertNotNull("can't get an adapter for null", (object) this, @object);
      return (NakedObject) this.pojoAdapterMap.getPojo(@object);
    }

    public virtual NakedObject getAdapterFor(Oid oid)
    {
      Assert.assertNotNull("OID should not be null", (object) this, (object) oid);
      this.updateOid(oid);
      return this.identityAdapterMap.getAdapter(oid);
    }

    public virtual NakedCollection getAdapterForElseCreateAdapterForCollection(
      NakedObject parent,
      string fieldName,
      NakedObjectSpecification specification,
      object collection)
    {
      Assert.assertNotNull("can't get an adapter for null", (object) this, collection);
      InternalCollectionKey key = InternalCollectionKey.createKey(parent, fieldName);
      NakedCollection nakedCollection = (NakedCollection) this.pojoAdapterMap.getPojo((object) key);
      if (nakedCollection == null)
      {
        nakedCollection = this.adapterFactory.createCollectionAdapter(collection, specification);
        this.pojoAdapterMap.add((object) key, (Naked) nakedCollection);
        if (parent.getResolveState().isPersistent())
        {
          if (ObjectLoaderImpl.LOG.isDebugEnabled())
            ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("creating adapter for persistent collection: ").append(collection).ToString());
          nakedCollection.changeState(ResolveState.GHOST);
        }
        else
        {
          if (ObjectLoaderImpl.LOG.isDebugEnabled())
            ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("creating adapter for transient collection: ").append(collection).ToString());
          nakedCollection.changeState(ResolveState.TRANSIENT);
        }
      }
      Assert.assertNotNull("should have an adapter for ", collection, (object) nakedCollection);
      return nakedCollection;
    }

    public virtual NakedObject getAdapterForElseCreateAdapterForTransient(object @object)
    {
      NakedObject nakedObject = this.getAdapterFor(@object);
      if (nakedObject == null)
      {
        if (ObjectLoaderImpl.LOG.isDebugEnabled())
          ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("no existing adapter found; creating a transient adapter for ").append(@object).ToString());
        nakedObject = this.createAdapterForTransient(@object);
      }
      Assert.assertNotNull("should have an adapter for ", @object, (object) nakedObject);
      return nakedObject;
    }

    public virtual void debugData(DebugString debug)
    {
      debug.appendTitle("POJO-Adapter Mappings");
      debug.append((DebugInfo) this.pojoAdapterMap);
      debug.appendln();
      debug.appendTitle("Identity-Adapter Mappings");
      Enumeration enumeration = this.identityAdapterMap.oids();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        Oid oid = (Oid) enumeration.nextElement();
        NakedObject adapter = this.identityAdapterMap.getAdapter(oid);
        DebugString debugString = debug;
        int num2;
        num1 = (num2 = num1) + 1;
        int number = num2;
        debugString.append(number, 5);
        debug.append((object) " ");
        debug.append((object) oid.ToString(), 8);
        debug.append((object) "    ");
        debug.appendln(adapter.ToString());
      }
      debug.appendln();
    }

    public virtual string getDebugTitle() => "Object Loader";

    public virtual Enumeration getIdentifiedObjects() => this.pojoAdapterMap.elements();

    public virtual void init()
    {
      if (ObjectLoaderImpl.LOG.isInfoEnabled())
        ObjectLoaderImpl.LOG.info((object) new StringBuffer().append("initialising ").append((object) this).ToString());
      Assert.assertNotNull("needs an object factory", (object) this.objectFactory);
      Assert.assertNotNull("needs an adapter factory", (object) this.adapterFactory);
      if (this.identityAdapterMap == null)
        this.identityAdapterMap = (IdentityAdapterMap) new IdentityAdapterHashMap();
      if (this.pojoAdapterMap != null)
        return;
      this.pojoAdapterMap = (PojoAdapterMap) new PojoAdapterHashMap();
    }

    public virtual bool isIdentityKnown(Oid oid)
    {
      Assert.assertNotNull((object) oid);
      this.updateOid(oid);
      return this.identityAdapterMap.isIdentityKnown(oid);
    }

    public virtual void start(NakedReference @object, ResolveState state)
    {
      Assert.assertNotNull("Cannot change state into Null state", (object) state);
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("start ").append((object) @object).append(" as ").append(state.name()).ToString());
      @object.changeState(state);
    }

    public virtual void end(NakedReference @object)
    {
      ResolveState endState = @object.getResolveState().getEndState();
      Assert.assertNotNull("Cannot change state into Null state", (object) endState);
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("end ").append((object) @object).append(" as ").append(endState.name()).ToString());
      @object.changeState(endState);
    }

    public virtual void madePersistent(NakedReference adapter, Oid assignedOid)
    {
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("made persistent ").append((object) adapter).append(" as ").append((object) assignedOid).ToString());
      Assert.assertTrue("No adapter found in map", this.pojoAdapterMap.getPojo(adapter.getObject()) != null);
      Assert.assertTrue("Not the same adapter in map", this.pojoAdapterMap.getPojo(adapter.getObject()) == adapter);
      Assert.assertTrue("OID should not already map to a known adapter ", (object) assignedOid, this.identityAdapterMap.getAdapter(assignedOid) == null);
      adapter.persistedAs(assignedOid);
      this.addIdentityMapping(assignedOid, adapter);
    }

    public virtual NakedObject recreateAdapterForPersistent(
      Oid oid,
      NakedObjectSpecification specification)
    {
      Assert.assertNotNull("must have an OID", (object) oid);
      Assert.assertTrue("must be an object", specification.isObject());
      if (this.isIdentityKnown(oid))
        return this.getAdapterFor(oid);
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("recreating object ").append(specification.getFullName()).append("/").append((object) oid).ToString());
      object obj = this.objectFactory.createObject(specification);
      Assert.assertNotNull((object) oid);
      Assert.assertFalse("Identity Map already contains object for OID ", (object) oid, this.identityAdapterMap.isIdentityKnown(oid));
      PojoAdapter objectAdapter = (PojoAdapter) this.createObjectAdapter(obj);
      this.addIdentityMapping(oid, (NakedReference) objectAdapter);
      Assert.assertTrue(this.pojoAdapterMap.getPojo(obj) == objectAdapter);
      Assert.assertTrue(this.identityAdapterMap.getAdapter(oid) == objectAdapter);
      objectAdapter.recreatedAs(oid);
      return (NakedObject) objectAdapter;
    }

    public virtual NakedObject recreateAdapterForPersistent(Oid oid, object @object)
    {
      Assert.assertNotNull("must have an OID", (object) oid);
      if (this.isIdentityKnown(oid))
        return this.getAdapterFor(oid);
      Assert.assertNotNull((object) oid);
      Assert.assertFalse("Identity Map already contains object for OID ", (object) oid, this.identityAdapterMap.isIdentityKnown(oid));
      PojoAdapter objectAdapter = (PojoAdapter) this.createObjectAdapter(@object);
      this.addIdentityMapping(oid, (NakedReference) objectAdapter);
      Assert.assertTrue(this.pojoAdapterMap.getPojo(@object) == objectAdapter);
      Assert.assertTrue(this.identityAdapterMap.getAdapter(oid) == objectAdapter);
      objectAdapter.recreatedAs(oid);
      return (NakedObject) objectAdapter;
    }

    public virtual void reset()
    {
      this.identityAdapterMap.reset();
      this.pojoAdapterMap.reset();
      InternalCollectionKey.reset();
    }

    public virtual IdentityAdapterMap IdentityAdapterMap
    {
      set => this.identityAdapterMap = value;
    }

    public virtual ObjectFactory ObjectFactory
    {
      set => this.objectFactory = value;
    }

    public virtual AdapterFactory AdapterFactory
    {
      set => this.adapterFactory = value;
    }

    public virtual PojoAdapterMap PojoAdapterMap
    {
      set => this.pojoAdapterMap = value;
    }

    public virtual void setIdentityAdapterMap(IdentityAdapterMap identityAdapterMap) => this.identityAdapterMap = identityAdapterMap;

    public virtual void setObjectFactory(ObjectFactory objectFactory) => this.objectFactory = objectFactory;

    public virtual void setPojoAdapterMap(PojoAdapterMap pojoAdpaterMap) => this.pojoAdapterMap = pojoAdpaterMap;

    public virtual void setAdapterFactory(AdapterFactory adapterFactory) => this.adapterFactory = adapterFactory;

    public virtual void shutdown()
    {
      if (ObjectLoaderImpl.LOG.isInfoEnabled())
        ObjectLoaderImpl.LOG.info((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append(" shutting down ").append((object) this).ToString());
      this.identityAdapterMap.shutdown();
      this.identityAdapterMap = (IdentityAdapterMap) null;
      this.pojoAdapterMap.shutdown();
      this.adapterFactory = (AdapterFactory) null;
      InternalCollectionKey.reset();
    }

    public virtual void unloaded(NakedObject @object)
    {
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("unload ignored: ").append((object) @object).ToString());
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("removed loaded object ").append((object) @object).ToString());
      Oid oid = @object.getOid();
      if (oid != null)
        this.identityAdapterMap.remove(oid);
      this.pojoAdapterMap.remove(@object);
    }

    private void updateOid(Oid oid)
    {
      if (!oid.hasPrevious())
        return;
      NakedObject adapter = this.identityAdapterMap.getAdapter(oid.getPrevious());
      if (adapter == null)
        return;
      if (ObjectLoaderImpl.LOG.isDebugEnabled())
        ObjectLoaderImpl.LOG.debug((object) new StringBuffer().append("#").append(Long.toHexString((long) this.GetHashCode())).append("updating oid ").append((object) oid.getPrevious()).append(" to ").append((object) oid).ToString());
      this.identityAdapterMap.remove(oid.getPrevious());
      Oid oid1 = adapter.getOid();
      oid1.copyFrom(oid);
      this.identityAdapterMap.add(oid1, (NakedReference) adapter);
    }

    public virtual void stateCreateChangeLock(Oid oid, NakedObjectSpecification spec)
    {
    }

    public virtual void stateCreateChangeRelease(Oid oid, NakedObjectSpecification spec)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static ObjectLoaderImpl()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ObjectLoaderImpl objectLoaderImpl = this;
      ObjectImpl.clone((object) objectLoaderImpl);
      return ((object) objectLoaderImpl).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
