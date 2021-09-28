// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Hierarchy
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.or;
using org.apache.log4j.spi;
using System;
using System.Threading;

namespace org.apache.log4j
{
  [JavaInterfaces("2;org/apache/log4j/spi/LoggerRepository;org/apache/log4j/spi/RendererSupport;")]
  public class Hierarchy : LoggerRepository, RendererSupport
  {
    private LoggerFactory defaultFactory;
    private Vector listeners;
    [JavaFlags(0)]
    public Hashtable ht;
    [JavaFlags(0)]
    public Logger root;
    [JavaFlags(0)]
    public RendererMap rendererMap;
    [JavaFlags(0)]
    public int thresholdInt;
    [JavaFlags(0)]
    public Level threshold;
    [JavaFlags(0)]
    public bool emittedNoAppenderWarning;
    [JavaFlags(0)]
    public bool emittedNoResourceBundleWarning;

    public Hierarchy(Logger root)
    {
      this.emittedNoAppenderWarning = false;
      this.emittedNoResourceBundleWarning = false;
      this.ht = new Hashtable();
      this.listeners = new Vector(1);
      this.root = root;
      this.setThreshold(Level.ALL);
      this.root.setHierarchy((LoggerRepository) this);
      this.rendererMap = new RendererMap();
      this.defaultFactory = (LoggerFactory) new DefaultCategoryFactory();
    }

    public virtual void addRenderer(Class classToRender, ObjectRenderer or) => this.rendererMap.put(classToRender, or);

    public virtual void addHierarchyEventListener(HierarchyEventListener listener)
    {
      if (this.listeners.contains((object) listener))
        LogLog.warn("Ignoring attempt to add an existent listener.");
      else
        this.listeners.addElement((object) listener);
    }

    public virtual void clear() => this.ht.clear();

    public virtual void emitNoAppenderWarning(Category cat)
    {
      if (this.emittedNoAppenderWarning)
        return;
      LogLog.warn(new StringBuffer().append("No appenders could be found for logger (").append(cat.getName()).append(").").ToString());
      LogLog.warn("Please initialize the log4j system properly.");
      this.emittedNoAppenderWarning = true;
    }

    public virtual Logger exists(string name)
    {
      object obj = this.ht.get((object) new CategoryKey(name));
      return obj is Logger ? (Logger) obj : (Logger) null;
    }

    public virtual void setThreshold(string levelStr)
    {
      Level level = Level.toLevel(levelStr, (Level) null);
      if (level != null)
        this.setThreshold(level);
      else
        LogLog.warn(new StringBuffer().append("Could not convert [").append(levelStr).append("] to Level.").ToString());
    }

    public virtual void setThreshold(Level l)
    {
      if (l == null)
        return;
      this.thresholdInt = l.level;
      this.threshold = l;
    }

    public virtual void fireAddAppenderEvent(Category logger, Appender appender)
    {
      if (this.listeners == null)
        return;
      int num = this.listeners.size();
      for (int index = 0; index < num; ++index)
        ((HierarchyEventListener) this.listeners.elementAt(index)).addAppenderEvent(logger, appender);
    }

    [JavaFlags(0)]
    public virtual void fireRemoveAppenderEvent(Category logger, Appender appender)
    {
      if (this.listeners == null)
        return;
      int num = this.listeners.size();
      for (int index = 0; index < num; ++index)
        ((HierarchyEventListener) this.listeners.elementAt(index)).removeAppenderEvent(logger, appender);
    }

    public virtual Level getThreshold() => this.threshold;

    public virtual Logger getLogger(string name) => this.getLogger(name, this.defaultFactory);

    public virtual Logger getLogger(string name, LoggerFactory factory)
    {
      CategoryKey categoryKey = new CategoryKey(name);
      object ht = (object) this.ht;
      \u003CCorArrayWrapper\u003E.Enter(ht);
      try
      {
        object obj = this.ht.get((object) categoryKey);
        switch (obj)
        {
          case null:
            Logger cat = factory.makeNewLoggerInstance(name);
            cat.setHierarchy((LoggerRepository) this);
            this.ht.put((object) categoryKey, (object) cat);
            this.updateParents(cat);
            return cat;
          case Logger _:
            return (Logger) obj;
          case ProvisionNode _:
            Logger logger = factory.makeNewLoggerInstance(name);
            logger.setHierarchy((LoggerRepository) this);
            this.ht.put((object) categoryKey, (object) logger);
            this.updateChildren((ProvisionNode) obj, logger);
            this.updateParents(logger);
            return logger;
          default:
            return (Logger) null;
        }
      }
      finally
      {
        Monitor.Exit(ht);
      }
    }

