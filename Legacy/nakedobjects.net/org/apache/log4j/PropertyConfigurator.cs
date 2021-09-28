// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.PropertyConfigurator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.net;
using java.util;
using org.apache.log4j.config;
using org.apache.log4j.helpers;
using org.apache.log4j.or;
using org.apache.log4j.spi;
using System.Threading;

namespace org.apache.log4j
{
  [JavaInterfaces("1;org/apache/log4j/spi/Configurator;")]
  public class PropertyConfigurator : Configurator
  {
    [JavaFlags(4)]
    public Hashtable registry;
    [JavaFlags(4)]
    public LoggerFactory loggerFactory;
    [JavaFlags(24)]
    public const string CATEGORY_PREFIX = "log4j.category.";
    [JavaFlags(24)]
    public const string LOGGER_PREFIX = "log4j.logger.";
    [JavaFlags(24)]
    public const string FACTORY_PREFIX = "log4j.factory";
    [JavaFlags(24)]
    public const string ADDITIVITY_PREFIX = "log4j.additivity.";
    [JavaFlags(24)]
    public const string ROOT_CATEGORY_PREFIX = "log4j.rootCategory";
    [JavaFlags(24)]
    public const string ROOT_LOGGER_PREFIX = "log4j.rootLogger";
    [JavaFlags(24)]
    public const string APPENDER_PREFIX = "log4j.appender.";
    [JavaFlags(24)]
    public const string RENDERER_PREFIX = "log4j.renderer.";
    [JavaFlags(24)]
    public const string THRESHOLD_PREFIX = "log4j.threshold";
    public const string LOGGER_FACTORY_KEY = "log4j.loggerFactory";
    private const string INTERNAL_ROOT_NAME = "root";

    public virtual void doConfigure(string configFileName, LoggerRepository hierarchy)
    {
      Properties properties = new Properties();
      try
      {
        FileInputStream fileInputStream = new FileInputStream(configFileName);
        properties.load((InputStream) fileInputStream);
        fileInputStream.close();
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not read configuration file [").append(configFileName).append("].").ToString(), (Throwable) ex);
        LogLog.error(new StringBuffer().append("Ignoring configuration file [").append(configFileName).append("].").ToString());
        return;
      }
      this.doConfigure(properties, hierarchy);
    }

    public static void configure(string configFilename) => new PropertyConfigurator().doConfigure(configFilename, LogManager.getLoggerRepository());

    public static void configure(URL configURL) => new PropertyConfigurator().doConfigure(configURL, LogManager.getLoggerRepository());

    public static void configure(Properties properties) => new PropertyConfigurator().doConfigure(properties, LogManager.getLoggerRepository());

    public static void configureAndWatch(string configFilename) => PropertyConfigurator.configureAndWatch(configFilename, 60000L);

    public static void configureAndWatch(string configFilename, long delay)
    {
      PropertyWatchdog propertyWatchdog = new PropertyWatchdog(configFilename);
      propertyWatchdog.setDelay(delay);
      propertyWatchdog.start();
    }

    public virtual void doConfigure(Properties properties, LoggerRepository hierarchy)
    {
      string property = properties.getProperty("log4j.debug");
      if (property == null)
      {
        property = properties.getProperty("log4j.configDebug");
        if (property != null)
          LogLog.warn("[log4j.configDebug] is deprecated. Use [log4j.debug] instead.");
      }
      if (property != null)
        LogLog.setInternalDebugging(OptionConverter.toBoolean(property, true));
      string andSubst = OptionConverter.findAndSubst("log4j.threshold", properties);
      if (andSubst != null)
      {
        hierarchy.setThreshold(OptionConverter.toLevel(andSubst, Level.ALL));
        LogLog.debug(new StringBuffer().append("Hierarchy threshold set to [").append((object) hierarchy.getThreshold()).append("].").ToString());
      }
      this.configureRootCategory(properties, hierarchy);
      this.configureLoggerFactory(properties);
      this.parseCatsAndRenderers(properties, hierarchy);
      LogLog.debug("Finished configuring.");
      this.registry.clear();
    }

