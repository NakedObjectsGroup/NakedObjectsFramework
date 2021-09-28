// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Category
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.text;
using java.util;
using org.apache.log4j.helpers;
using org.apache.log4j.spi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace org.apache.log4j
{
  [JavaInterfaces("1;org/apache/log4j/spi/AppenderAttachable;")]
  public class Category : AppenderAttachable
  {
    [JavaFlags(4)]
    public string name;
    [JavaFlags(68)]
    public volatile Level level;
    [JavaFlags(68)]
    public volatile Category parent;
    private static readonly string FQCN;
    [JavaFlags(4)]
    public ResourceBundle resourceBundle;
    [JavaFlags(4)]
    public LoggerRepository repository;
    [JavaFlags(0)]
    public AppenderAttachableImpl aai;
    [JavaFlags(4)]
    public bool additive;

    [JavaFlags(4)]
    public Category(string name)
    {
      this.additive = true;
      this.name = name;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void addAppender(Appender newAppender)
    {
      if (this.aai == null)
        this.aai = new AppenderAttachableImpl();
      this.aai.addAppender(newAppender);
      this.repository.fireAddAppenderEvent(this, newAppender);
    }

    public virtual void assertLog(bool assertion, string msg)
    {
      if (assertion)
        return;
      this.error((object) msg);
    }

    public virtual void callAppenders(LoggingEvent @event)
    {
      int num = 0;
      for (Category category = this; category != null; category = category.parent)
      {
        object obj = (object) category;
        \u003CCorArrayWrapper\u003E.Enter(obj);
        try
        {
          if (category.aai != null)
            num += category.aai.appendLoopOnAppenders(@event);
          if (!category.additive)
            break;
        }
        finally
        {
          Monitor.Exit(obj);
        }
      }
      if (num != 0)
        return;
      this.repository.emitNoAppenderWarning(this);
    }

    [JavaFlags(32)]
    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void closeNestedAppenders()
    {
      Enumeration allAppenders = this.getAllAppenders();
      if (allAppenders == null)
        return;
      while (allAppenders.hasMoreElements())
      {
        Appender appender = (Appender) allAppenders.nextElement();
        if (appender is AppenderAttachable)
          appender.close();
      }
    }

    public virtual void debug(object message)
    {
      if (this.repository.isDisabled(10000) || !Level.DEBUG.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.DEBUG, message, (Throwable) null);
    }

    public virtual void debug(object message, Throwable t)
    {
      if (this.repository.isDisabled(10000) || !Level.DEBUG.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.DEBUG, message, t);
    }

    public virtual void error(object message)
    {
      if (this.repository.isDisabled(40000) || !Level.ERROR.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.ERROR, message, (Throwable) null);
    }

    public virtual void error(object message, Throwable t)
    {
      if (this.repository.isDisabled(40000) || !Level.ERROR.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.ERROR, message, t);
    }

    [Obsolete(null, false)]
    public static Logger exists(string name) => LogManager.exists(name);

    public virtual void fatal(object message)
    {
      if (this.repository.isDisabled(50000) || !Level.FATAL.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.FATAL, message, (Throwable) null);
    }

    public virtual void fatal(object message, Throwable t)
    {
      if (this.repository.isDisabled(50000) || !Level.FATAL.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.FATAL, message, t);
    }

    [JavaFlags(4)]
    public virtual void forcedLog(string fqcn, Priority level, object message, Throwable t) => this.callAppenders(new LoggingEvent(fqcn, this, level, message, t));

    public virtual bool getAdditivity() => this.additive;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Enumeration getAllAppenders() => this.aai == null ? (Enumeration) NullEnumeration.getInstance() : this.aai.getAllAppenders();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual Appender getAppender(string name) => this.aai == null || name == null ? (Appender) null : this.aai.getAppender(name);

    public virtual Level getEffectiveLevel()
    {
      for (Category category = this; category != null; category = category.parent)
      {
        if (category.level != null)
          return category.level;
      }
      return (Level) null;
    }

    [Obsolete(null, false)]
    public virtual Priority getChainedPriority()
    {
      for (Category category = this; category != null; category = category.parent)
      {
        if (category.level != null)
          return (Priority) category.level;
      }
      return (Priority) null;
    }

    [Obsolete(null, false)]
    public static Enumeration getCurrentCategories() => LogManager.getCurrentLoggers();

    [Obsolete(null, false)]
    public static LoggerRepository getDefaultHierarchy() => LogManager.getLoggerRepository();

    [Obsolete(null, false)]
    public virtual LoggerRepository getHierarchy() => this.repository;

    public virtual LoggerRepository getLoggerRepository() => this.repository;

    public static Category getInstance(string name) => (Category) LogManager.getLogger(name);

    public static Category getInstance(Class clazz) => (Category) LogManager.getLogger(clazz);

    [JavaFlags(17)]
    public string getName() => this.name;

    [JavaFlags(17)]
    public Category getParent() => this.parent;

    [JavaFlags(17)]
    public Level getLevel() => this.level;

    [Obsolete(null, false)]
    [JavaFlags(17)]
    public Level getPriority() => this.level;

    [JavaFlags(25)]
    public static Category getRoot() => (Category) LogManager.getRootLogger();

    public virtual ResourceBundle getResourceBundle()
    {
      for (Category category = this; category != null; category = category.parent)
      {
        if (category.resourceBundle != null)
          return category.resourceBundle;
      }
      return (ResourceBundle) null;
    }

    [JavaFlags(4)]
    public virtual string getResourceBundleString(string key)
    {
      ResourceBundle resourceBundle = this.getResourceBundle();
      if (resourceBundle == null)
        return (string) null;
      try
      {
        return resourceBundle.getString(key);
      }
      catch (MissingResourceException ex)
      {
        this.error((object) new StringBuffer().append("No resource is associated with key \"").append(key).append("\".").ToString());
        return (string) null;
      }
    }

    public virtual void info(object message)
    {
      if (this.repository.isDisabled(20000) || !Level.INFO.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.INFO, message, (Throwable) null);
    }

    public virtual void info(object message, Throwable t)
    {
      if (this.repository.isDisabled(20000) || !Level.INFO.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.INFO, message, t);
    }

    public virtual bool isAttached(Appender appender) => appender != null && this.aai != null && this.aai.isAttached(appender);

    public virtual bool isDebugEnabled() => !this.repository.isDisabled(10000) && Level.DEBUG.isGreaterOrEqual((Priority) this.getEffectiveLevel());

    public virtual bool isEnabledFor(Priority level) => !this.repository.isDisabled(level.level) && level.isGreaterOrEqual((Priority) this.getEffectiveLevel());

    public virtual bool isInfoEnabled() => !this.repository.isDisabled(20000) && Level.INFO.isGreaterOrEqual((Priority) this.getEffectiveLevel());

    public virtual void l7dlog(Priority priority, string key, Throwable t)
    {
      if (this.repository.isDisabled(priority.level) || !priority.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      string str = this.getResourceBundleString(key) ?? key;
      this.forcedLog(Category.FQCN, priority, (object) str, t);
    }

    public virtual void l7dlog(Priority priority, string key, object[] @params, Throwable t)
    {
      if (this.repository.isDisabled(priority.level) || !priority.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      string resourceBundleString = this.getResourceBundleString(key);
      string str = resourceBundleString != null ? MessageFormat.format(resourceBundleString, @params) : key;
      this.forcedLog(Category.FQCN, priority, (object) str, t);
    }

    public virtual void log(Priority priority, object message, Throwable t)
    {
      if (this.repository.isDisabled(priority.level) || !priority.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, priority, message, t);
    }

    public virtual void log(Priority priority, object message)
    {
      if (this.repository.isDisabled(priority.level) || !priority.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, priority, message, (Throwable) null);
    }

    public virtual void log(string callerFQCN, Priority level, object message, Throwable t)
    {
      if (this.repository.isDisabled(level.level) || !level.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(callerFQCN, level, message, t);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void removeAllAppenders()
    {
      if (this.aai == null)
        return;
      this.aai.removeAllAppenders();
      this.aai = (AppenderAttachableImpl) null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void removeAppender(Appender appender)
    {
      if (appender == null || this.aai == null)
        return;
      this.aai.removeAppender(appender);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public virtual void removeAppender(string name)
    {
      if (name == null || this.aai == null)
        return;
      this.aai.removeAppender(name);
    }

    public virtual void setAdditivity(bool additive) => this.additive = additive;

    [JavaFlags(16)]
    public void setHierarchy(LoggerRepository repository) => this.repository = repository;

    public virtual void setLevel(Level level) => this.level = level;

    [Obsolete(null, false)]
    public virtual void setPriority(Priority priority) => this.level = (Level) priority;

    public virtual void setResourceBundle(ResourceBundle bundle) => this.resourceBundle = bundle;

    [Obsolete(null, false)]
    public static void shutdown() => LogManager.shutdown();

    public virtual void warn(object message)
    {
      if (this.repository.isDisabled(30000) || !Level.WARN.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.WARN, message, (Throwable) null);
    }

    public virtual void warn(object message, Throwable t)
    {
      if (this.repository.isDisabled(30000) || !Level.WARN.isGreaterOrEqual((Priority) this.getEffectiveLevel()))
        return;
      this.forcedLog(Category.FQCN, (Priority) Level.WARN, message, t);
    }

    public virtual bool isWarnEnabled() => !this.repository.isDisabled(30000) && Level.WARN.isGreaterOrEqual((Priority) this.getEffectiveLevel());

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Category()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      Category category = this;
      ObjectImpl.clone((object) category);
      return ((object) category).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