    public virtual Enumeration getCurrentLoggers()
    {
      Vector vector = new Vector(this.ht.size());
      Enumeration enumeration = this.ht.elements();
      while (enumeration.hasMoreElements())
      {
        object obj = enumeration.nextElement();
        if (obj is Logger)
          vector.addElement(obj);
      }
      return vector.elements();
    }

    [Obsolete(null, false)]
    public virtual Enumeration getCurrentCategories() => this.getCurrentLoggers();

    public virtual RendererMap getRendererMap() => this.rendererMap;

    public virtual Logger getRootLogger() => this.root;

    public virtual bool isDisabled(int level) => this.thresholdInt > level;

    [Obsolete(null, false)]
    public virtual void overrideAsNeeded(string @override) => LogLog.warn("The Hiearchy.overrideAsNeeded method has been deprecated.");

    public virtual void resetConfiguration()
    {
      this.getRootLogger().setLevel(Level.DEBUG);
      this.root.setResourceBundle((ResourceBundle) null);
      this.setThreshold(Level.ALL);
      object ht = (object) this.ht;
      \u003CCorArrayWrapper\u003E.Enter(ht);
      try
      {
        this.shutdown();
        Enumeration currentLoggers = this.getCurrentLoggers();
        while (currentLoggers.hasMoreElements())
        {
          Logger logger = (Logger) currentLoggers.nextElement();
          logger.setLevel((Level) null);
          logger.setAdditivity(true);
          logger.setResourceBundle((ResourceBundle) null);
        }
      }
      finally
      {
        Monitor.Exit(ht);
      }
      this.rendererMap.clear();
    }

    [Obsolete(null, false)]
    public virtual void setDisableOverride(string @override) => LogLog.warn("The Hiearchy.setDisableOverride method has been deprecated.");

    public virtual void setRenderer(Class renderedClass, ObjectRenderer renderer) => this.rendererMap.put(renderedClass, renderer);

    public virtual void shutdown()
    {
      Logger rootLogger = this.getRootLogger();
      rootLogger.closeNestedAppenders();
      object ht = (object) this.ht;
      \u003CCorArrayWrapper\u003E.Enter(ht);
      try
      {
        Enumeration currentLoggers1 = this.getCurrentLoggers();
        while (currentLoggers1.hasMoreElements())
          ((Category) currentLoggers1.nextElement()).closeNestedAppenders();
        rootLogger.removeAllAppenders();
        Enumeration currentLoggers2 = this.getCurrentLoggers();
        while (currentLoggers2.hasMoreElements())
          ((Category) currentLoggers2.nextElement()).removeAllAppenders();
      }
      finally
      {
        Monitor.Exit(ht);
      }
    }

    [JavaFlags(18)]
    private void updateParents(Logger cat)
    {
      string name = cat.name;
      int num = StringImpl.length(name);
      bool flag = false;
      for (int index = StringImpl.lastIndexOf(name, 46, num - 1); index >= 0; index = StringImpl.lastIndexOf(name, 46, index - 1))
      {
        CategoryKey categoryKey = new CategoryKey(StringImpl.substring(name, 0, index));
        object obj = this.ht.get((object) categoryKey);
        switch (obj)
        {
          case null:
            ProvisionNode provisionNode = new ProvisionNode(cat);
            this.ht.put((object) categoryKey, (object) provisionNode);
            break;
          case Category _:
            flag = true;
            cat.parent = (Category) obj;
            goto label_8;
          case ProvisionNode _:
            ((Vector) obj).addElement((object) cat);
            break;
          default:
            ((Throwable) new IllegalStateException(new StringBuffer().append("unexpected object type ").append((object) ObjectImpl.getClass(obj)).append(" in ht.").ToString())).printStackTrace();
            break;
        }
      }
label_8:
      if (flag)
        return;
      cat.parent = (Category) this.root;
    }

    [JavaFlags(18)]
    private void updateChildren(ProvisionNode pn, Logger logger)
    {
      int num = pn.size();
      for (int index = 0; index < num; ++index)
      {
        Logger logger1 = (Logger) pn.elementAt(index);
        if (!StringImpl.startsWith(logger1.parent.name, logger.name))
        {
          logger.parent = logger1.parent;
          logger1.parent = (Category) logger;
        }
      }
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      Hierarchy hierarchy = this;
      ObjectImpl.clone((object) hierarchy);
      return ((object) hierarchy).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
