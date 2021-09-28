// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.reflector.java.reflect.JavaIntrospector
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
using org.nakedobjects.application;
using org.nakedobjects.application.collection;
using org.nakedobjects.application.control;
using org.nakedobjects.application.value;
using org.nakedobjects.application.valueholder;
using org.nakedobjects.reflector.java.control;
using org.nakedobjects.utility;
using System;
using System.ComponentModel;

namespace org.nakedobjects.reflector.java.reflect
{
  public class JavaIntrospector
  {
    private const string HIDDEN_PREFIX = "Hidden";
    private const string ABOUT_FIELD_DEFAULT = "aboutFieldDefault";
    private const string ABOUT_PREFIX = "about";
    public const bool CLASS = true;
    private const string DERIVE_PREFIX = "derive";
    private const string GET_PREFIX = "get";
    private static readonly org.apache.log4j.Logger LOG;
    private static readonly object[] NO_PARAMETERS;
    public const bool OBJECT = false;
    private const string SET_PREFIX = "set";
    private readonly ReflectionPeerBuilder builder;
    private Action[] classActions;
    private string className;
    private Method clearDirtyMethod;
    private Class cls;
    private Method defaultAboutFieldMethod;
    private NakedObjectField[] fields;
    private Method isDirtyMethod;
    private Method markDirtyMethod;
    private Method[] methods;
    private Action[] objectActions;

    [JavaFlags(12)]
    public static string javaBaseName(string javaName)
    {
      int num1 = 0;
      int num2 = StringImpl.length(javaName);
      while (num1 < num2 && StringImpl.charAt(javaName, num1) != '_' && Character.isLowerCase(StringImpl.charAt(javaName, num1)))
        ++num1;
      if (num1 >= num2)
        return "";
      if (StringImpl.charAt(javaName, num1) == '_')
        ++num1;
      if (num1 >= num2)
        return "";
      string str = StringImpl.substring(javaName, num1);
      char ch = StringImpl.charAt(str, 0);
      return Character.isLowerCase(ch) ? new StringBuffer().append(Character.toUpperCase(ch)).append(StringImpl.substring(str, 1)).ToString() : str;
    }

    private string[] readSortOrder(Class aClass, string type)
    {
      Method method = this.findMethod(true, new StringBuffer().append(type).append("Order").ToString(), Class.FromType(typeof (string)), (Class[]) null);
      if (method == null)
        return (string[]) null;
      try
      {
        if (Modifier.isStatic(method.getModifiers()))
        {
          string str = \u003CVerifierFix\u003E.genCastToString(method.invoke((object) null, JavaIntrospector.NO_PARAMETERS));
          if (StringImpl.length(StringImpl.trim(str)) <= 0)
            return (string[]) null;
          StringTokenizer stringTokenizer = new StringTokenizer(str, ",");
          int length = stringTokenizer.countTokens();
          string[] strArray = length >= 0 ? new string[length] : throw new NegativeArraySizeException();
          int index = 0;
          while (stringTokenizer.hasMoreTokens())
          {
            strArray[index] = StringImpl.trim(stringTokenizer.nextToken());
            ++index;
          }
          return strArray;
        }
        if (JavaIntrospector.LOG.isWarnEnabled())
          JavaIntrospector.LOG.warn((object) new StringBuffer().append("method ").append(aClass.getName()).append(".").append(type).append("Order() must be decared as static").ToString());
      }
      catch (IllegalAccessException ex)
      {
      }
      catch (InvocationTargetException ex)
      {
      }
      return (string[]) null;
    }

    private JavaIntrospector.Set readSortOrder2(Class aClass, string type)
    {
      Method method = this.findMethod(true, new StringBuffer().append(type).append("Order").ToString(), Class.FromType(typeof (string)), (Class[]) null);
      if (method == null)
        return (JavaIntrospector.Set) null;
      string s;
      try
      {
        if (Modifier.isStatic(method.getModifiers()))
        {
          s = \u003CVerifierFix\u003E.genCastToString(method.invoke((object) null, JavaIntrospector.NO_PARAMETERS));
          if (StringImpl.length(StringImpl.trim(s)) == 0)
            return (JavaIntrospector.Set) null;
        }
        else
        {
          if (JavaIntrospector.LOG.isWarnEnabled())
            JavaIntrospector.LOG.warn((object) new StringBuffer().append("method ").append(aClass.getName()).append(".").append(type).append("Order() must be decared as static").ToString());
          return (JavaIntrospector.Set) null;
        }
      }
      catch (IllegalAccessException ex)
      {
        if (JavaIntrospector.LOG.isWarnEnabled())
          JavaIntrospector.LOG.warn((object) new StringBuffer().append("method ").append(aClass.getName()).append(".").append(type).append("Order() must be decared as public").ToString());
        return (JavaIntrospector.Set) null;
      }
      catch (InvocationTargetException ex)
      {
        throw new ReflectionException((Throwable) ex);
      }
      return this.extractOrder(s);
    }