    public virtual void doConfigure(URL configURL, LoggerRepository hierarchy)
    {
      Properties properties = new Properties();
      LogLog.debug(new StringBuffer().append("Reading configuration from URL ").append((object) configURL).ToString());
      try
      {
        properties.load(configURL.openStream());
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not read configuration file from URL [").append((object) configURL).append("].").ToString(), (Throwable) ex);
        LogLog.error(new StringBuffer().append("Ignoring configuration file [").append((object) configURL).append("].").ToString());
        return;
      }
      this.doConfigure(properties, hierarchy);
    }

    [JavaFlags(4)]
    public virtual void configureLoggerFactory(Properties props)
    {
      string andSubst = OptionConverter.findAndSubst("log4j.loggerFactory", props);
      if (andSubst == null)
        return;
      LogLog.debug(new StringBuffer().append("Setting category factory to [").append(andSubst).append("].").ToString());
      this.loggerFactory = (LoggerFactory) OptionConverter.instantiateByClassName(andSubst, Class.FromType(typeof (LoggerFactory)), (object) this.loggerFactory);
      PropertySetter.setProperties((object) this.loggerFactory, props, new StringBuffer().append("log4j.factory").append(".").ToString());
    }

    [JavaFlags(0)]
    public virtual void configureRootCategory(Properties props, LoggerRepository hierarchy)
    {
      string optionKey = "log4j.rootLogger";
      string andSubst = OptionConverter.findAndSubst("log4j.rootLogger", props);
      if (andSubst == null)
      {
        andSubst = OptionConverter.findAndSubst("log4j.rootCategory", props);
        optionKey = "log4j.rootCategory";
      }
      if (andSubst == null)
      {
        LogLog.debug("Could not find root logger information. Is this OK?");
      }
      else
      {
        Logger rootLogger = hierarchy.getRootLogger();
        object obj = (object) rootLogger;
        \u003CCorArrayWrapper\u003E.Enter(obj);
        try
        {
          this.parseCategory(props, rootLogger, optionKey, "root", andSubst);
        }
        finally
        {
          Monitor.Exit(obj);
        }
      }
    }

    [JavaFlags(4)]
    public virtual void parseCatsAndRenderers(Properties props, LoggerRepository hierarchy)
    {
      Enumeration enumeration = props.propertyNames();
      while (enumeration.hasMoreElements())
      {
        string str1 = \u003CVerifierFix\u003E.genCastToString(enumeration.nextElement());
        if (StringImpl.startsWith(str1, "log4j.category.") || StringImpl.startsWith(str1, "log4j.logger."))
        {
          string str2 = (string) null;
          if (StringImpl.startsWith(str1, "log4j.category."))
            str2 = StringImpl.substring(str1, StringImpl.length("log4j.category."));
          else if (StringImpl.startsWith(str1, "log4j.logger."))
            str2 = StringImpl.substring(str1, StringImpl.length("log4j.logger."));
          string andSubst = OptionConverter.findAndSubst(str1, props);
          Logger logger = hierarchy.getLogger(str2, this.loggerFactory);
          object obj = (object) logger;
          \u003CCorArrayWrapper\u003E.Enter(obj);
          try
          {
            this.parseCategory(props, logger, str1, str2, andSubst);
            this.parseAdditivityForLogger(props, logger, str2);
          }
          finally
          {
            Monitor.Exit(obj);
          }
        }
        else if (StringImpl.startsWith(str1, "log4j.renderer."))
        {
          string renderedClassName = StringImpl.substring(str1, StringImpl.length("log4j.renderer."));
          string andSubst = OptionConverter.findAndSubst(str1, props);
          if (hierarchy is RendererSupport)
            RendererMap.addRenderer((RendererSupport) hierarchy, renderedClassName, andSubst);
        }
      }
    }

    [JavaFlags(0)]
    public virtual void parseAdditivityForLogger(Properties props, Logger cat, string loggerName)
    {
      string andSubst = OptionConverter.findAndSubst(new StringBuffer().append("log4j.additivity.").append(loggerName).ToString(), props);
      LogLog.debug(new StringBuffer().append("Handling ").append("log4j.additivity.").append(loggerName).append("=[").append(andSubst).append("]").ToString());
      if (andSubst == null || StringImpl.equals(andSubst, (object) ""))
        return;
      bool boolean = OptionConverter.toBoolean(andSubst, true);
      LogLog.debug(new StringBuffer().append("Setting additivity for \"").append(loggerName).append("\" to ").append(boolean).ToString());
      cat.setAdditivity(boolean);
    }

