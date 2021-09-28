// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.io.Memento
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.io;
using org.nakedobjects.utility;
using System.ComponentModel;
using System.Threading;

namespace org.nakedobjects.@object.io
{
  [JavaInterfaces("2;org/nakedobjects/object/io/Transferable;java/io/Serializable;")]
  public class Memento : Transferable, Serializable
  {
    private const long serialVersionUID = 1;
    private static readonly org.apache.log4j.Logger LOG;
    private Data state;

    public Memento(NakedObject @object)
    {
      this.state = @object != null ? this.createData((Naked) @object) : (Data) null;
      if (!Memento.LOG.isDebugEnabled())
        return;
      Memento.LOG.debug((object) new StringBuffer().append("created memento for ").append((object) this).ToString());
    }

    public Memento()
    {
    }

    private Data createData(Naked @object)
    {
      if (!(@object is InternalCollection))
        return (Data) this.createObjectData((NakedObject) @object);
      InternalCollection nternalCollection = (InternalCollection) @object;
      int length = nternalCollection.size();
      Data[] elements = length >= 0 ? new Data[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < nternalCollection.size(); ++index)
      {
        NakedObject nakedObject = nternalCollection.elementAt(index);
        elements[index] = new Data(nakedObject.getOid(), nakedObject.getResolveState().name(), nakedObject.getSpecification().getFullName());
      }
      return (Data) new CollectionData(nternalCollection.getOid(), Class.FromType(typeof (InternalCollection)).getName(), elements);
    }

    private ObjectData createObjectData(NakedObject @object)
    {
      NakedObjectSpecification specification = @object.getSpecification();
      ObjectData objectData = new ObjectData(@object.getOid(), @object.getResolveState().name(), specification.getFullName());
      foreach (NakedObjectField field1 in specification.getFields())
      {
        if (!field1.isDerived())
        {
          if (field1.isCollection())
          {
            InternalCollection field2 = (InternalCollection) @object.getField(field1);
            objectData.addField(field1.getId(), (object) this.createData((Naked) field2));
          }
          else if (field1.isObject())
          {
            NakedObject association = @object.getAssociation((OneToOneAssociation) field1);
            object entry = association != null ? (object) new Data(association.getOid(), association.getResolveState().name(), association.getSpecification().getFullName()) : (object) (Data) null;
            objectData.addField(field1.getId(), entry);
          }
          else if (field1.isValue())
          {
            NakedValue nakedValue = @object.getValue((OneToOneAssociation) field1);
            objectData.addField(field1.getId(), (object) nakedValue.asEncodedString());
          }
        }
      }
      return objectData;
    }

    public virtual Oid getOid() => this.state.oid;

    public virtual NakedObject recreateObject()
    {
      if (this.state == null)
        return (NakedObject) null;
      NakedObjectSpecification objectSpecification = NakedObjects.getSpecificationLoader().loadSpecification(this.state.className);
      NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
      NakedObject @object = this.getOid() == null || this.getOid().isNull() ? objectLoader.recreateTransientInstance(objectSpecification) : objectLoader.recreateAdapterForPersistent(this.getOid(), objectSpecification);
      if (Memento.LOG.isDebugEnabled())
        Memento.LOG.debug((object) new StringBuffer().append("recreated object ").append((object) @object.getOid()).ToString());
      this.updateObject(@object, ResolveState.UPDATING);
      return @object;
    }

    private NakedObject recreateReference(Data data)
    {
      NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
      object obj = (object) objectLoader;
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        Oid oid = data.oid;
        NakedObject nakedObject;
        if (oid == null || oid.isNull())
        {
          nakedObject = (NakedObject) null;
        }
        else
        {
          NakedObjectSpecification spec = NakedObjects.getSpecificationLoader().loadSpecification(data.className);
          nakedObject = objectLoader.recreateAdapterForPersistent(oid, spec);
        }
        return nakedObject;
      }
      finally
      {
        Monitor.Exit(obj);
      }
    }

    public override string ToString() => new StringBuffer().append("[").append(this.state != null ? new StringBuffer().append(this.state.className).append("/").append((object) this.state.oid).append((object) this.state).ToString() : (string) null).append("]").ToString();

