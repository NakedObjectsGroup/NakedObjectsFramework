// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using java.util;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  [JavaInterfaces("1;org/nakedobjects/object/NakedObjectSpecification;")]
  public class JavaSpecification : NakedObjectSpecification
  {
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly object[] NO_PARAMETERS;
    private Action[] classActions;
    private Hint classHint;
    private Method clearDirtyMethod;
    private NakedObjectField[] fields;
    private string fullName;
    private NakedObjectSpecification[] interfaces;
    private JavaIntrospector introspector;
    private bool isAbstract;
    private bool isCollection;
    private Method isDirtyMethod;
    private bool isLookup;
    private bool isObject;
    private bool isValue;
    private Method markDirtyMethod;
    private Action[] objectActions;
    private Persistable persistable;
    private string pluralName;
    private string shortName;
    private string singularName;
    private JavaSpecification.SubclassList subclasses;
    private NakedObjectSpecification superclass;
    private ObjectTitle title;
    private NakedObjectField[] accessibleFields;

    public JavaSpecification(Class cls, ReflectionPeerBuilder builder)
    {
      this.introspector = new JavaIntrospector(cls, builder);
      this.subclasses = new JavaSpecification.SubclassList(this);
    }

    public virtual void addSubclass(NakedObjectSpecification subclass) => this.subclasses.addSubclass(subclass);

    public virtual void clearDirty(NakedObject @object)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void deleted(NakedObject @object)
    {
    }

    private Action getAction(
      Action[] availableActions,
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters)
    {
      if (name == null)
        return (Action) null;
label_10:
      for (int index1 = 0; index1 < availableActions.Length; ++index1)
      {
        Action availableAction = availableActions[index1];
        if (availableAction.getType().Equals((object) type) && StringImpl.equals(availableAction.getId(), (object) name) && availableAction.getParameterTypes().Length == parameters.Length)
        {
          for (int index2 = 0; index2 < parameters.Length; ++index2)
          {
            if (!parameters[index2].isOfType(availableAction.getParameterTypes()[index2]))
              goto label_10;
          }
          return availableAction;
        }
      }
      return (Action) null;
    }

    private Action[] getActions(
      Action[] availableActions,
      Action.Type type,
      int noParameters)
    {
      Vector vector = new Vector();
      for (int index = 0; index < availableActions.Length; ++index)
      {
        Action availableAction = availableActions[index];
        Action.Type type1 = availableAction.getType();
        if (type1 == Action.SET || type1.Equals((object) type) && (noParameters == -1 || availableAction.getParameterTypes().Length == noParameters))
          vector.addElement((object) availableAction);
      }
      int length = vector.size();
      Action[] actionArray = length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
      vector.copyInto((object[]) actionArray);
      return actionArray;
    }

    public virtual Action[] getButtonActions() => (Action[]) null;

    public virtual Action getClassAction(Action.Type type, string name)
    {
      Action.Type type1 = type;
      string name1 = name;
      int length = 0;
      NakedObjectSpecification[] parameters = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      return this.getClassAction(type1, name1, parameters);
    }

    public virtual Action getClassAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters)
    {
      return name == null ? this.getDefaultAction(this.classActions, type, parameters) : this.getAction(this.classActions, type, name, parameters);
    }

    public virtual Action[] getClassActions(Action.Type type) => this.getActions(this.classActions, type, -1);

    public virtual Hint getClassHint()
    {
      if (this.classHint != null)
        return this.classHint;
      Hint hint = (Hint) null;
      if (this.superclass != null)
        hint = this.superclass.getClassHint();
      return hint;
    }

    private Action getDefaultAction(
      Action[] availableActions,
      Action.Type type,
      NakedObjectSpecification[] parameters)
    {
label_8:
      for (int index1 = 0; index1 < availableActions.Length; ++index1)
      {
        Action availableAction = availableActions[index1];
        if (availableAction.getType().Equals((object) type) && availableAction.getParameterTypes().Length == parameters.Length)
        {
          for (int index2 = 0; index2 < parameters.Length; ++index2)
          {
            if (!parameters[index2].isOfType(availableAction.getParameterTypes()[index2]))
              goto label_8;
          }
          return availableAction;
        }
      }
      return (Action) null;
    }

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectField getField(string name)
    {
      for (int index = 0; index < this.fields.Length; ++index)
      {
        if (StringImpl.equals(this.fields[index].getId(), (object) name))
          return this.fields[index];
      }
      throw new NakedObjectSpecificationException(new StringBuffer().append("No field called '").append(name).append("' in '").append(this.getSingularName()).append("'").ToString());
    }

    public virtual object getFieldExtension(string name, Class cls) => (object) null;

    public virtual Class[] getFieldExtensions(string name)
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual NakedObjectField[] getFields() => this.fields;

    public virtual string getFullName() => this.fullName;

    public virtual Action getObjectAction(Action.Type type, string name)
    {
      Action.Type type1 = type;
      string name1 = name;
      int length = 0;
      NakedObjectSpecification[] parameters = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      return this.getObjectAction(type1, name1, parameters);
    }

    public virtual Action getObjectAction(
      Action.Type type,
      string name,
      NakedObjectSpecification[] parameters)
    {
      return name == null ? this.getDefaultAction(this.objectActions, type, parameters) : this.getAction(this.objectActions, type, name, parameters);
    }

    public virtual Action[] getObjectActions(Action.Type type) => this.getActions(this.objectActions, type, -1);

    public virtual string getPluralName() => this.pluralName;

    public virtual string getShortName() => this.shortName;

    public virtual string getSingularName() => this.singularName;

    public virtual string getTitle(NakedObject @object) => this.title.title(@object);

    public virtual NakedObjectField[] getVisibleFields(NakedObject @object)
    {
      this.getAccessibleFields();
      int length1 = this.accessibleFields.Length;
      NakedObjectField[] nakedObjectFieldArray1 = length1 >= 0 ? new NakedObjectField[length1] : throw new NegativeArraySizeException();
      int num1 = 0;
      for (int index1 = 0; index1 < this.accessibleFields.Length; ++index1)
      {
        if (@object.isVisible(this.accessibleFields[index1]).isAllowed())
        {
          NakedObjectField[] nakedObjectFieldArray2 = nakedObjectFieldArray1;
          int num2;
          num1 = (num2 = num1) + 1;
          int index2 = num2;
          NakedObjectField accessibleField = this.accessibleFields[index1];
          nakedObjectFieldArray2[index2] = accessibleField;
        }
      }
      int length2 = num1;
      NakedObjectField[] nakedObjectFieldArray3 = length2 >= 0 ? new NakedObjectField[length2] : throw new NegativeArraySizeException();
      java.lang.System.arraycopy((object) nakedObjectFieldArray1, 0, (object) nakedObjectFieldArray3, 0, num1);
      return nakedObjectFieldArray3;
    }

    public virtual NakedObjectField[] getAccessibleFields()
    {
      if (this.accessibleFields == null)
      {
        int length1 = this.fields.Length;
        NakedObjectField[] nakedObjectFieldArray1 = length1 >= 0 ? new NakedObjectField[length1] : throw new NegativeArraySizeException();
        int num1 = 0;
        for (int index1 = 0; index1 < this.fields.Length; ++index1)
        {
          if (!this.fields[index1].isHidden() && this.fields[index1].isAuthorised())
          {
            NakedObjectField[] nakedObjectFieldArray2 = nakedObjectFieldArray1;
            int num2;
            num1 = (num2 = num1) + 1;
            int index2 = num2;
            NakedObjectField field = this.fields[index1];
            nakedObjectFieldArray2[index2] = field;
          }
        }
        int length2 = num1;
        this.accessibleFields = length2 >= 0 ? new NakedObjectField[length2] : throw new NegativeArraySizeException();
        java.lang.System.arraycopy((object) nakedObjectFieldArray1, 0, (object) this.accessibleFields, 0, num1);
      }
      return this.accessibleFields;
    }

    public virtual NakedObjectField[] getAccessibleFieldsForCollectiveView() => this.getAccessibleFields();

    public virtual bool hasSubclasses() => this.subclasses.hasSubclasses();

    public virtual NakedObjectSpecification[] interfaces() => this.interfaces;

    public virtual void introspect()
    {
      if (this.introspector == null)
        throw new ReflectionException("Introspection already taken place, cannot introspect again");
      this.introspector.introspect();
      this.classHint = this.introspector.classHint();
      this.clearDirtyMethod = this.introspector.getClearDirtyMethod();
      this.isDirtyMethod = this.introspector.getIsDirtyMethod();
      this.markDirtyMethod = this.introspector.getMarkDirtyMethod();
      this.classActions = this.introspector.getClassActions();
      this.fields = this.introspector.getFields();
      this.objectActions = this.introspector.getObjectActions();
      this.fullName = this.introspector.getFullName();
      this.pluralName = this.introspector.pluralName();
      this.singularName = this.introspector.singularName();
      this.shortName = this.introspector.shortName();
      this.title = this.introspector.title();
      this.isAbstract = this.introspector.isAbstract();
      this.isCollection = this.introspector.isCollection();
      this.isLookup = this.introspector.isLookup();
      this.isObject = this.introspector.isObject();
      this.isValue = this.introspector.isValue();
      this.persistable = this.introspector.persistable();
      string superclass = this.introspector.getSuperclass();
      string[] interfaces = this.introspector.getInterfaces();
      NakedObjectSpecificationLoader specificationLoader = NakedObjects.getSpecificationLoader();
      if (superclass != null)
      {
        this.superclass = specificationLoader.loadSpecification(superclass);
        if (this.superclass != null)
        {
          if (JavaSpecification.LOG.isDebugEnabled())
            JavaSpecification.LOG.debug((object) new StringBuffer().append("  Superclass ").append(superclass).ToString());
          this.superclass.addSubclass((NakedObjectSpecification) this);
        }
      }
      int length = interfaces.Length;
      this.interfaces = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < interfaces.Length; ++index)
      {
        this.interfaces[index] = specificationLoader.loadSpecification(interfaces[index]);
        this.interfaces[index].addSubclass((NakedObjectSpecification) this);
      }
      this.introspector = (JavaIntrospector) null;
    }

    public virtual bool isAbstract() => this.isAbstract;

    public virtual bool isCollection() => this.isCollection;

    public virtual bool isDirty(NakedObject @object)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual bool isLookup() => this.isLookup;

    public virtual bool isObject() => this.isObject;

    public virtual bool isOfType(NakedObjectSpecification specification)
    {
      if (specification == this)
        return true;
      if (this.interfaces != null)
      {
        int index = 0;
        for (int length = this.interfaces.Length; index < length; ++index)
        {
          if (this.interfaces[index].isOfType(specification))
            return true;
        }
      }
      return this.superclass != null && this.superclass.isOfType(specification);
    }

    public virtual bool isValue() => this.isValue;

    public virtual void markDirty(NakedObject @object)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual Persistable persistable() => this.persistable;

    private string searchName(string name) => NameConvertor.simpleName(name);

    public virtual NakedObjectSpecification[] subclasses() => this.subclasses.toArray();

    public virtual NakedObjectSpecification superclass() => this.superclass;

    public override string ToString()
    {
      org.nakedobjects.utility.ToString toString = new org.nakedobjects.utility.ToString((object) this);
      toString.append("name", this.fullName);
      toString.append("persistable", (object) this.persistable);
      toString.append("superclass", this.superclass != null ? this.superclass.getFullName() : "Object");
      return toString.ToString();
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaSpecification()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaSpecification javaSpecification = this;
      ObjectImpl.clone((object) javaSpecification);
      return ((object) javaSpecification).MemberwiseClone();
    }

    [Inner]
    [JavaFlags(34)]
    private class SubclassList
    {
      private Vector classes;
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private JavaSpecification this\u00240;

      public virtual void addSubclass(NakedObjectSpecification subclass) => this.classes.addElement((object) subclass);

      public virtual bool hasSubclasses() => ((this.classes.isEmpty() ? 1 : 0) ^ 1) != 0;

      public virtual NakedObjectSpecification[] toArray()
      {
        int length = this.classes.size();
        NakedObjectSpecification[] objectSpecificationArray = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
        this.classes.copyInto((object[]) objectSpecificationArray);
        return objectSpecificationArray;
      }

      [JavaFlags(2)]
      public SubclassList(JavaSpecification _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 == null)
          ObjectImpl.getClass((object) _param1);
        this.classes = new Vector();
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        JavaSpecification.SubclassList subclassList = this;
        ObjectImpl.clone((object) subclassList);
        return ((object) subclassList).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
