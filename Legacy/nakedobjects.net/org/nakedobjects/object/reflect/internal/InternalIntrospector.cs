// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.object.reflect.internal.InternalIntrospector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.lang.reflect;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.@object.control;
using org.nakedobjects.@object.reflect;
using org.nakedobjects.@object.reflect.@internal;
using org.nakedobjects.@object.reflect.@internal.about;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.@object.reflect.@internal
{
  public class InternalIntrospector
  {
    private const string ABOUT_PREFIX = "about";
    public const bool CLASS = true;
    private const string DERIVE_PREFIX = "derive";
    private const string GET_PREFIX = "get";
    private static readonly Category LOG;
    public const bool OBJECT = false;
    private const string SET_PREFIX = "set";
    private ReflectionPeerBuilder builder;
    private Action[] classActions;
    private Class cls;
    private NakedObjectField[] fields;
    private Method[] methods;
    private Action[] objectActions;

    private static string[] readSortOrder(Class aClass, string type)
    {
      try
      {
        Class @class = aClass;
        string str1 = new StringBuffer().append(type).append("Order").ToString();
        int length1 = 0;
        Class[] classArray = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        Method method1 = @class.getMethod(str1, classArray);
        if (Modifier.isStatic(method1.getModifiers()))
        {
          Method method2 = method1;
          int length2 = 0;
          object[] objArray = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
          string str2 = \u003CVerifierFix\u003E.genCastToString(method2.invoke((object) null, objArray));
          if (StringImpl.length(StringImpl.trim(str2)) <= 0)
            return (string[]) null;
          StringTokenizer stringTokenizer = new StringTokenizer(str2, ",");
          int length3 = stringTokenizer.countTokens();
          string[] strArray = length3 >= 0 ? new string[length3] : throw new NegativeArraySizeException();
          int index = 0;
          while (stringTokenizer.hasMoreTokens())
          {
            strArray[index] = StringImpl.trim(stringTokenizer.nextToken());
            ++index;
          }
          return strArray;
        }
        if (InternalIntrospector.LOG.isWarnEnabled())
          InternalIntrospector.LOG.warn((object) new StringBuffer().append("method ").append(aClass.getName()).append(".").append(type).append("Order() must be decared as static").ToString());
      }
      catch (NoSuchMethodException ex)
      {
      }
      catch (IllegalAccessException ex)
      {
      }
      catch (InvocationTargetException ex)
      {
      }
      return (string[]) null;
    }

    [JavaFlags(12)]
    public static string shortClassName(string fullyQualifiedClassName) => StringImpl.substring(fullyQualifiedClassName, StringImpl.lastIndexOf(fullyQualifiedClassName, 46) + 1);

    public virtual Action[] getClassActions() => this.classActions;

    public virtual NakedObjectField[] getFields() => this.fields;

    public virtual Action[] getObjectActions() => this.objectActions;

    public InternalIntrospector(Class cls, ReflectionPeerBuilder builder)
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) new StringBuffer().append("creating JavaIntrospector for ").append((object) cls).ToString());
      this.builder = builder;
      if (!Class.FromType(typeof (InternalNakedObject)).isAssignableFrom(cls) && !StringImpl.startsWith(cls.getName(), "java.") && !Class.FromType(typeof (Exception)).isAssignableFrom(cls))
        throw new NakedObjectSpecificationException(new StringBuffer().append("Class must be InternalNakedObject: ").append(cls.getName()).ToString());
      this.cls = Modifier.isPublic(cls.getModifiers()) ? cls : throw new NakedObjectSpecificationException(new StringBuffer().append("A NakedObject class must be marked as public.  Error in ").append((object) cls).ToString());
      this.methods = cls.getMethods();
    }

    public virtual ActionPeer[] actionPeers(bool forClass)
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) "  looking for action methods");
      Vector vector = new Vector();
      Vector actions = new Vector();
      for (int index1 = 0; index1 < this.methods.Length; ++index1)
      {
        if (this.methods[index1] != null)
        {
          Method method1 = this.methods[index1];
          if (Modifier.isStatic(method1.getModifiers()) == forClass)
          {
            int length1 = 3;
            string[] strArray1 = length1 >= 0 ? new string[length1] : throw new NegativeArraySizeException();
            strArray1[0] = "action";
            strArray1[1] = "explorationAction";
            strArray1[2] = "debugAction";
            string[] strArray2 = strArray1;
            int index2 = -1;
            for (int index3 = 0; index3 < strArray2.Length; ++index3)
            {
              if (StringImpl.startsWith(method1.getName(), strArray2[index3]))
              {
                index2 = index3;
                break;
              }
            }
            if (index2 != -1)
            {
              Class[] parameterTypes = method1.getParameterTypes();
              vector.addElement((object) method1);
              if (InternalIntrospector.LOG.isDebugEnabled())
                InternalIntrospector.LOG.debug((object) new StringBuffer().append("  identified action ").append((object) method1).ToString());
              string name1 = method1.getName();
              this.methods[index1] = (Method) null;
              string name2 = StringImpl.substring(name1, StringImpl.length(strArray2[index2]));
              int length2 = parameterTypes.Length + 1;
              Class[] paramTypes = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
              paramTypes[0] = Class.FromType(typeof (InternalAbout));
              java.lang.System.arraycopy((object) parameterTypes, 0, (object) paramTypes, 1, parameterTypes.Length);
              string name3 = new StringBuffer().append("about").append(StringImpl.toUpperCase(StringImpl.substring(name1, 0, 1))).append(StringImpl.substring(name1, 1)).ToString();
              Method method2 = this.findMethod(forClass, name3, (Class) null, paramTypes);
              if (method2 != null && InternalIntrospector.LOG.isDebugEnabled())
                InternalIntrospector.LOG.debug((object) new StringBuffer().append("  with about method ").append((object) method2).ToString());
              int length3 = 3;
              Action.Type[] typeArray = length3 >= 0 ? new Action.Type[length3] : throw new NegativeArraySizeException();
              typeArray[0] = Action.USER;
              typeArray[1] = Action.EXPLORATION;
              typeArray[2] = Action.DEBUG;
              Action.Type type = typeArray[index2];
              ActionPeer actionPeer = (ActionPeer) new InternalAction(this.className(), name2, type, method1);
              actions.addElement((object) actionPeer);
            }
          }
        }
      }
      return this.convertToArray(actions);
    }

    public virtual string[] actionSortOrder()
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) "  looking  for action sort order");
      return InternalIntrospector.readSortOrder(this.cls, "action");
    }

    public virtual string[] classActionSortOrder()
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) "  looking  for class action sort order");
      return InternalIntrospector.readSortOrder(this.cls, "classAction");
    }

    public virtual Hint classHint()
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) "  looking  for class about");
      try
      {
        InternalAbout internalAbout = new InternalAbout();
        Class cls = this.cls;
        string str = new StringBuffer().append("about").append(this.shortName()).ToString();
        int length1 = 1;
        Class[] classArray = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        classArray[0] = Class.FromType(typeof (InternalAbout));
        Method method = cls.getMethod(str, classArray);
        int length2 = 1;
        object[] objArray = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
        objArray[0] = (object) internalAbout;
        method.invoke((object) null, objArray);
        return (Hint) internalAbout;
      }
      catch (NoSuchMethodException ex)
      {
      }
      catch (IllegalAccessException ex)
      {
      }
      catch (InvocationTargetException ex)
      {
      }
      return (Hint) null;
    }

    private string className() => this.cls.getName();

    public virtual void clearDirty(NakedObject @object)
    {
    }

    private ActionPeer[] convertToArray(Vector actions)
    {
      int length = actions.size();
      ActionPeer[] actionPeerArray1 = length >= 0 ? new ActionPeer[length] : throw new NegativeArraySizeException();
      Enumeration enumeration = actions.elements();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        ActionPeer[] actionPeerArray2 = actionPeerArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        ActionPeer actionPeer = (ActionPeer) enumeration.nextElement();
        actionPeerArray2[index] = actionPeer;
      }
      return actionPeerArray1;
    }

    private void derivedFields(Vector fields)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "derive", (Class) null, 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (InternalIntrospector.LOG.isDebugEnabled())
          InternalIntrospector.LOG.debug((object) new StringBuffer().append("  identified derived value method ").append((object) get).ToString());
        string name = get.getName();
        InternalOneToOneAssociation toOneAssociation = new InternalOneToOneAssociation(false, this.className(), name, get.getReturnType(), get, (Method) null, (Method) null, (Method) null);
        fields.addElement((object) toOneAssociation);
      }
    }

    public virtual void destroyed(NakedObject @object)
    {
    }

    public virtual FieldPeer[] fields()
    {
      if (InternalIntrospector.LOG.isDebugEnabled())
        InternalIntrospector.LOG.debug((object) "  looking  for fields");
      Vector vector = new Vector();
      this.valueFields(vector, Class.FromType(typeof (string)));
      this.derivedFields(vector);
      this.oneToManyAssociationFields(vector);
      this.oneToOneAssociationFields(vector);
      int length = vector.size();
      FieldPeer[] fieldPeerArray = length >= 0 ? new FieldPeer[length] : throw new NegativeArraySizeException();
      vector.copyInto((object[]) fieldPeerArray);
      return fieldPeerArray;
    }

    public virtual string[] fieldSortOrder() => InternalIntrospector.readSortOrder(this.cls, "field");

    private Method findMethod(
      bool forClass,
      string name,
      Class returnType,
      Class[] paramTypes)
    {
label_11:
      for (int index1 = 0; index1 < this.methods.Length; ++index1)
      {
        if (this.methods[index1] != null)
        {
          Method method = this.methods[index1];
          if (Modifier.isPublic(method.getModifiers()) && Modifier.isStatic(method.getModifiers()) == forClass && StringImpl.equals(method.getName(), (object) name) && (returnType == null || returnType == method.getReturnType()))
          {
            if (paramTypes != null)
            {
              if (paramTypes.Length == method.getParameterTypes().Length)
              {
                for (int index2 = 0; index2 < paramTypes.Length; ++index2)
                {
                  if (paramTypes[index2] != null && paramTypes[index2] != method.getParameterTypes()[index2])
                    goto label_11;
                }
              }
              else
                continue;
            }
            this.methods[index1] = (Method) null;
            return method;
          }
        }
      }
      return (Method) null;
    }

    private Vector findPrefixedMethods(
      bool forClass,
      string prefix,
      Class returnType,
      bool canBeVoid,
      int paramCount)
    {
      Vector vector = new Vector();
      for (int index = 0; index < this.methods.Length; ++index)
      {
        if (this.methods[index] != null)
        {
          Method method = this.methods[index];
          if (Modifier.isStatic(method.getModifiers()) == forClass)
          {
            bool flag1 = StringImpl.startsWith(method.getName(), prefix);
            bool flag2 = method.getParameterTypes().Length == paramCount;
            Class returnType1 = method.getReturnType();
            bool flag3 = returnType == null || canBeVoid && returnType1 == Void.TYPE || returnType.isAssignableFrom(returnType1);
            if (flag1 && flag2 && flag3)
            {
              vector.addElement((object) method);
              this.methods[index] = (Method) null;
            }
          }
        }
      }
      return vector;
    }

    private Vector findPrefixedMethods(
      bool forClass,
      string prefix,
      Class returnType,
      int paramCount)
    {
      return this.findPrefixedMethods(forClass, prefix, returnType, false, paramCount);
    }

    public virtual string getFullName() => this.cls.getName();

    public virtual object getExtension(Class cls) => (object) null;

    public virtual Class[] getExtensions()
    {
      int length = 0;
      return length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
    }

    public virtual string[] getInterfaces()
    {
      Class[] interfaces = this.cls.getInterfaces();
      int length1 = interfaces.Length;
      Class[] classArray1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      int num1 = 0;
      for (int index1 = 0; index1 < interfaces.Length; ++index1)
      {
        Class[] classArray2 = classArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index2 = num2;
        Class @class = interfaces[index1];
        classArray2[index2] = @class;
      }
      int length2 = num1;
      string[] strArray = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
      for (int index = 0; index < num1; ++index)
        strArray[index] = classArray1[index].getName();
      return strArray;
    }

    public virtual string getSuperclass()
    {
      Class superclass = this.cls.getSuperclass();
      return superclass == null || superclass == Class.FromType(typeof (object)) ? (string) null : superclass.getName();
    }

    private Action[] createActions(ReflectionPeerBuilder builder, ActionPeer[] actions)
    {
      int length = actions.Length;
      Action[] actionArray = length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < actions.Length; ++index)
        actionArray[index] = builder.createAction(this.getFullName(), actions[index]);
      return actionArray;
    }

    private Action[] createActions(
      ReflectionPeerBuilder builder,
      ActionPeer[] delegates,
      string[] order)
    {
      Action[] actions = this.createActions(builder, delegates);
      return (Action[]) this.orderArray(Class.FromType(typeof (Action)), (NakedObjectMember[]) actions, order);
    }

    private NakedObjectField[] createFields(
      ReflectionPeerBuilder builder,
      FieldPeer[] fieldPeers)
    {
      int length = fieldPeers.Length;
      NakedObjectField[] nakedObjectFieldArray = length >= 0 ? new NakedObjectField[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < fieldPeers.Length; ++index)
      {
        object fieldPeer = (object) fieldPeers[index];
        switch (fieldPeer)
        {
          case OneToOnePeer _:
            nakedObjectFieldArray[index] = builder.createField(this.getFullName(), (OneToOnePeer) fieldPeer);
            break;
          case OneToManyPeer _:
            nakedObjectFieldArray[index] = builder.createField(this.getFullName(), (OneToManyPeer) fieldPeer);
            break;
          default:
            throw new NakedObjectRuntimeException();
        }
      }
      return nakedObjectFieldArray;
    }

    [JavaFlags(4)]
    public virtual NakedObjectMember[] orderArray(
      Class memberType,
      NakedObjectMember[] original,
      string[] order)
    {
      if (order == null)
        return original;
      for (int index = 0; index < order.Length; ++index)
        order[index] = NameConvertor.simpleName(order[index]);
      NakedObjectMember[] nakedObjectMemberArray1 = (NakedObjectMember[]) Array.newInstance(memberType, original.Length);
      bool flag = InternalIntrospector.LOG.isWarnEnabled();
      int num1 = 0;
label_14:
      for (int index1 = 0; index1 < order.Length; ++index1)
      {
        for (int index2 = 0; index2 < original.Length; ++index2)
        {
          NakedObjectMember nakedObjectMember1 = original[index2];
          if (nakedObjectMember1 != null && StringImpl.equalsIgnoreCase(nakedObjectMember1.getId(), order[index1]))
          {
            NakedObjectMember[] nakedObjectMemberArray2 = nakedObjectMemberArray1;
            int num2;
            num1 = (num2 = num1) + 1;
            int index3 = num2;
            NakedObjectMember nakedObjectMember2 = original[index2];
            nakedObjectMemberArray2[index3] = nakedObjectMember2;
            original[index2] = (NakedObjectMember) null;
            goto label_14;
          }
        }
        if (!StringImpl.equals(StringImpl.trim(order[index1]), (object) "") && flag)
          InternalIntrospector.LOG.warn((object) new StringBuffer().append("invalid ordering element '").append(order[index1]).append("' in ").append(this.getFullName()).ToString());
      }
      NakedObjectMember[] nakedObjectMemberArray3 = (NakedObjectMember[]) Array.newInstance(memberType, original.Length);
      int num3 = 0;
      for (int index4 = 0; index4 < nakedObjectMemberArray1.Length; ++index4)
      {
        NakedObjectMember nakedObjectMember3 = nakedObjectMemberArray1[index4];
        if (nakedObjectMember3 != null)
        {
          NakedObjectMember[] nakedObjectMemberArray4 = nakedObjectMemberArray3;
          int num4;
          num3 = (num4 = num3) + 1;
          int index5 = num4;
          NakedObjectMember nakedObjectMember4 = nakedObjectMember3;
          nakedObjectMemberArray4[index5] = nakedObjectMember4;
        }
      }
      for (int index6 = 0; index6 < original.Length; ++index6)
      {
        NakedObjectMember nakedObjectMember5 = original[index6];
        if (nakedObjectMember5 != null)
        {
          NakedObjectMember[] nakedObjectMemberArray5 = nakedObjectMemberArray3;
          int num5;
          num3 = (num5 = num3) + 1;
          int index7 = num5;
          NakedObjectMember nakedObjectMember6 = nakedObjectMember5;
          nakedObjectMemberArray5[index7] = nakedObjectMember6;
        }
      }
      return nakedObjectMemberArray3;
    }

    [JavaFlags(4)]
    public virtual void introspect()
    {
      if (InternalIntrospector.LOG.isInfoEnabled())
        InternalIntrospector.LOG.info((object) new StringBuffer().append("introspecting ").append(this.cls.getName()).ToString());
      this.objectActions = this.createActions(this.builder, this.actionPeers(false), this.actionSortOrder());
      this.classActions = this.createActions(this.builder, this.actionPeers(true), this.classActionSortOrder());
      NakedObjectField[] fields = this.createFields(this.builder, this.fields());
      this.fields = (NakedObjectField[]) this.orderArray(Class.FromType(typeof (NakedObjectField)), (NakedObjectMember[]) fields, this.fieldSortOrder());
    }

    public virtual bool isAbstract() => Modifier.isAbstract(this.cls.getModifiers());

    public virtual bool isCollection() => Class.FromType(typeof (NakedCollection)).isAssignableFrom(this.cls);

    public virtual bool isDirty(NakedObject @object) => false;

    public virtual bool isLookup() => false;

    public virtual bool isObject() => Class.FromType(typeof (InternalNakedObject)).isAssignableFrom(this.cls);

    public virtual bool isPartOf() => Class.FromType(typeof (Aggregated)).isAssignableFrom(this.cls);

    public virtual bool isValue() => Class.FromType(typeof (string)).isAssignableFrom(this.cls) || Class.FromType(typeof (Date)).isAssignableFrom(this.cls);

    public virtual void markDirty(NakedObject @object)
    {
    }

    [JavaFlags(0)]
    public virtual string[] names(Vector methods)
    {
      int length = methods.size();
      string[] strArray1 = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
      Enumeration enumeration = methods.elements();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        Method method = (Method) enumeration.nextElement();
        string[] strArray2 = strArray1;
        int num2;
        num1 = (num2 = num1) + 1;
        int index = num2;
        string name = method.getName();
        strArray2[index] = name;
      }
      return strArray1;
    }

    private void oneToManyAssociationFields(Vector associations)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", Class.FromType(typeof (Vector)), 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (InternalIntrospector.LOG.isDebugEnabled())
          InternalIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-many association method ").append((object) get).ToString());
        string name1 = StringImpl.substring(get.getName(), StringImpl.length("get"));
        string name2 = new StringBuffer().append("about").append(name1).ToString();
        int length = 3;
        Class[] paramTypes = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        paramTypes[0] = Class.FromType(typeof (InternalAbout));
        paramTypes[1] = (Class) null;
        paramTypes[2] = (Class) Boolean.TYPE;
        Class parameterType1 = this.findMethod(false, name2, (Class) null, paramTypes)?.getParameterTypes()[1];
        Method add = (this.findMethod(false, new StringBuffer().append("addTo").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("add").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("associate").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null);
        Method remove = (this.findMethod(false, new StringBuffer().append("removeFrom").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("remove").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("dissociate").append(name1).ToString(), (Class) Void.TYPE, (Class[]) null);
        if (add == null || remove == null)
          InternalIntrospector.LOG.error((object) new StringBuffer().append("there must be both add and remove methods for ").append(name1).append(" in ").append(this.className()).ToString());
        Class parameterType2 = remove?.getParameterTypes()[0];
        Class parameterType3 = add?.getParameterTypes()[0];
        Class class1 = parameterType1 ?? (Class) null;
        Class class2 = parameterType3 ?? class1;
        Class type = parameterType2 ?? class2;
        if (parameterType1 != null && parameterType1 != type || parameterType3 != null && parameterType3 != type || parameterType2 != null && parameterType2 != type)
          InternalIntrospector.LOG.error((object) new StringBuffer().append("the add/remove/associate/dissociate/about methods in ").append(this.className()).append(" must ").append("all deal with same type of object.  There are at least two different ").append("types").ToString());
        associations.addElement((object) new InternalOneToManyAssociation(this.className(), name1, type, get, add, remove));
      }
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectionException;")]
    private void oneToOneAssociationFields(Vector associations)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", Class.FromType(typeof (object)), 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (InternalIntrospector.LOG.isDebugEnabled())
          InternalIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-1 association method ").append((object) get).ToString());
        if (!StringImpl.equals(get.getName(), (object) "getNakedClass"))
        {
          string name = StringImpl.substring(get.getName(), StringImpl.length("get"));
          int length = 1;
          Class[] classArray = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
          classArray[0] = get.getReturnType();
          Class[] paramTypes = classArray;
          Method add = this.findMethod(false, new StringBuffer().append("associate").append(name).ToString(), (Class) Void.TYPE, paramTypes) ?? this.findMethod(false, new StringBuffer().append("add").append(name).ToString(), (Class) Void.TYPE, paramTypes);
          Method remove = this.findMethod(false, new StringBuffer().append("dissociate").append(name).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("remove").append(name).ToString(), (Class) Void.TYPE, (Class[]) null);
          Method method = this.findMethod(false, new StringBuffer().append("set").append(name).ToString(), (Class) Void.TYPE, paramTypes);
          if (method != null)
          {
            if (InternalIntrospector.LOG.isDebugEnabled())
              InternalIntrospector.LOG.debug((object) new StringBuffer().append("one-to-one association ").append(name).append(" ->").append((object) add).ToString());
            InternalOneToOneAssociation toOneAssociation = new InternalOneToOneAssociation(true, this.className(), name, get.getReturnType(), get, method, add, remove);
            associations.addElement((object) toOneAssociation);
          }
        }
      }
    }

    public virtual Persistable persistable() => Persistable.TRANSIENT;

    public virtual string pluralName()
    {
      try
      {
        Class cls = this.cls;
        int length1 = 0;
        Class[] classArray = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        Method method = cls.getMethod(nameof (pluralName), classArray);
        int length2 = 0;
        object[] objArray = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
        return \u003CVerifierFix\u003E.genCastToString(method.invoke((object) null, objArray));
      }
      catch (NoSuchMethodException ex)
      {
      }
      catch (IllegalAccessException ex)
      {
      }
      catch (InvocationTargetException ex)
      {
      }
      return this.shortName();
    }

    public virtual string shortName()
    {
      string name = this.cls.getName();
      return StringImpl.substring(name, StringImpl.lastIndexOf(name, 46) + 1);
    }

    public virtual string singularName()
    {
      try
      {
        Class cls = this.cls;
        int length1 = 0;
        Class[] classArray = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        Method method = cls.getMethod(nameof (singularName), classArray);
        int length2 = 0;
        object[] objArray = length2 >= 0 ? new object[length2] : throw new NegativeArraySizeException();
        return \u003CVerifierFix\u003E.genCastToString(method.invoke((object) null, objArray));
      }
      catch (NoSuchMethodException ex)
      {
      }
      catch (IllegalAccessException ex)
      {
      }
      catch (InvocationTargetException ex)
      {
      }
      return this.shortName();
    }

    public virtual ObjectTitle title()
    {
      Method titleMethod = this.findMethod(false, "titleString", Class.FromType(typeof (string)), (Class[]) null) ?? this.findMethod(false, nameof (title), Class.FromType(typeof (string)), (Class[]) null);
      return titleMethod == null ? (ObjectTitle) new InternalIntrospector.\u0031(this) : (ObjectTitle) new InternalObjectTitle(titleMethod);
    }

    private Vector valueFields(Vector fields, Class type)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", type, 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        Class returnType = get.getReturnType();
        string name1 = StringImpl.substring(get.getName(), StringImpl.length("get"));
        string name2 = new StringBuffer().append("set").append(name1).ToString();
        string name3 = name2;
        int length1 = 1;
        Class[] paramTypes1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        paramTypes1[0] = returnType;
        Method method = this.findMethod(false, name3, (Class) null, paramTypes1);
        int length2 = 1;
        Class[] classArray = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
        classArray[0] = returnType;
        Class[] paramTypes2 = classArray;
        if (this.findMethod(false, name2, (Class) Void.TYPE, paramTypes2) != null)
          InternalIntrospector.LOG.error((object) new StringBuffer().append("the method set").append(name1).append(" is not needed for the NakedValue class ").append(this.className()).ToString());
        if (this.findMethod(false, new StringBuffer().append("associate").append(name1).ToString(), (Class) Void.TYPE, paramTypes2) != null)
          InternalIntrospector.LOG.error((object) new StringBuffer().append("the method associate").append(name1).append(" is not needed for the NakedValue class ").append(this.className()).ToString());
        if (InternalIntrospector.LOG.isDebugEnabled())
          InternalIntrospector.LOG.debug((object) new StringBuffer().append("  value ").append(name1).append(" ->").append((object) get).ToString());
        InternalOneToOneAssociation toOneAssociation = new InternalOneToOneAssociation(false, this.className(), name1, get.getReturnType(), get, method, (Method) null, (Method) null);
        fields.addElement((object) toOneAssociation);
      }
      return fields;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static InternalIntrospector()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      InternalIntrospector internalIntrospector = this;
      ObjectImpl.clone((object) internalIntrospector);
      return ((object) internalIntrospector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(32)]
    [Inner]
    [JavaInterfaces("1;org/nakedobjects/object/reflect/ObjectTitle;")]
    public class \u0031 : ObjectTitle
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private InternalIntrospector this\u00240;

      public virtual string title(NakedObject @object) => @object.getObject().ToString();

      public \u0031(InternalIntrospector _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        InternalIntrospector.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