    public virtual void updateObject(NakedObject @object) => this.updateObject(@object, ResolveState.RESOLVING);

    private void updateObject(NakedObject @object, ResolveState resolveState)
    {
      object oid = (object) @object.getOid();
      if (oid != null && !oid.Equals((object) this.state.oid))
        throw new IllegalArgumentException(new StringBuffer().append("This memento can only be used to update the naked object with the Oid ").append((object) this.state.oid).ToString());
      if (!(this.state is ObjectData))
        throw new NakedObjectRuntimeException(new StringBuffer().append("Expected an ObjectData but got ").append((object) ObjectImpl.getClass((object) this.state)).ToString());
      NakedObjectLoader objectLoader = NakedObjects.getObjectLoader();
      objectLoader.start((NakedReference) @object, resolveState);
      ObjectData state = (ObjectData) this.state;
      foreach (NakedObjectField field in @object.getSpecification().getFields())
      {
        object entry = state.getEntry(field.getId());
        if (!field.isDerived())
        {
          if (field.isCollection())
            this.updateOneToManyAssociation(@object, (OneToManyAssociation) field, (CollectionData) entry);
          else if (field.isObject())
            this.updateOneToOneAssociation(@object, (OneToOneAssociation) field, (Data) entry);
          else if (field.isValue())
            @object.initValue((OneToOneAssociation) field, entry);
        }
      }
      objectLoader.end((NakedReference) @object);
      if (!Memento.LOG.isDebugEnabled())
        return;
      Memento.LOG.debug((object) new StringBuffer().append("object updated ").append((object) @object.getOid()).ToString());
    }

    private void updateOneToManyAssociation(
      NakedObject @object,
      OneToManyAssociation field,
      CollectionData collectionData)
    {
      InternalCollection field1 = (InternalCollection) @object.getField((NakedObjectField) field);
      Vector vector = new Vector();
      int num1 = field1.size();
      for (int index = 0; index < num1; ++index)
        vector.addElement((object) field1.elementAt(index));
      for (int index = 0; index < collectionData.elements.Length; ++index)
      {
        NakedObject nakedObject = this.recreateReference(collectionData.elements[index]);
        if (!field1.contains(nakedObject))
        {
          if (Memento.LOG.isDebugEnabled())
            Memento.LOG.debug((object) new StringBuffer().append("  association ").append((object) field).append(" changed, added ").append((object) nakedObject.getOid()).ToString());
          @object.setAssociation((NakedObjectField) field, nakedObject);
        }
        else
          @object.clearAssociation((NakedObjectField) field, nakedObject);
      }
      int num2 = vector.size();
      for (int index = 0; index < num2; ++index)
      {
        NakedObject @ref = (NakedObject) vector.elementAt(index);
        if (Memento.LOG.isDebugEnabled())
          Memento.LOG.debug((object) new StringBuffer().append("  association ").append((object) field).append(" changed, removed ").append((object) @ref.getOid()).ToString());
        @object.clearAssociation((NakedObjectField) field, @ref);
      }
    }

    private void updateOneToOneAssociation(
      NakedObject @object,
      OneToOneAssociation field,
      Data fieldData)
    {
      if (fieldData == null)
      {
        @object.setValue(field, (object) null);
      }
      else
      {
        NakedObject nakedObject = this.recreateReference(fieldData);
        if (@object.getField((NakedObjectField) field) == nakedObject)
          return;
        if (Memento.LOG.isDebugEnabled())
          Memento.LOG.debug((object) new StringBuffer().append("  association ").append((object) field).append(" changed to ").append((object) nakedObject.getOid()).ToString());
        @object.setValue(field, (object) nakedObject);
      }
    }

    public virtual void writeData(TransferableWriter data) => data.writeObject((Transferable) this.state);

    public virtual void restore(TransferableReader data) => this.state = (Data) data.readObject();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Memento()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Memento memento = this;
      ObjectImpl.clone((object) memento);
      return ((object) memento).MemberwiseClone();
    }
  }
}
