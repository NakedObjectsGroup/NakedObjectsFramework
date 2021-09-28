// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.defaults.PojoAdapter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.defaults;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.@object.defaults
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObject;")]
  public class PojoAdapter : AbstractNakedReference, NakedObject
  {
    private static readonly Logger LOG;
    private object pojo;

    public PojoAdapter(object pojo) => this.pojo = pojo;

    public virtual void clearAssociation(NakedObjectField field, NakedObject associate)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("clearAssociation ").append(field.getId()).append("/").append((object) associate).append(" in ").append((object) this).ToString());
      if (field is OneToOneAssociation)
        ((OneToOneAssociation) field).clearAssociation((NakedObject) this, associate);
      else
        ((OneToManyAssociation) field).removeElement((NakedObject) this, associate);
    }

    public virtual void clearCollection(OneToManyAssociation field)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("clearCollection ").append(field.getId()).append(" in ").append((object) this).ToString());
      field.clearCollection((NakedObject) this);
    }

    public virtual void clearValue(OneToOneAssociation field)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("clearValue ").append(field.getId()).append(" in ").append((object) this).ToString());
      field.clearValue((NakedObject) this);
    }

    public override void destroyed()
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("deleted notification for ").append((object) this).ToString());
      this.getSpecification().deleted((NakedObject) this);
    }

    [JavaThrownExceptions("1;java/lang/Throwable;")]
    [JavaFlags(4)]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!PojoAdapter.LOG.isDebugEnabled())
          return;
        PojoAdapter.LOG.debug((object) new StringBuffer().append("finalizing pojo: ").append(this.pojo).ToString());
      }
      catch (Exception ex)
      {
      }
    }

    public virtual NakedObject getAssociation(OneToOneAssociation field) => (NakedObject) field.get((NakedObject) this);

    public virtual Naked getField(NakedObjectField field) => field.get((NakedObject) this);

    public virtual NakedObjectField[] getFields() => this.getSpecification().getFields();

    public virtual Consent isValid(OneToOneAssociation field, NakedValue value) => field.isValueValid((NakedObject) this, value);

    public virtual Consent isValid(OneToOneAssociation field, NakedObject nakedObject) => field.isAssociationValid((NakedObject) this, nakedObject);

    public virtual Consent isVisible(NakedObjectField field) => field.isVisible((NakedReference) this);

    public virtual string getDescription(NakedObjectMember member) => member.getDescription();

    public override object getObject() => this.pojo;

    public virtual NakedValue getValue(OneToOneAssociation field) => (NakedValue) field.get((NakedObject) this);

    public virtual NakedObjectField[] getVisibleFields() => this.getSpecification().getVisibleFields((NakedObject) this);

    public virtual void initAssociation(NakedObjectField field, NakedObject associatedObject)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("initAssociation ").append(field.getId()).append("/").append((object) associatedObject).append(" in ").append((object) this).ToString());
      if (field is OneToOneAssociation)
        ((OneToOneAssociation) field).initAssociation((NakedObject) this, associatedObject);
      else
        ((OneToManyAssociation) field).initElement((NakedObject) this, associatedObject);
    }

    public virtual void initAssociation(OneToManyAssociation field, NakedObject[] instances)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("initAssociation ").append(field.getId()).append(" with ").append(instances.Length).append("instances in ").append((object) this).ToString());
      field.initCollection((NakedObject) this, instances);
    }

    public virtual void initValue(OneToOneAssociation field, object @object)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("initValue ").append(field.getId()).append(" with ").append(@object).append(" in ").append((object) this).ToString());
      field.initValue((NakedObject) this, @object);
    }

    public virtual bool isEmpty(NakedObjectField field) => field.isEmpty((NakedObject) this);

    public virtual void setAssociation(NakedObjectField field, NakedObject associatedObject)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("setAssociation ").append(field.getId()).append(" of ").append((object) this).append(" with ").append((object) associatedObject).ToString());
      if (field is OneToOneAssociation)
        ((OneToOneAssociation) field).setAssociation((NakedObject) this, associatedObject);
      else
        ((OneToManyAssociation) field).addElement((NakedObject) this, associatedObject);
    }

    public virtual void setValue(OneToOneAssociation field, object @object)
    {
      if (PojoAdapter.LOG.isDebugEnabled())
        PojoAdapter.LOG.debug((object) new StringBuffer().append("setValue ").append(field.getId()).append(" with ").append(@object).append(" in ").append((object) this).ToString());
      field.setValue((NakedObject) this, @object);
    }

    public override string titleString()
    {
      string str = this.getSpecification().getTitle((NakedObject) this);
      if (str == null && this.getResolveState().isGhost())
      {
        if (PojoAdapter.LOG.isInfoEnabled())
          PojoAdapter.LOG.info((object) new StringBuffer().append("attempting to use unresolved object; resolving it immediately: ").append((object) this).ToString());
        NakedObjects.getObjectPersistor().resolveImmediately((NakedObject) this);
      }
      if (str == null)
        str = this.getDefaultTitle();
      return str;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override string ToString()
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void recreatedAs(Oid oid)
    {
      this.changeState(ResolveState.GHOST);
      this.setOid(oid);
    }

    public virtual Consent canAdd(OneToManyAssociation field, NakedObject element) => field.validToAdd((NakedObject) this, element);

    public virtual Consent isAvailable(NakedObjectField field) => field.isAvailable((NakedReference) this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static PojoAdapter()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
