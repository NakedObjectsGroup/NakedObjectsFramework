// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.DOMConfigurator
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
using org.w3c.dom;
using org.xml.sax;
using System.ComponentModel;
using System.Threading;

namespace org.apache.log4j.xml
{
  [JavaInterfaces("1;org/apache/log4j/spi/Configurator;")]
  public class DOMConfigurator : Configurator
  {
    [JavaFlags(24)]
    public const string CONFIGURATION_TAG = "log4j:configuration";
    [JavaFlags(24)]
    public const string OLD_CONFIGURATION_TAG = "configuration";
    [JavaFlags(24)]
    public const string RENDERER_TAG = "renderer";
    [JavaFlags(24)]
    public const string APPENDER_TAG = "appender";
    [JavaFlags(24)]
    public const string APPENDER_REF_TAG = "appender-ref";
    [JavaFlags(24)]
    public const string PARAM_TAG = "param";
    [JavaFlags(24)]
    public const string LAYOUT_TAG = "layout";
    [JavaFlags(24)]
    public const string CATEGORY = "category";
    [JavaFlags(24)]
    public const string LOGGER = "logger";
    [JavaFlags(24)]
    public const string LOGGER_REF = "logger-ref";
    [JavaFlags(24)]
    public const string CATEGORY_FACTORY_TAG = "categoryFactory";
    [JavaFlags(24)]
    public const string NAME_ATTR = "name";
    [JavaFlags(24)]
    public const string CLASS_ATTR = "class";
    [JavaFlags(24)]
    public const string VALUE_ATTR = "value";
    [JavaFlags(24)]
    public const string ROOT_TAG = "root";
    [JavaFlags(24)]
    public const string ROOT_REF = "root-ref";
    [JavaFlags(24)]
    public const string LEVEL_TAG = "level";
    [JavaFlags(24)]
    public const string PRIORITY_TAG = "priority";
    [JavaFlags(24)]
    public const string FILTER_TAG = "filter";
    [JavaFlags(24)]
    public const string ERROR_HANDLER_TAG = "errorHandler";
    [JavaFlags(24)]
    public const string REF_ATTR = "ref";
    [JavaFlags(24)]
    public const string ADDITIVITY_ATTR = "additivity";
    [JavaFlags(24)]
    public const string THRESHOLD_ATTR = "threshold";
    [JavaFlags(24)]
    public const string CONFIG_DEBUG_ATTR = "configDebug";
    [JavaFlags(24)]
    public const string INTERNAL_DEBUG_ATTR = "debug";
    [JavaFlags(24)]
    public const string RENDERING_CLASS_ATTR = "renderingClass";
    [JavaFlags(24)]
    public const string RENDERED_CLASS_ATTR = "renderedClass";
    [JavaFlags(24)]
    public const string EMPTY_STR = "";
    [JavaFlags(24)]
    public static readonly Class[] ONE_STRING_PARAM;
    [JavaFlags(24)]
    public const string dbfKey = "javax.xml.parsers.DocumentBuilderFactory";
    [JavaFlags(0)]
    public Hashtable appenderBag;
    [JavaFlags(0)]
    public Properties props;
    [JavaFlags(0)]
    public LoggerRepository repository;

    public DOMConfigurator() => this.appenderBag = new Hashtable();

    [JavaFlags(4)]
    public virtual Appender findAppenderByName(Document doc, string appenderName)
    {
      Appender appender1 = (Appender) this.appenderBag.get((object) appenderName);
      if (appender1 != null)
        return appender1;
      Element appenderElement = (Element) null;
      NodeList elementsByTagName = doc.getElementsByTagName("appender");
      for (int index = 0; index < elementsByTagName.getLength(); ++index)
      {
        Node node = elementsByTagName.item(index);
        Node namedItem = node.getAttributes().getNamedItem("name");
        if (StringImpl.equals(appenderName, (object) namedItem.getNodeValue()))
        {
          appenderElement = (Element) node;
          break;
        }
      }
      if (appenderElement == null)
      {
        LogLog.error(new StringBuffer().append("No appender named [").append(appenderName).append("] could be found.").ToString());
        return (Appender) null;
      }
      Appender appender2 = this.parseAppender(appenderElement);
      this.appenderBag.put((object) appenderName, (object) appender2);
      return appender2;
    }