    [JavaFlags(0)]
    public virtual void parseCategory(
      Properties props,
      Logger logger,
      string optionKey,
      string loggerName,
      string value)
    {
      LogLog.debug(new StringBuffer().append("Parsing for [").append(loggerName).append("] with value=[").append(value).append("].").ToString());
      StringTokenizer stringTokenizer = new StringTokenizer(value, ",");
      if (!StringImpl.startsWith(value, ",") && !StringImpl.equals(value, (object) ""))
      {
        if (!stringTokenizer.hasMoreTokens())
          return;
        string str = stringTokenizer.nextToken();
        LogLog.debug(new StringBuffer().append("Level token is [").append(str).append("].").ToString());
        if (StringImpl.equalsIgnoreCase("inherited", str) || StringImpl.equalsIgnoreCase("null", str))
        {
          if (StringImpl.equals(loggerName, (object) "root"))
            LogLog.warn("The root logger cannot be set to null.");
          else
            logger.setLevel((Level) null);
        }
        else
          logger.setLevel(OptionConverter.toLevel(str, Level.DEBUG));
        LogLog.debug(new StringBuffer().append("Category ").append(loggerName).append(" set to ").append((object) logger.getLevel()).ToString());
      }
      logger.removeAllAppenders();
      while (stringTokenizer.hasMoreTokens())
      {
        string appenderName = StringImpl.trim(stringTokenizer.nextToken());
        if (appenderName != null && !StringImpl.equals(appenderName, (object) ","))
        {
          LogLog.debug(new StringBuffer().append("Parsing appender named \"").append(appenderName).append("\".").ToString());
          Appender appender = this.parseAppender(props, appenderName);
          if (appender != null)
            logger.addAppender(appender);
        }
      }
    }

    [JavaFlags(0)]
    public virtual Appender parseAppender(Properties props, string appenderName)
    {
      Appender appender1 = this.registryGet(appenderName);
      if (appender1 != null)
      {
        LogLog.debug(new StringBuffer().append("Appender \"").append(appenderName).append("\" was already parsed.").ToString());
        return appender1;
      }
      string key1 = new StringBuffer().append("log4j.appender.").append(appenderName).ToString();
      string key2 = new StringBuffer().append(key1).append(".layout").ToString();
      Appender appender2 = (Appender) OptionConverter.instantiateByKey(props, key1, Class.FromType(typeof (Appender)), (object) null);
      if (appender2 == null)
      {
        LogLog.error(new StringBuffer().append("Could not instantiate appender named \"").append(appenderName).append("\".").ToString());
        return (Appender) null;
      }
      appender2.setName(appenderName);
      if (appender2 is OptionHandler)
      {
        if (appender2.requiresLayout())
        {
          Layout layout = (Layout) OptionConverter.instantiateByKey(props, key2, Class.FromType(typeof (Layout)), (object) null);
          if (layout != null)
          {
            appender2.setLayout(layout);
            LogLog.debug(new StringBuffer().append("Parsing layout options for \"").append(appenderName).append("\".").ToString());
            PropertySetter.setProperties((object) layout, props, new StringBuffer().append(key2).append(".").ToString());
            LogLog.debug(new StringBuffer().append("End of parsing for \"").append(appenderName).append("\".").ToString());
          }
        }
        PropertySetter.setProperties((object) appender2, props, new StringBuffer().append(key1).append(".").ToString());
        LogLog.debug(new StringBuffer().append("Parsed \"").append(appenderName).append("\" options.").ToString());
      }
      this.registryPut(appender2);
      return appender2;
    }

    [JavaFlags(0)]
    public virtual void registryPut(Appender appender) => this.registry.put((object) appender.getName(), (object) appender);

    [JavaFlags(0)]
    public virtual Appender registryGet(string name) => (Appender) this.registry.get((object) name);

    public PropertyConfigurator()
    {
      this.registry = new Hashtable(11);
      this.loggerFactory = (LoggerFactory) new DefaultCategoryFactory();
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      PropertyConfigurator propertyConfigurator = this;
      ObjectImpl.clone((object) propertyConfigurator);
      return ((object) propertyConfigurator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