    private JavaIntrospector.Set extractOrder(string s)
    {
      JavaIntrospector.Set set = new JavaIntrospector.Set();
      StringTokenizer stringTokenizer = new StringTokenizer(s, ",");
      while (stringTokenizer.hasMoreTokens())
      {
        string element1 = StringImpl.trim(stringTokenizer.nextToken());
        bool flag;
        if (flag = StringImpl.endsWith(element1, ")"))
          element1 = StringImpl.trim(StringImpl.substring(element1, 0, StringImpl.length(element1) - 1));
        if (StringImpl.startsWith(element1, "("))
        {
          int num = StringImpl.indexOf(element1, 58);
          string groupName = StringImpl.trim(StringImpl.substring(element1, 1, num));
          string element2 = StringImpl.trim(StringImpl.substring(element1, num + 1));
          set = new JavaIntrospector.Set(set, groupName, element2);
        }
        else
          set.add(element1);
        if (flag)
          set = set.getParent();
      }
      return set;
    }

    [JavaFlags(12)]
    public static string shortClassName(string fullyQualifiedClassName) => StringImpl.substring(fullyQualifiedClassName, StringImpl.lastIndexOf(fullyQualifiedClassName, 46) + 1);

    public JavaIntrospector(Class cls, ReflectionPeerBuilder builder)
    {
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) new StringBuffer().append("creating JavaIntrospector for ").append((object) cls).ToString());
      this.builder = builder;
      this.cls = Modifier.isPublic(cls.getModifiers()) ? cls : throw new NakedObjectSpecificationException(new StringBuffer().append("A NakedObject class must be marked as public.  Error in ").append((object) cls).ToString());
      this.methods = cls.getMethods();
      for (int index = 0; index < this.methods.Length; ++index)
      {
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  ").append((object) this.methods[index]).ToString());
      }
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) "");
      // ISSUE: variable of the null type
      __Null type1 = Boolean.TYPE;
      int length1 = 0;
      Class[] paramTypes1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      this.isDirtyMethod = this.findMethod(false, "isDirty", (Class) type1, paramTypes1);
      // ISSUE: variable of the null type
      __Null type2 = Void.TYPE;
      int length2 = 0;
      Class[] paramTypes2 = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
      this.clearDirtyMethod = this.findMethod(false, "clearDirty", (Class) type2, paramTypes2);
      // ISSUE: variable of the null type
      __Null type3 = Void.TYPE;
      int length3 = 0;
      Class[] paramTypes3 = length3 >= 0 ? new Class[length3] : throw new NegativeArraySizeException();
      this.markDirtyMethod = this.findMethod(false, "markDirty", (Class) type3, paramTypes3);
      this.className = cls.getName();
    }

    private ActionPeer[] actionPeers(bool forClass)
    {
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) "  looking for action methods");
      int num = forClass ? 1 : 0;
      int length1 = 1;
      Class[] paramTypes1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      paramTypes1[0] = Class.FromType(typeof (ActionAbout));
      Method method1 = this.findMethod(num != 0, "aboutActionDefault", (Class) null, paramTypes1);
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug(method1 != null ? (object) method1.ToString() : (object) "  no default about method for actions");
      Vector actions = new Vector();
      for (int index1 = 0; index1 < this.methods.Length; ++index1)
      {
        if (this.methods[index1] != null)
        {
          Method method2 = this.methods[index1];
          if (Modifier.isStatic(method2.getModifiers()) == forClass)
          {
            string name1 = method2.getName();
            int length2 = 3;
            string[] strArray1 = length2 >= 0 ? new string[length2] : throw new NegativeArraySizeException();
            strArray1[0] = "action";
            strArray1[1] = "explorationAction";
            strArray1[2] = "debugAction";
            string[] strArray2 = strArray1;
            int index2 = -1;
            for (int index3 = 0; index3 < strArray2.Length; ++index3)
            {
              if (StringImpl.startsWith(name1, strArray2[index3]))
              {
                index2 = index3;
                break;
              }
            }
            if (index2 != -1)
            {
              Class[] parameterTypes = method2.getParameterTypes();
              if (JavaIntrospector.LOG.isDebugEnabled())
                JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified action ").append((object) method2).ToString());
              this.methods[index1] = (Method) null;
              string name2 = StringImpl.substring(name1, StringImpl.length(strArray2[index2]));
              Action.Target target = Action.DEFAULT;
              if (StringImpl.startsWith(name2, "Local"))
              {
                target = Action.LOCAL;
                name2 = StringImpl.substring(name2, 5);
              }
              else if (StringImpl.startsWith(name2, "Remote"))
              {
                target = Action.REMOTE;
                name2 = StringImpl.substring(name2, 6);
              }
              int length3 = parameterTypes.Length + 1;
              Class[] paramTypes2 = length3 >= 0 ? new Class[length3] : throw new NegativeArraySizeException();
              paramTypes2[0] = Class.FromType(typeof (ActionAbout));
              java.lang.System.arraycopy((object) parameterTypes, 0, (object) paramTypes2, 1, parameterTypes.Length);
              string name3 = new StringBuffer().append("about").append(StringImpl.toUpperCase(StringImpl.substring(name1, 0, 1))).append(StringImpl.substring(name1, 1)).ToString();
              Method aboutMethod = this.findMethod(forClass, name3, (Class) null, paramTypes2);
              if (aboutMethod == null)
                aboutMethod = method1;
              else if (JavaIntrospector.LOG.isDebugEnabled())
                JavaIntrospector.LOG.debug((object) new StringBuffer().append("  with about method ").append((object) aboutMethod).ToString());
              int length4 = 3;
              Action.Type[] typeArray = length4 >= 0 ? new Action.Type[length4] : throw new NegativeArraySizeException();
              typeArray[0] = Action.USER;
              typeArray[1] = Action.EXPLORATION;
              typeArray[2] = Action.DEBUG;
              Action.Type action1 = typeArray[index2];
              ActionPeer action2 = this.createAction(method2, name2, aboutMethod, action1, target);
              actions.addElement((object) action2);
            }
          }
        }
      }
      return this.convertToArray(actions);
    }

    private JavaIntrospector.Set actionSortOrder()
    {
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) "  looking for action sort order");
      return this.readSortOrder2(this.cls, "action");
    }

    private JavaIntrospector.Set classActionSortOrder()
    {
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) "  looking for class action sort order");
      return this.readSortOrder2(this.cls, "classAction");
    }

    public virtual Hint classHint()
    {
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) "  looking for class about");
      try
      {
        SimpleClassAbout simpleClassAbout = new SimpleClassAbout((Session) null, (object) null);
        Method classAboutMethod = this.getClassAboutMethod(this.shortName());
        int length = 1;
        object[] objArray = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
        objArray[0] = (object) simpleClassAbout;
        classAboutMethod.invoke((object) null, objArray);
        return (Hint) simpleClassAbout;
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

    private ActionPeer createAction(
      Method method,
      string name,
      Method aboutMethod,
      Action.Type action,
      Action.Target target)
    {
      Class[] parameterTypes = method.getParameterTypes();
      int length = parameterTypes.Length;
      NakedObjectSpecification[] objectSpecificationArray = length >= 0 ? new NakedObjectSpecification[length] : throw new NegativeArraySizeException();
      for (int index = 0; index < parameterTypes.Length; ++index)
        objectSpecificationArray[index] = this.specification(parameterTypes[index]);
      return (ActionPeer) new JavaAction((MemberIdentifier) new MemberIdentifierImpl(this.className, name, objectSpecificationArray), action, objectSpecificationArray, target, method, aboutMethod);
    }

    private NakedObjectSpecification specification(Class returnType) => NakedObjects.getSpecificationLoader().loadSpecification(returnType.getName());

    private Action[] createActions(
      ReflectionPeerBuilder builder,
      ActionPeer[] delegates,
      JavaIntrospector.Set order)
    {
      int length1 = delegates.Length;
      Action[] original = length1 >= 0 ? new Action[length1] : throw new NegativeArraySizeException();
      for (int index = 0; index < delegates.Length; ++index)
        original[index] = builder.createAction(this.className, delegates[index]);
      Action[] actionArray1 = this.extractedOrderedActions(original, order);
      Vector vector = new Vector();
      for (int index = 0; index < actionArray1.Length; ++index)
        vector.addElement((object) actionArray1[index]);
      for (int index = 0; index < original.Length; ++index)
      {
        if (original[index] != null)
          vector.addElement((object) original[index]);
      }
      int length2 = vector.size();
      Action[] actionArray2 = length2 >= 0 ? new Action[length2] : throw new NegativeArraySizeException();
      vector.copyInto((object[]) actionArray2);
      return actionArray2;
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
            nakedObjectFieldArray[index] = builder.createField(this.className, (OneToOnePeer) fieldPeer);
            break;
          case OneToManyPeer _:
            nakedObjectFieldArray[index] = builder.createField(this.className, (OneToManyPeer) fieldPeer);
            break;
          default:
            throw new NakedObjectRuntimeException();
        }
      }
      return nakedObjectFieldArray;
    }

    private void derivedFields(Vector fields)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "derive", (Class) null, 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified derived value method ").append((object) get).ToString());
        string fieldName = JavaIntrospector.javaBaseName(get.getName());
        string name = new StringBuffer().append("about").append(fieldName).ToString();
        int length = 1;
        Class[] paramTypes = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        paramTypes[0] = Class.FromType(typeof (FieldAbout));
        Method about = this.findMethod(false, name, (Class) null, paramTypes) ?? this.defaultAboutFieldMethod;
        bool isHidden = false;
        bool isHiddenInTableViews = false;
        JavaOneToOneAssociation toOneAssociation = new JavaOneToOneAssociation(false, (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName), get.getReturnType(), get, (Method) null, (Method) null, (Method) null, about, isHidden, isHiddenInTableViews, true);
        fields.addElement((object) toOneAssociation);
      }
    }

    private FieldPeer[] fields()
    {
      if (StringImpl.startsWith(this.cls.getName(), "java.") || Class.FromType(typeof (BusinessValueHolder)).isAssignableFrom(this.cls))
      {
        int length = 0;
        return length >= 0 ? new FieldPeer[length] : throw new NegativeArraySizeException();
      }
      this.removeMethod(false, "getClass", Class.FromType(typeof (Class)), (Class[]) null);
      if (JavaIntrospector.LOG.isDebugEnabled())
        JavaIntrospector.LOG.debug((object) new StringBuffer().append("  looking for fields for ").append((object) this.cls).ToString());
      Vector vector = new Vector();
      int length1 = 1;
      Class[] paramTypes = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
      paramTypes[0] = Class.FromType(typeof (FieldAbout));
      this.defaultAboutFieldMethod = this.findMethod(false, "aboutFieldDefault", (Class) null, paramTypes);
      this.valueFields(vector, Class.FromType(typeof (BusinessValueHolder)));
      this.valueFields(vector, Class.FromType(typeof (BusinessValue)));
      this.valueFields(vector, Class.FromType(typeof (string)));
      this.valueFields(vector, Class.FromType(typeof (java.util.Date)));
      this.valueFields(vector, (Class) Boolean.TYPE);
      this.valueFields(vector, (Class) Character.TYPE);
      this.valueFields(vector, (Class) Byte.TYPE);
      this.valueFields(vector, (Class) Short.TYPE);
      this.valueFields(vector, (Class) Integer.TYPE);
      this.valueFields(vector, (Class) Long.TYPE);
      this.valueFields(vector, (Class) Float.TYPE);
      this.valueFields(vector, (Class) Double.TYPE);
      this.derivedFields(vector);
      this.oneToManyAssociationFieldsVector(vector);
      this.oneToManyAssociationFieldsInternalCollection(vector);
      this.oneToManyAssociationFieldsArray(vector);
      this.oneToOneAssociationFields(vector);
      int length2 = vector.size();
      FieldPeer[] fieldPeerArray = length2 >= 0 ? new FieldPeer[length2] : throw new NegativeArraySizeException();
      vector.copyInto((object[]) fieldPeerArray);
      return fieldPeerArray;
    }

    private string[] fieldSortOrder() => this.readSortOrder(this.cls, "field");

    [JavaFlags(4)]
    [JavaThrownExceptions("1;java/lang/Throwable;")]
    public override void Finalize()
    {
      try
      {
        // ISSUE: explicit finalizer call
        base.Finalize();
        if (!JavaIntrospector.LOG.isDebugEnabled())
          return;
        JavaIntrospector.LOG.debug((object) new StringBuffer().append("finalizing reflector ").append((object) this).ToString());
      }
      catch (Exception ex)
      {
      }
    }

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

    [JavaThrownExceptions("1;java/lang/NoSuchMethodException;")]
    private Method getClassAboutMethod(string className)
    {
      Class cls = this.cls;
      string str = new StringBuffer().append("about").append(className).ToString();
      int length = 1;
      Class[] classArray = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
      classArray[0] = Class.FromType(typeof (ClassAbout));
      return cls.getMethod(str, classArray) ?? this.getClassAboutMethod(this.getSuperclass());
    }

    public virtual Action[] getClassActions() => this.classActions;

    public virtual Method getClearDirtyMethod() => this.clearDirtyMethod;

    public virtual NakedObjectField[] getFields() => this.fields;

    public virtual string getFullName() => this.cls.getName();

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

    public virtual Method getIsDirtyMethod() => this.isDirtyMethod;

    public virtual Method getMarkDirtyMethod() => this.markDirtyMethod;

    public virtual Action[] getObjectActions() => this.objectActions;

    public virtual string getSuperclass() => this.cls.getSuperclass()?.getName();

    [JavaFlags(4)]
    public virtual void introspect()
    {
      if (JavaIntrospector.LOG.isInfoEnabled())
        JavaIntrospector.LOG.info((object) new StringBuffer().append("introspecting ").append(this.cls.getName()).ToString());
      this.objectActions = this.createActions(this.builder, this.actionPeers(false), this.actionSortOrder());
      this.classActions = this.createActions(this.builder, this.actionPeers(true), this.classActionSortOrder());
      this.fields = this.createFields(this.builder, this.orderArray(this.fields(), this.fieldSortOrder()));
    }

    public virtual bool isAbstract() => Modifier.isAbstract(this.cls.getModifiers());

    public virtual bool isCollection() => Class.FromType(typeof (Vector)).isAssignableFrom(this.cls) || Class.FromType(typeof (InternalCollection)).isAssignableFrom(this.cls);

    public virtual bool isLookup() => Class.FromType(typeof (Lookup)).isAssignableFrom(this.cls);

    public virtual bool isObject() => !this.isValue() && !this.isCollection();

    public virtual bool isValue() => Class.FromType(typeof (BusinessValueHolder)).isAssignableFrom(this.cls) || Class.FromType(typeof (BusinessValue)).isAssignableFrom(this.cls) || StringImpl.startsWith(this.cls.getName(), "java.") && !this.cls.isAssignableFrom(Class.FromType(typeof (Vector)));

    private void oneToManyAssociationFieldsArray(Vector associations)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", Class.FromType(typeof (object[])), 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-many association method ").append((object) get).ToString());
        string fieldName = JavaIntrospector.javaBaseName(get.getName());
        string name1 = new StringBuffer().append("about").append(fieldName).ToString();
        int length = 3;
        Class[] paramTypes = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        paramTypes[0] = Class.FromType(typeof (FieldAbout));
        paramTypes[1] = (Class) null;
        paramTypes[2] = (Class) Boolean.TYPE;
        Method about = this.findMethod(false, name1, (Class) null, paramTypes);
        Class parameterType1 = about?.getParameterTypes()[1];
        if (about == null)
          about = this.defaultAboutFieldMethod;
        Method add = (this.findMethod(false, new StringBuffer().append("addTo").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("add").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("associate").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null);
        Method remove = (this.findMethod(false, new StringBuffer().append("removeFrom").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("remove").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("dissociate").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null);
        Class parameterType2 = remove?.getParameterTypes()[0];
        Class parameterType3 = add?.getParameterTypes()[0];
        Class componentType = get.getReturnType().getComponentType();
        if (parameterType1 != null && parameterType1 != componentType || parameterType3 != null && parameterType3 != componentType || parameterType2 != null && parameterType2 != componentType)
          JavaIntrospector.LOG.error((object) new StringBuffer().append("The add/remove/associate/dissociate/about methods in ").append(this.className()).append(" must ").append("all deal with same type of object.  There are at least two different ").append("types").ToString());
        bool isHidden = false;
        bool isHiddenInTableViews = false;
        if (StringImpl.startsWith(fieldName, "Hidden"))
        {
          isHidden = true;
          isHiddenInTableViews = true;
          fieldName = StringImpl.substring(fieldName, StringImpl.length("Hidden"));
        }
        MemberIdentifier name2 = (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName);
        associations.addElement((object) new JavaOneToManyAssociation(name2, componentType, get, add, remove, about, isHidden, isHiddenInTableViews));
      }
    }

    private void oneToManyAssociationFieldsInternalCollection(Vector associations)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", Class.FromType(typeof (InternalCollection)), 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-many association method ").append((object) get).ToString());
        string fieldName = JavaIntrospector.javaBaseName(get.getName());
        string name = new StringBuffer().append("about").append(fieldName).ToString();
        int length = 3;
        Class[] paramTypes = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        paramTypes[0] = Class.FromType(typeof (FieldAbout));
        paramTypes[1] = (Class) null;
        paramTypes[2] = (Class) Boolean.TYPE;
        Method about = this.findMethod(false, name, (Class) null, paramTypes);
        Class parameterType1 = about?.getParameterTypes()[1];
        if (about == null)
          about = this.defaultAboutFieldMethod;
        Method add = (this.findMethod(false, new StringBuffer().append("addTo").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("add").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("associate").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null);
        Method remove = (this.findMethod(false, new StringBuffer().append("removeFrom").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("remove").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null)) ?? this.findMethod(false, new StringBuffer().append("dissociate").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null);
        Class parameterType2 = remove?.getParameterTypes()[0];
        Class parameterType3 = add?.getParameterTypes()[0];
        Class class1 = parameterType1 ?? (Class) null;
        Class class2 = parameterType3 ?? class1;
        Class type = parameterType2 ?? class2;
        if (parameterType1 != null && parameterType1 != type || parameterType3 != null && parameterType3 != type || parameterType2 != null && parameterType2 != type)
          JavaIntrospector.LOG.error((object) new StringBuffer().append("the add/remove/associate/dissociate/about methods in ").append(this.className()).append(" must ").append("all deal with same type of object.  There are at least two different ").append("types").ToString());
        bool isHidden = false;
        bool isHiddenInTableViews = false;
        if (StringImpl.startsWith(fieldName, "Hidden"))
        {
          isHidden = true;
          fieldName = StringImpl.substring(fieldName, StringImpl.length("Hidden"));
        }
        MemberIdentifier identifier = (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName);
        associations.addElement((object) new JavaInternalCollection(identifier, type, get, add, remove, about, isHidden, isHiddenInTableViews));
      }
    }

    private void oneToManyAssociationFieldsVector(Vector associations)
    {
      Vector prefixedMethods = this.findPrefixedMethods(false, "get", Class.FromType(typeof (Vector)), 0);
      bool flag = JavaIntrospector.LOG.isWarnEnabled();
      Enumeration enumeration = prefixedMethods.elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-many association method ").append((object) get).ToString());
        string fieldName = JavaIntrospector.javaBaseName(get.getName());
        string name1 = new StringBuffer().append("about").append(fieldName).ToString();
        int length1 = 3;
        Class[] paramTypes1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        paramTypes1[0] = Class.FromType(typeof (FieldAbout));
        paramTypes1[1] = (Class) null;
        paramTypes1[2] = (Class) Boolean.TYPE;
        Method about = this.findMethod(false, name1, (Class) null, paramTypes1);
        Class parameterType1 = about?.getParameterTypes()[1];
        if (about == null)
          about = this.defaultAboutFieldMethod;
        int length2 = 1;
        Class[] classArray = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
        classArray[0] = (Class) null;
        Class[] paramTypes2 = classArray;
        Method add = (this.findMethod(false, new StringBuffer().append("addTo").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2) ?? this.findMethod(false, new StringBuffer().append("add").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2)) ?? this.findMethod(false, new StringBuffer().append("associate").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2);
        Method remove = (this.findMethod(false, new StringBuffer().append("removeFrom").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2) ?? this.findMethod(false, new StringBuffer().append("remove").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2)) ?? this.findMethod(false, new StringBuffer().append("dissociate").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes2);
        if (add == null || remove == null)
        {
          JavaIntrospector.LOG.error((object) new StringBuffer().append("there must be both add and remove methods for ").append(fieldName).append(" in ").append(this.className()).ToString());
          break;
        }
        Class parameterType2 = remove?.getParameterTypes()[0];
        Class parameterType3 = add?.getParameterTypes()[0];
        Class class1 = parameterType1 ?? (Class) null;
        Class class2 = parameterType3 ?? class1;
        Class type = parameterType2 ?? class2;
        if (type == null)
        {
          if (!flag)
            break;
          JavaIntrospector.LOG.warn((object) new StringBuffer().append("cannot determine a type for the collection ").append(fieldName).append("; not added as a field").ToString());
          break;
        }
        if (parameterType1 != null && parameterType1 != type || parameterType3 != null && parameterType3 != type || parameterType2 != null && parameterType2 != type)
          JavaIntrospector.LOG.error((object) new StringBuffer().append("the add/remove/associate/dissociate/about methods in ").append(this.className()).append(" must ").append("all deal with same type of object.  There are at least two different ").append("types").ToString());
        bool isHidden = false;
        bool isHiddenInTableViews = false;
        if (StringImpl.startsWith(fieldName, "Hidden"))
        {
          isHidden = true;
          fieldName = StringImpl.substring(fieldName, StringImpl.length("Hidden"));
        }
        MemberIdentifier name2 = (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName);
        associations.addElement((object) new JavaOneToManyAssociation(name2, type, get, add, remove, about, isHidden, isHiddenInTableViews));
      }
    }

    [JavaThrownExceptions("1;org/nakedobjects/object/reflect/ReflectionException;")]
    private void oneToOneAssociationFields(Vector associations)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", Class.FromType(typeof (object)), 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified 1-1 association method ").append((object) get).ToString());
        if (!StringImpl.equals(get.getName(), (object) "getNakedClass"))
        {
          string fieldName = JavaIntrospector.javaBaseName(get.getName());
          int length1 = 1;
          Class[] classArray = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
          classArray[0] = get.getReturnType();
          Class[] paramTypes1 = classArray;
          string name = new StringBuffer().append("about").append(fieldName).ToString();
          int length2 = 2;
          Class[] paramTypes2 = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
          paramTypes2[0] = Class.FromType(typeof (FieldAbout));
          paramTypes2[1] = get.getReturnType();
          Method about = this.findMethod(false, name, (Class) null, paramTypes2) ?? this.defaultAboutFieldMethod;
          Method associate = this.findMethod(false, new StringBuffer().append("associate").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes1) ?? this.findMethod(false, new StringBuffer().append("add").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes1);
          Method dissociate = this.findMethod(false, new StringBuffer().append("dissociate").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null) ?? this.findMethod(false, new StringBuffer().append("remove").append(fieldName).ToString(), (Class) Void.TYPE, (Class[]) null);
          Method method = this.findMethod(false, new StringBuffer().append("set").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes1);
          bool isHidden = false;
          bool isHiddenInTableViews = false;
          if (StringImpl.startsWith(fieldName, "Hidden"))
          {
            isHidden = true;
            fieldName = StringImpl.substring(fieldName, StringImpl.length("Hidden"));
          }
          if (JavaIntrospector.LOG.isDebugEnabled())
            JavaIntrospector.LOG.debug((object) new StringBuffer().append("one-to-one association ").append(fieldName).append(" ->").append((object) associate).ToString());
          JavaOneToOneAssociation toOneAssociation = new JavaOneToOneAssociation(true, (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName), get.getReturnType(), get, method, associate, dissociate, about, isHidden, isHiddenInTableViews, method == null);
          associations.addElement((object) toOneAssociation);
        }
      }
    }

    [JavaFlags(4)]
    public virtual FieldPeer[] orderArray(FieldPeer[] original, string[] order)
    {
      if (order == null)
        return original;
      for (int index = 0; index < order.Length; ++index)
        order[index] = NameConvertor.simpleName(order[index]);
      int length1 = original.Length;
      FieldPeer[] fieldPeerArray1 = length1 >= 0 ? new FieldPeer[length1] : throw new NegativeArraySizeException();
      bool flag = JavaIntrospector.LOG.isWarnEnabled();
      int num1 = 0;
label_16:
      for (int index1 = 0; index1 < order.Length; ++index1)
      {
        for (int index2 = 0; index2 < original.Length; ++index2)
        {
          FieldPeer fieldPeer1 = original[index2];
          if (fieldPeer1 != null && StringImpl.equalsIgnoreCase(fieldPeer1.getIdentifier().getName(), order[index1]))
          {
            FieldPeer[] fieldPeerArray2 = fieldPeerArray1;
            int num2;
            num1 = (num2 = num1) + 1;
            int index3 = num2;
            FieldPeer fieldPeer2 = original[index2];
            fieldPeerArray2[index3] = fieldPeer2;
            original[index2] = (FieldPeer) null;
            goto label_16;
          }
        }
        if (!StringImpl.equals(StringImpl.trim(order[index1]), (object) "") && flag)
          JavaIntrospector.LOG.warn((object) new StringBuffer().append("invalid ordering element '").append(order[index1]).append("' in ").append(this.className).ToString());
      }
      int length2 = original.Length;
      FieldPeer[] fieldPeerArray3 = length2 >= 0 ? new FieldPeer[length2] : throw new NegativeArraySizeException();
      int num3 = 0;
      for (int index4 = 0; index4 < fieldPeerArray1.Length; ++index4)
      {
        FieldPeer fieldPeer3 = fieldPeerArray1[index4];
        if (fieldPeer3 != null)
        {
          FieldPeer[] fieldPeerArray4 = fieldPeerArray3;
          int num4;
          num3 = (num4 = num3) + 1;
          int index5 = num4;
          FieldPeer fieldPeer4 = fieldPeer3;
          fieldPeerArray4[index5] = fieldPeer4;
        }
      }
      for (int index6 = 0; index6 < original.Length; ++index6)
      {
        FieldPeer fieldPeer5 = original[index6];
        if (fieldPeer5 != null)
        {
          FieldPeer[] fieldPeerArray5 = fieldPeerArray3;
          int num5;
          num3 = (num5 = num3) + 1;
          int index7 = num5;
          FieldPeer fieldPeer6 = fieldPeer5;
          fieldPeerArray5[index7] = fieldPeer6;
        }
      }
      return fieldPeerArray3;
    }

    private Action getAction(Action[] actions, string name)
    {
      for (int index = 0; index < actions.Length; ++index)
      {
        Action action = actions[index];
        if (action != null && StringImpl.equals(action.getId(), (object) name))
        {
          actions[index] = (Action) null;
          return action;
        }
      }
      throw new NakedObjectRuntimeException(new StringBuffer().append("No action ").append(name).append("()").ToString());
    }

    [JavaFlags(4)]
    public virtual Action[] extractedOrderedActions(
      Action[] original,
      JavaIntrospector.Set order)
    {
      if (order == null)
      {
        int length = 0;
        return length >= 0 ? new Action[length] : throw new NegativeArraySizeException();
      }
      int length1 = order.elements.size();
      Action[] actionArray1 = length1 >= 0 ? new Action[length1] : throw new NegativeArraySizeException();
      Enumeration enumeration = order.elements.elements();
      int num1 = 0;
      while (enumeration.hasMoreElements())
      {
        object @object = enumeration.nextElement();
        if (\u003CVerifierFix\u003E.isInstanceOfString(@object))
        {
          Action[] actionArray2 = actionArray1;
          int num2;
          num1 = (num2 = num1) + 1;
          int index = num2;
          Action action = this.getAction(original, NameConvertor.simpleName(@object.ToString()));
          actionArray2[index] = action;
        }
        else
        {
          if (!(@object is JavaIntrospector.Set))
            throw new UnknownTypeException(@object);
          Action[] actionArray3 = actionArray1;
          int num3;
          num1 = (num3 = num1) + 1;
          int index = num3;
          ActionSet actionSet = new ActionSet("", ((JavaIntrospector.Set) @object).groupName, this.extractedOrderedActions(original, (JavaIntrospector.Set) @object));
          actionArray3[index] = (Action) actionSet;
        }
      }
      return actionArray1;
    }

    public virtual Persistable persistable() => Class.FromType(typeof (NonPersistable)).isAssignableFrom(this.cls) ? Persistable.TRANSIENT : Persistable.USER_PERSISTABLE;

    public virtual string pluralName()
    {
      try
      {
        Class cls = this.cls;
        int length = 0;
        Class[] classArray = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        return \u003CVerifierFix\u003E.genCastToString(cls.getMethod(nameof (pluralName), classArray).invoke((object) null, JavaIntrospector.NO_PARAMETERS));
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
      return NameConvertor.pluralName(this.singularName());
    }

    private void removeMethod(bool forClass, string name, Class returnType, Class[] paramTypes) => this.findMethod(forClass, name, returnType, paramTypes);

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
        int length = 0;
        Class[] classArray = length >= 0 ? new Class[length] : throw new NegativeArraySizeException();
        return \u003CVerifierFix\u003E.genCastToString(cls.getMethod(nameof (singularName), classArray).invoke((object) null, JavaIntrospector.NO_PARAMETERS));
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
      return NameConvertor.naturalName(this.shortName());
    }

    public virtual ObjectTitle title()
    {
      Method titleMethod = this.findMethod(false, nameof (title), Class.FromType(typeof (Title)), (Class[]) null) ?? this.findMethod(false, nameof (title), Class.FromType(typeof (string)), (Class[]) null);
      return titleMethod == null ? (ObjectTitle) new JavaIntrospector.\u0031(this) : (ObjectTitle) new JavaObjectTitle(titleMethod);
    }

    private Vector valueFields(Vector fields, Class type)
    {
      Enumeration enumeration = this.findPrefixedMethods(false, "get", type, 0).elements();
      while (enumeration.hasMoreElements())
      {
        Method get = (Method) enumeration.nextElement();
        Class returnType = get.getReturnType();
        bool flag = Class.FromType(typeof (BusinessValueHolder)).isAssignableFrom(returnType);
        string fieldName = JavaIntrospector.javaBaseName(get.getName());
        string name1 = new StringBuffer().append("set").append(fieldName).ToString();
        string name2 = new StringBuffer().append("about").append(fieldName).ToString();
        int length1 = 2;
        Class[] paramTypes1 = length1 >= 0 ? new Class[length1] : throw new NegativeArraySizeException();
        paramTypes1[0] = Class.FromType(typeof (FieldAbout));
        paramTypes1[1] = returnType;
        Method about = this.findMethod(false, name2, (Class) null, paramTypes1) ?? this.defaultAboutFieldMethod;
        string name3 = name1;
        int length2 = 1;
        Class[] paramTypes2 = length2 >= 0 ? new Class[length2] : throw new NegativeArraySizeException();
        paramTypes2[0] = returnType;
        Method method = this.findMethod(false, name3, (Class) null, paramTypes2);
        int length3 = 1;
        Class[] classArray = length3 >= 0 ? new Class[length3] : throw new NegativeArraySizeException();
        classArray[0] = returnType;
        Class[] paramTypes3 = classArray;
        if (this.findMethod(false, name1, (Class) Void.TYPE, paramTypes3) != null || this.findMethod(false, new StringBuffer().append("set_").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes3) != null)
          JavaIntrospector.LOG.error((object) new StringBuffer().append("the method set").append(fieldName).append(" is not needed for the NakedValue class ").append(this.className()).ToString());
        if (this.findMethod(false, new StringBuffer().append("associate").append(fieldName).ToString(), (Class) Void.TYPE, paramTypes3) != null)
          JavaIntrospector.LOG.error((object) new StringBuffer().append("the method associate").append(fieldName).append(" is not needed for the NakedValue class ").append(this.className()).ToString());
        bool isHidden = false;
        bool isHiddenInTableViews = false;
        if (StringImpl.startsWith(fieldName, "Hidden"))
        {
          isHidden = true;
          isHiddenInTableViews = true;
          fieldName = StringImpl.substring(fieldName, StringImpl.length("Hidden"));
        }
        if (JavaIntrospector.LOG.isDebugEnabled())
          JavaIntrospector.LOG.debug((object) new StringBuffer().append("  identified value ").append(fieldName).append(" -> ").append((object) get).ToString());
        JavaOneToOneAssociation toOneAssociation = new JavaOneToOneAssociation(false, (MemberIdentifier) new MemberIdentifierImpl(this.className, fieldName), get.getReturnType(), get, method, (Method) null, (Method) null, about, isHidden, isHiddenInTableViews, method == null && !flag);
        fields.addElement((object) toOneAssociation);
      }
      return fields;
    }

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static JavaIntrospector()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      JavaIntrospector javaIntrospector = this;
      ObjectImpl.clone((object) javaIntrospector);
      return ((object) javaIntrospector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(42)]
    private class Set
    {
      [JavaFlags(16)]
      public readonly Vector elements;
      [JavaFlags(16)]
      public readonly JavaIntrospector.Set parent;
      [JavaFlags(16)]
      public readonly string groupName;

      public Set()
      {
        this.elements = new Vector();
        this.parent = (JavaIntrospector.Set) null;
        this.groupName = "";
      }

      public Set(JavaIntrospector.Set set, string groupName, string element)
      {
        this.elements = new Vector();
        this.parent = set;
        this.parent.elements.addElement((object) this);
        this.groupName = groupName;
        this.add(element);
      }

      [JavaFlags(0)]
      public virtual JavaIntrospector.Set getParent() => this.parent;

      [JavaFlags(0)]
      public virtual void add(string element) => this.elements.addElement((object) element);

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        JavaIntrospector.Set set = this;
        ObjectImpl.clone((object) set);
        return ((object) set).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }

    [JavaFlags(32)]
    [JavaInterfaces("1;org/nakedobjects/object/reflect/ObjectTitle;")]
    [Inner]
    public class \u0031 : ObjectTitle
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private JavaIntrospector this\u00240;

      public virtual string title(NakedObject @object) => (string) null;

      public \u0031(JavaIntrospector _param1)
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
        JavaIntrospector.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
