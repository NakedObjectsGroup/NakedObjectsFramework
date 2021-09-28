// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.configuration.ComponentLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.utility.configuration
{
  public class ComponentLoader
  {
    [JavaThrownExceptions("1;org/nakedobjects/utility/configuration/ConfigurationException;")]
    public static object loadComponent(string className, Class requiredClass) => ComponentLoader.loadComponent(className, (Class) null, requiredClass);

    [JavaThrownExceptions("1;org/nakedobjects/utility/configuration/ConfigurationException;")]
    public static object loadComponent(string className, Class defaultType, Class requiredClass)
    {
      Class @class = (Class) null;
      try
      {
        @class = Class.forName(className);
        return requiredClass.isAssignableFrom(@class) ? @class.newInstance() : throw new ConfigurationException(new StringBuffer().append("Component class ").append(className).append(" must be of the type ").append((object) requiredClass).ToString());
      }
      catch (ClassNotFoundException ex)
      {
        throw new ConfigurationException(new StringBuffer().append("The component class ").append(className).append(" can not be found").ToString());
      }
      catch (InstantiationException ex)
      {
        throw new ConfigurationException(new StringBuffer().append("Could not instantiate an object of class ").append(@class.getName()).append("; ").append(((Throwable) ex).getMessage()).ToString());
      }
      catch (IllegalAccessException ex)
      {
        throw new ConfigurationException(new StringBuffer().append("Could not access the class ").append(@class.getName()).append("; ").append(((Throwable) ex).getMessage()).ToString());
      }
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ComponentLoader componentLoader = this;
      ObjectImpl.clone((object) componentLoader);
      return ((object) componentLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
