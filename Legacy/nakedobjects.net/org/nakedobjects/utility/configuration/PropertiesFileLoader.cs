// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.configuration.PropertiesFileLoader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;

namespace org.nakedobjects.utility.configuration
{
  [JavaInterfaces("1;org/nakedobjects/utility/configuration/ConfigurationLoader;")]
  public class PropertiesFileLoader : ConfigurationLoader
  {
    private Properties properties;

    public PropertiesFileLoader(string pathname, bool ensureFileLoads)
    {
      this.properties = new Properties();
      try
      {
        this.properties.load((InputStream) new FileInputStream(new File(pathname)));
      }
      catch (FileNotFoundException ex)
      {
        if (ensureFileLoads)
          throw new NakedObjectRuntimeException(new StringBuffer().append("Could not find configuration file: ").append(pathname).ToString(), (Throwable) ex);
      }
      catch (IOException ex)
      {
        throw new NakedObjectRuntimeException(new StringBuffer().append("Could not load configuration file: ").append(pathname).ToString(), (Throwable) ex);
      }
    }

    public virtual Properties getProperties() => this.properties;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      PropertiesFileLoader propertiesFileLoader = this;
      ObjectImpl.clone((object) propertiesFileLoader);
      return ((object) propertiesFileLoader).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