    [JavaFlags(4)]
    public virtual Appender findAppenderByReference(Element appenderRef)
    {
      string appenderName = this.subst(appenderRef.getAttribute("ref"));
      return this.findAppenderByName(appenderRef.getOwnerDocument(), appenderName);
    }

    [JavaFlags(4)]
    public virtual Appender parseAppender(Element appenderElement)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void parseErrorHandler(Element element, Appender appender)
    {
      org.apache.log4j.spi.ErrorHandler errorHandler = (org.apache.log4j.spi.ErrorHandler) OptionConverter.instantiateByClassName(this.subst(element.getAttribute("class")), Class.FromType(typeof (org.apache.log4j.spi.ErrorHandler)), (object) null);
      if (errorHandler == null)
        return;
      errorHandler.setAppender(appender);
      PropertySetter propSetter = new PropertySetter((object) errorHandler);
      NodeList childNodes = element.getChildNodes();
      int length = childNodes.getLength();
      for (int index = 0; index < length; ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 1)
        {
          Element element1 = (Element) node;
          string tagName = element1.getTagName();
          if (StringImpl.equals(tagName, (object) "param"))
            this.setParameter(element1, propSetter);
          else if (StringImpl.equals(tagName, (object) "appender-ref"))
            errorHandler.setBackupAppender(this.findAppenderByReference(element1));
          else if (StringImpl.equals(tagName, (object) "logger-ref"))
          {
            Logger logger = this.repository.getLogger(element1.getAttribute("ref"));
            errorHandler.setLogger(logger);
          }
          else if (StringImpl.equals(tagName, (object) "root-ref"))
          {
            Logger rootLogger = this.repository.getRootLogger();
            errorHandler.setLogger(rootLogger);
          }
        }
      }
      propSetter.activate();
      appender.setErrorHandler(errorHandler);
    }

