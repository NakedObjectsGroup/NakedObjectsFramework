// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.JavaBusinessObjectContainer
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.application;
using System;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java
{
  [JavaInterfaces("1;org/nakedobjects/application/BusinessObjectContainer;")]
  public class JavaBusinessObjectContainer : BusinessObjectContainer
  {
    private static readonly Logger LOG;

    private NakedObject adapterFor(object @object) => NakedObjects.getObjectLoader().getAdapterForElseCreateAdapterForTransient(@object);

    public virtual Vector allInstances(Class cls) => this.allInstances(cls, false);

    public virtual Vector allInstances(Class cls, bool includeSubclasses)
    {
      TypedNakedCollection typedNakedCollection = this.objectPersistor().allInstances(this.getSpecification(cls), includeSubclasses);
      Vector vector = new Vector(typedNakedCollection.size());
      Enumeration enumeration = typedNakedCollection.elements();
      while (enumeration.hasMoreElements())
      {
        NakedObject nakedObject = (NakedObject) enumeration.nextElement();
        vector.addElement(nakedObject.getObject());
      }
      return vector;
    }

    public virtual object createInstance(Class cls)
    {
      if (JavaBusinessObjectContainer.LOG.isDebugEnabled())
        JavaBusinessObjectContainer.LOG.debug((object) new StringBuffer().append("creating new persistent instance of ").append(cls.getName()).ToString());
      return this.objectPersistor().createPersistentInstance(cls.getName()).getObject();
    }

    public virtual object createTransientInstance(Class cls)
    {
      if (JavaBusinessObjectContainer.LOG.isDebugEnabled())
        JavaBusinessObjectContainer.LOG.debug((object) new StringBuffer().append("creating new tranisent instance of ").append(cls.getName()).ToString());
      return this.objectPersistor().createTransientInstance(cls.getName()).getObject();
    }

    public virtual void destroyObject(object @object) => this.objectPersistor().destroyObject(this.adapterFor(@object));

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!JavaBusinessObjectContainer.LOG.isInfoEnabled())
          return;
        JavaBusinessObjectContainer.LOG.info((object) new StringBuffer().append("finalizing java business object container ").append((object) this).ToString());
      }
      catch (Exception ex)
      {
      }
    }

    private NakedObjectSpecification getSpecification(Class cls) => NakedObjects.getSpecificationLoader().loadSpecification(cls);

    public virtual bool hasInstances(Class cls) => this.objectPersistor().hasInstances(this.getSpecification(cls), false);

    public virtual void init()
    {
    }

    public virtual bool isPersitent(object @object) => this.adapterFor(@object).getOid() != null;

    public virtual void makePersistent(object transientObject)
    {
      NakedObject @object = this.adapterFor(transientObject);
      this.objectPersistor().makePersistent(@object);
    }

    public virtual int numberOfInstances(Class cls) => this.objectPersistor().numberOfInstances(this.getSpecification(cls), false);

    public virtual void objectChanged(object @object)
    {
      if (@object == null)
        return;
      NakedObject object1 = this.adapterFor(@object);
      this.objectPersistor().objectChanged(object1);
    }

    private NakedObjectPersistor objectPersistor() => NakedObjects.getObjectPersistor();

    public virtual void resolve(object parent, object field)
    {
      if (field != null)
        return;
      NakedObject @object = this.adapterFor(parent);
      if (!@object.getResolveState().isResolvable(ResolveState.RESOLVING))
        return;
      this.objectPersistor().resolveImmediately(@object);
    }

    public virtual long serialNumber(string sequence)
    {
      if (JavaBusinessObjectContainer.LOG.isDebugEnabled())
        JavaBusinessObjectContainer.LOG.debug((object) new StringBuffer().append("serialNumber ").append(sequence).ToString());
      Enumeration enumeration = this.allInstances(Class.FromType(typeof (Sequence)), false).elements();
      while (enumeration.hasMoreElements())
      {
        Sequence sequence1 = (Sequence) enumeration.nextElement();
        if (sequence1.getName().isSameAs(sequence))
        {
          sequence1.getSerialNumber().next();
          this.objectChanged((object) sequence1);
          return sequence1.getSerialNumber().longValue();
        }
      }
      Sequence transientInstance = (Sequence) this.createTransientInstance(Class.FromType(typeof (Sequence)));
      transientInstance.getName().setValue(sequence);
      this.makePersistent((object) transientInstance);
      return transientInstance.getSerialNumber().longValue();
    }

    public virtual void informUser(string message) => NakedObjects.getMessageBroker().addMessage(message);

    public virtual void warnUser(string message) => NakedObjects.getMessageBroker().addWarning(message);

    public virtual void raiseError(string message) => throw new org.nakedobjects.application.ApplicationException(message);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static JavaBusinessObjectContainer()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaBusinessObjectContainer businessObjectContainer = this;
      ObjectImpl.clone((object) businessObjectContainer);
      return ((object) businessObjectContainer).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
