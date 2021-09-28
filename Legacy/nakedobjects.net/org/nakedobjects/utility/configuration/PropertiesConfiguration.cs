// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.configuration.PropertiesConfiguration
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt;
using java.lang;
using java.util;
using System.ComponentModel;

namespace org.nakedobjects.utility.configuration
{
  [JavaInterfaces("1;org/nakedobjects/utility/NakedObjectConfiguration;")]
  public class PropertiesConfiguration : NakedObjectConfiguration
  {
    private static readonly org.apache.log4j.Logger LOG;
    private readonly Properties p;

    public PropertiesConfiguration() => this.p = new Properties();

    public PropertiesConfiguration(ConfigurationLoader loader)
    {
      this.p = new Properties();
      this.add(loader.getProperties());
    }

    public virtual void add(Properties properties)
    {
      Enumeration enumeration = properties.propertyNames();
      while (enumeration.hasMoreElements())
      {
        string str = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        ((Hashtable) this.p).put((object) str, (object) properties.getProperty(str));
      }
    }

    public virtual void add(string key, string value) => ((Hashtable) this.p).put((object) key, (object) value);

    public virtual NakedObjectConfiguration createSubset(string prefix)
    {
      PropertiesConfiguration propertiesConfiguration = new PropertiesConfiguration();
      if (!StringImpl.endsWith(prefix, "."))
        prefix = new StringBuffer().append(prefix).append('.').ToString();
      int num = StringImpl.length(prefix);
      Enumeration enumeration = ((Hashtable) this.p).keys();
      while (enumeration.hasMoreElements())
      {
        string str1 = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(str1, prefix))
        {
          string str2 = StringImpl.substring(str1, num);
          ((Hashtable) propertiesConfiguration.p).put((object) str2, ((Hashtable) this.p).get((object) str1));
        }
      }
      return (NakedObjectConfiguration) propertiesConfiguration;
    }

    public virtual bool getBoolean(string name) => this.getBoolean(name, false);

    public virtual bool getBoolean(string name, bool defaultValue)
    {
      string property = this.getProperty(name);
      if (property == null)
        return defaultValue;
      string lowerCase = StringImpl.toLowerCase(property);
      if (StringImpl.equals(lowerCase, (object) "on") || StringImpl.equals(lowerCase, (object) "yes") || StringImpl.equals(lowerCase, (object) "true") || StringImpl.equals(lowerCase, (object) ""))
        return true;
      if (StringImpl.equals(lowerCase, (object) "off") || StringImpl.equals(lowerCase, (object) "no") || StringImpl.equals(lowerCase, (object) "false"))
        return false;
      throw new ConfigurationException("Illegal flag for name; must be one of on, off, yes, no, true or false");
    }

    public virtual Color getColor(string name) => this.getColor(name, (Color) null);

    public virtual Color getColor(string name, Color defaultValue)
    {
      string property = this.getProperty(name);
      return property == null ? defaultValue : Color.decode(property);
    }

    public virtual void debugData(DebugString debug)
    {
      Enumeration enumeration = this.p.propertyNames();
      DebugString debugString = new DebugString();
      while (enumeration.hasMoreElements())
      {
        string str = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        debugString.append((object) str, 55);
        debugString.append((object) " = ");
        debugString.appendln(this.p.getProperty(str));
      }
    }

    public virtual string getDebugTitle() => "Properties Configuration";

    public virtual Font getFont(string name) => this.getFont(name, (Font) null);

    public virtual Font getFont(string name, Font defaultValue)
    {
      string property = this.getProperty(name);
      return property == null ? defaultValue : Font.decode(property);
    }

    public virtual int getInteger(string name) => this.getInteger(name, 0);

    public virtual int getInteger(string name, int defaultValue)
    {
      string property = this.getProperty(name);
      return property == null ? defaultValue : Integer.valueOf(property).intValue();
    }

    public virtual Properties getProperties(string withPrefix) => this.getProperties(withPrefix, "");

    private Properties getProperties(string withPrefix, string stripPrefix)
    {
      int num = StringImpl.length(stripPrefix);
      Properties properties = new Properties();
      Enumeration enumeration = ((Hashtable) this.p).keys();
      while (enumeration.hasMoreElements())
      {
        string str1 = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(str1, withPrefix))
        {
          string str2 = StringImpl.substring(str1, num);
          ((Hashtable) properties).put((object) str2, ((Hashtable) this.p).get((object) str1));
        }
      }
      return properties;
    }

    private string getProperty(string name) => this.getProperty(name, (string) null);

    private string getProperty(string name, string defaultValue)
    {
      string str = this.referedToAs(name);
      if (StringImpl.indexOf(str, "..") >= 0)
        throw new NakedObjectRuntimeException(new StringBuffer().append("property names should not have '..' within them: ").append(name).ToString());
      string property = this.p.getProperty(str, defaultValue);
      if (PropertiesConfiguration.LOG.isDebugEnabled())
        PropertiesConfiguration.LOG.debug((object) new StringBuffer().append("property: ").append(str).append(" =  <").append(property).append(">").ToString());
      return property;
    }

    public virtual string getString(string name) => this.getProperty(name);

    public virtual string getString(string name, string defaultValue) => this.getProperty(name, defaultValue);

    public virtual bool hasProperty(string name) => ((Hashtable) this.p).containsKey((object) this.referedToAs(name));

    public virtual bool isEmpty() => ((Hashtable) this.p).isEmpty();

    public virtual Enumeration properties() => ((Hashtable) this.p).keys();

    public virtual string referedToAs(string name) => name;

    public virtual int size() => ((Hashtable) this.p).size();

    public override string ToString() => new StringBuffer().append("ConfigurationParameters [properties=").append((object) this.p).append("]").ToString();

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static PropertiesConfiguration()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PropertiesConfiguration propertiesConfiguration = this;
      ObjectImpl.clone((object) propertiesConfiguration);
      return ((object) propertiesConfiguration).MemberwiseClone();
    }
  }
}