    [JavaFlags(4)]
    public virtual void parseFilters(Element element, Appender appender)
    {
      Filter newFilter = (Filter) OptionConverter.instantiateByClassName(this.subst(element.getAttribute("class")), Class.FromType(typeof (Filter)), (object) null);
      if (newFilter == null)
        return;
      PropertySetter propSetter = new PropertySetter((object) newFilter);
      NodeList childNodes = element.getChildNodes();
      int length = childNodes.getLength();
      for (int index = 0; index < length; ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 1)
        {
          Element elem = (Element) node;
          if (StringImpl.equals(elem.getTagName(), (object) "param"))
            this.setParameter(elem, propSetter);
        }
      }
      propSetter.activate();
      LogLog.debug(new StringBuffer().append("Adding filter of type [").append((object) ObjectImpl.getClass((object) newFilter)).append("] to appender named [").append(appender.getName()).append("].").ToString());
      appender.addFilter(newFilter);
    }

    [JavaFlags(4)]
    public virtual void parseCategory(Element loggerElement)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void parseCategoryFactory(Element factoryElement)
    {
      string className = this.subst(factoryElement.getAttribute("class"));
      if (StringImpl.equals("", (object) className))
      {
        LogLog.error(new StringBuffer().append("Category Factory tag ").append("class").append(" attribute not found.").ToString());
        LogLog.debug("No Category Factory configured.");
      }
      else
      {
        LogLog.debug(new StringBuffer().append("Desired category factory: [").append(className).append(']').ToString());
        PropertySetter propSetter = new PropertySetter(OptionConverter.instantiateByClassName(className, Class.FromType(typeof (LoggerFactory)), (object) null));
        NodeList childNodes = factoryElement.getChildNodes();
        int length = childNodes.getLength();
        for (int index = 0; index < length; ++index)
        {
          Node node = childNodes.item(index);
          if (node.getNodeType() == (short) 1)
          {
            Element elem = (Element) node;
            if (StringImpl.equals(elem.getTagName(), (object) "param"))
              this.setParameter(elem, propSetter);
          }
        }
      }
    }

    [JavaFlags(4)]
    public virtual void parseRoot(Element rootElement)
    {
      Logger rootLogger = this.repository.getRootLogger();
      object obj = (object) rootLogger;
      \u003CCorArrayWrapper\u003E.Enter(obj);
      try
      {
        this.parseChildrenOfLoggerElement(rootElement, rootLogger, true);
      }
      finally
      {
        Monitor.Exit(obj);
      }
    }

    [JavaFlags(4)]
    public virtual void parseChildrenOfLoggerElement(Element catElement, Logger cat, bool isRoot)
    {
      PropertySetter propSetter = new PropertySetter((object) cat);
      cat.removeAllAppenders();
      NodeList childNodes = catElement.getChildNodes();
      int length = childNodes.getLength();
      for (int index = 0; index < length; ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 1)
        {
          Element element = (Element) node;
          string tagName = element.getTagName();
          if (StringImpl.equals(tagName, (object) "appender-ref"))
          {
            Element appenderRef = (Element) node;
            Appender appenderByReference = this.findAppenderByReference(appenderRef);
            string str = this.subst(appenderRef.getAttribute("ref"));
            if (appenderByReference != null)
              LogLog.debug(new StringBuffer().append("Adding appender named [").append(str).append("] to category [").append(cat.getName()).append("].").ToString());
            else
              LogLog.debug(new StringBuffer().append("Appender named [").append(str).append("] not found.").ToString());
            cat.addAppender(appenderByReference);
          }
          else if (StringImpl.equals(tagName, (object) "level"))
            this.parseLevel(element, cat, isRoot);
          else if (StringImpl.equals(tagName, (object) "priority"))
            this.parseLevel(element, cat, isRoot);
          else if (StringImpl.equals(tagName, (object) "param"))
            this.setParameter(element, propSetter);
        }
      }
      propSetter.activate();
    }

    [JavaFlags(4)]
    public virtual Layout parseLayout(Element layout_element)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void parseRenderer(Element element)
    {
      string renderingClassName = this.subst(element.getAttribute("renderingClass"));
      string renderedClassName = this.subst(element.getAttribute("renderedClass"));
      if (!(this.repository is RendererSupport))
        return;
      RendererMap.addRenderer((RendererSupport) this.repository, renderedClassName, renderingClassName);
    }

    [JavaFlags(4)]
    public virtual void parseLevel(Element element, Logger logger, bool isRoot)
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4)]
    public virtual void setParameter(Element elem, PropertySetter propSetter)
    {
      string name = this.subst(elem.getAttribute("name"));
      string str = this.subst(OptionConverter.convertSpecialChars(elem.getAttribute("value")));
      propSetter.setProperty(name, str);
    }

    public static void configure(Element element) => new DOMConfigurator().doConfigure(element, LogManager.getLoggerRepository());

    public static void configureAndWatch(string configFilename) => DOMConfigurator.configureAndWatch(configFilename, 60000L);

    public static void configureAndWatch(string configFilename, long delay)
    {
      XMLWatchdog xmlWatchdog = new XMLWatchdog(configFilename);
      xmlWatchdog.setDelay(delay);
      xmlWatchdog.start();
    }

    public virtual void doConfigure(string filename, LoggerRepository repository)
    {
      FileInputStream fileInputStream = (FileInputStream) null;
      try
      {
        fileInputStream = new FileInputStream(filename);
        this.doConfigure((InputStream) fileInputStream, repository);
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not open [").append(filename).append("].").ToString(), (Throwable) ex);
      }
      finally
      {
        if (fileInputStream != null)
        {
          try
          {
            fileInputStream.close();
          }
          catch (IOException ex)
          {
            LogLog.error(new StringBuffer().append("Could not close [").append(filename).append("].").ToString(), (Throwable) ex);
          }
        }
      }
    }

    public virtual void doConfigure(URL url, LoggerRepository repository)
    {
      try
      {
        this.doConfigure(url.openStream(), repository);
      }
      catch (IOException ex)
      {
        LogLog.error(new StringBuffer().append("Could not open [").append((object) url).append("].").ToString(), (Throwable) ex);
      }
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public virtual void doConfigure(InputStream inputStream, LoggerRepository repository) => this.doConfigure(new InputSource(inputStream), repository);

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public virtual void doConfigure(Reader reader, LoggerRepository repository) => this.doConfigure(new InputSource(reader), repository);

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    [JavaFlags(4)]
    public virtual void doConfigure(InputSource inputSource, LoggerRepository repository)
    {
      // ISSUE: unable to decompile the method.
    }

    public virtual void doConfigure(Element element, LoggerRepository repository)
    {
      this.repository = repository;
      this.parse(element);
    }

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public static void configure(string filename) => new DOMConfigurator().doConfigure(filename, LogManager.getLoggerRepository());

    [JavaThrownExceptions("1;javax/xml/parsers/FactoryConfigurationError;")]
    public static void configure(URL url) => new DOMConfigurator().doConfigure(url, LogManager.getLoggerRepository());

    [JavaFlags(4)]
    public virtual void parse(Element element)
    {
      string tagName1 = element.getTagName();
      if (!StringImpl.equals(tagName1, (object) "log4j:configuration"))
      {
        if (StringImpl.equals(tagName1, (object) "configuration"))
        {
          LogLog.warn(new StringBuffer().append("The <").append("configuration").append("> element has been deprecated.").ToString());
          LogLog.warn(new StringBuffer().append("Use the <").append("log4j:configuration").append("> element instead.").ToString());
        }
        else
        {
          LogLog.error(new StringBuffer().append("DOM element is - not a <").append("log4j:configuration").append("> element.").ToString());
          return;
        }
      }
      string str1 = this.subst(element.getAttribute("debug"));
      LogLog.debug(new StringBuffer().append("debug attribute= \"").append(str1).append("\".").ToString());
      if (!StringImpl.equals(str1, (object) "") && !StringImpl.equals(str1, (object) "null"))
        LogLog.setInternalDebugging(OptionConverter.toBoolean(str1, true));
      else
        LogLog.debug(new StringBuffer().append("Ignoring ").append("debug").append(" attribute.").ToString());
      string str2 = this.subst(element.getAttribute("configDebug"));
      if (!StringImpl.equals(str2, (object) "") && !StringImpl.equals(str2, (object) "null"))
      {
        LogLog.warn(new StringBuffer().append("The \"").append("configDebug").append("\" attribute is deprecated.").ToString());
        LogLog.warn(new StringBuffer().append("Use the \"").append("debug").append("\" attribute instead.").ToString());
        LogLog.setInternalDebugging(OptionConverter.toBoolean(str2, true));
      }
      string val = this.subst(element.getAttribute("threshold"));
      LogLog.debug(new StringBuffer().append("Threshold =\"").append(val).append("\".").ToString());
      if (!StringImpl.equals("", (object) val) && !StringImpl.equals("null", (object) val))
        this.repository.setThreshold(val);
      NodeList childNodes = element.getChildNodes();
      int length = childNodes.getLength();
      for (int index = 0; index < length; ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 1)
        {
          Element factoryElement = (Element) node;
          if (StringImpl.equals(factoryElement.getTagName(), (object) "categoryFactory"))
            this.parseCategoryFactory(factoryElement);
        }
      }
      for (int index = 0; index < length; ++index)
      {
        Node node = childNodes.item(index);
        if (node.getNodeType() == (short) 1)
        {
          Element element1 = (Element) node;
          string tagName2 = element1.getTagName();
          if (StringImpl.equals(tagName2, (object) "category") || StringImpl.equals(tagName2, (object) "logger"))
            this.parseCategory(element1);
          else if (StringImpl.equals(tagName2, (object) "root"))
            this.parseRoot(element1);
          else if (StringImpl.equals(tagName2, (object) "renderer"))
            this.parseRenderer(element1);
        }
      }
    }

    [JavaFlags(4)]
    public virtual string subst(string value)
    {
      // ISSUE: unable to decompile the method.
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static DOMConfigurator()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DOMConfigurator domConfigurator = this;
      ObjectImpl.clone((object) domConfigurator);
      return ((object) domConfigurator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
