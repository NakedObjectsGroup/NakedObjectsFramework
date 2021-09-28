// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.system.StartUp
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.lang;
using java.util;
using org.apache.log4j;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.utility.configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace org.nakedobjects.system
{
  public class StartUp
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly Hashtable components;

    public static void main(string[] args)
    {
      // ISSUE: type reference
      RuntimeHelpers.RunClassConstructor(__typeref (ObjectImpl));
      new StartUp().start(args.Length <= 0 ? "nakedobjects.properties" : args[0]);
      Utilities.cleanupAfterMainReturns();
    }

    [JavaFlags(4)]
    public virtual void start(string name)
    {
      LogManager.getRootLogger().setLevel(Level.OFF);
      PropertiesConfiguration propertiesConfiguration = new PropertiesConfiguration((ConfigurationLoader) new PropertiesFileLoader(name, true));
      PropertyConfigurator.configure(propertiesConfiguration.getProperties("log4j"));
      AboutNakedObjects.logVersion();
      if (StartUp.LOG.isDebugEnabled())
        StartUp.LOG.debug((object) new StringBuffer().append("Configuring system using ").append(name).ToString());
      StringTokenizer stringTokenizer = new StringTokenizer(propertiesConfiguration.getString("nakedobjects.components"), ",;/:");
      Properties properties = propertiesConfiguration.getProperties("nakedobjects");
      Vector vector = new Vector();
      while (stringTokenizer.hasMoreTokens())
      {
        string str = StringImpl.trim(stringTokenizer.nextToken());
        string property = properties.getProperty(new StringBuffer().append("nakedobjects.").append(str).ToString());
        if (StartUp.LOG.isDebugEnabled())
          StartUp.LOG.debug((object) new StringBuffer().append("loading core component ").append(str).append(": ").append(property).ToString());
        NakedObjectsComponent objectsComponent = property != null ? (NakedObjectsComponent) StartUp.loadComponent(property, Class.FromType(typeof (NakedObjectsComponent))) : throw new StartupException(new StringBuffer().append("No component specified for nakedobjects.").append(str).ToString());
        if (objectsComponent is NakedObjects)
          ((NakedObjects) objectsComponent).setConfiguration((NakedObjectConfiguration) propertiesConfiguration);
        this.setProperties((object) objectsComponent, new StringBuffer().append("nakedobjects.").append(str).ToString(), properties);
        objectsComponent.init();
      }
      Enumeration enumeration = vector.elements();
      while (enumeration.hasMoreElements())
        ((NakedObjectsComponent) enumeration.nextElement()).init();
    }

    private void setProperties(object @object, string prefix, Properties properties)
    {
      if (StartUp.LOG.isDebugEnabled())
        StartUp.LOG.debug((object) new StringBuffer().append("looking for properties starting with ").append(prefix).ToString());
      Enumeration enumeration = properties.propertyNames();
      while (enumeration.hasMoreElements())
      {
        string str = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(str, prefix) && StringImpl.length(str) > StringImpl.length(prefix) && StringImpl.indexOf(StringImpl.substring(str, StringImpl.length(prefix) + 1), 46) == -1)
        {
          if (StartUp.LOG.isDebugEnabled())
            StartUp.LOG.debug((object) new StringBuffer().append("  property ").append(str).append(" (of prefix ").append(prefix).append(")").ToString());
          string className1 = StringImpl.trim(properties.getProperty(str));
          object obj1;
          if (StringImpl.indexOf(className1, 44) > 0)
          {
            StringTokenizer stringTokenizer = new StringTokenizer(className1, ",");
            int length = stringTokenizer.countTokens();
            object[] objArray1 = length >= 0 ? new object[length] : throw new NegativeArraySizeException();
            int num1 = 0;
            while (stringTokenizer.hasMoreTokens())
            {
              string className2 = stringTokenizer.nextToken();
              object[] objArray2 = objArray1;
              int num2;
              num1 = (num2 = num1) + 1;
              int index = num2;
              object obj2 = this.load(className2, str, properties);
              objArray2[index] = obj2;
            }
            obj1 = (object) objArray1;
          }
          else
            obj1 = !StringImpl.equalsIgnoreCase(className1, "true") ? (!StringImpl.equalsIgnoreCase(className1, "false") ? this.load(className1, str, properties) : (object) Boolean.FALSE) : (object) Boolean.TRUE;
          this.setProperty(@object, str, obj1);
        }
      }
    }

    private void setProperty(object @object, string fieldName, object value)
    {
      // ISSUE: unable to decompile the method.
    }

    private object load(string className, string name, Properties properties)
    {
      if (StringImpl.startsWith(className, "ref:"))
      {
        string str = StringImpl.substring(className, 4);
        return this.components.containsKey((object) str) ? this.components.get((object) str) : throw new StartupException(new StringBuffer().append("Could not reference the object names ").append(str).ToString());
      }
      if (StartUp.LOG.isDebugEnabled())
        StartUp.LOG.debug((object) new StringBuffer().append("loading component ").append(className).append(" for ").append(name).ToString());
      object @object = StartUp.loadComponent(className);
      this.components.put((object) name, @object);
      this.setProperties(@object, name, properties);
      return @object;
    }

    public static object loadComponent(string className) => StartUp.loadNamedComponent(className, (Class) null, (Class) null);

    public static object loadComponent(string className, Class requiredClass) => StartUp.loadNamedComponent(className, (Class) null, requiredClass);

    public static object loadNamedComponent(
      string className,
      Class defaultType,
      Class requiredClass)
    {
      Class @class = (Class) null;
      try
      {
        @class = Class.forName(className);
        return requiredClass == null || requiredClass.isAssignableFrom(@class) ? @class.newInstance() : throw new StartupException(new StringBuffer().append("Component class ").append(className).append(" must be of the type ").append((object) requiredClass).ToString());
      }
      catch (ClassNotFoundException ex)
      {
        throw new StartupException(new StringBuffer().append("The component class ").append(className).append(" can not be found").ToString());
      }
      catch (InstantiationException ex)
      {
        throw new StartupException(new StringBuffer().append("Could not instantiate an object of class ").append(@class.getName()).append("; ").append(((Throwable) ex).getMessage()).ToString());
      }
      catch (IllegalAccessException ex)
      {
        throw new StartupException(new StringBuffer().append("Could not access the class ").append(@class.getName()).append("; ").append(((Throwable) ex).getMessage()).ToString());
      }
    }

    public StartUp() => this.components = new Hashtable();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static StartUp()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      StartUp startUp = this;
      ObjectImpl.clone((object) startUp);
      return ((object) startUp).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
