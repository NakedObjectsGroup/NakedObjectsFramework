// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.LogManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using java.util;
using org.apache.log4j.spi;
using System;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class LogManager
  {
    [Obsolete(null, false)]
    public const string DEFAULT_CONFIGURATION_FILE = "log4j.properties";
    [JavaFlags(24)]
    public const string DEFAULT_XML_CONFIGURATION_FILE = "log4j.xml";
    [Obsolete(null, false)]
    public const string DEFAULT_CONFIGURATION_KEY = "log4j.configuration";
    [Obsolete(null, false)]
    public const string CONFIGURATOR_CLASS_KEY = "log4j.configuratorClass";
    [Obsolete(null, false)]
    public const string DEFAULT_INIT_OVERRIDE_KEY = "log4j.defaultInitOverride";
    private static object guard;
    private static RepositorySelector repositorySelector;

    [JavaThrownExceptions("1;java/lang/IllegalArgumentException;")]
    public static void setRepositorySelector(RepositorySelector selector, object guard)
    {
      if (LogManager.guard != null && LogManager.guard != guard)
        throw new IllegalArgumentException("Attempted to reset the LoggerFactory without possessing the guard.");
      if (selector == null)
        throw new IllegalArgumentException("RepositorySelector must be non-null.");
      LogManager.guard = guard;
      LogManager.repositorySelector = selector;
    }

    public static LoggerRepository getLoggerRepository() => LogManager.repositorySelector.getLoggerRepository();

    public static Logger getRootLogger() => LogManager.repositorySelector.getLoggerRepository().getRootLogger();

    public static Logger getLogger(string name) => LogManager.repositorySelector.getLoggerRepository().getLogger(name);

    public static Logger getLogger(Class clazz) => LogManager.repositorySelector.getLoggerRepository().getLogger(clazz.getName());

    public static Logger getLogger(string name, LoggerFactory factory) => LogManager.repositorySelector.getLoggerRepository().getLogger(name, factory);

    public static Logger exists(string name) => LogManager.repositorySelector.getLoggerRepository().exists(name);

    public static Enumeration getCurrentLoggers() => LogManager.repositorySelector.getLoggerRepository().getCurrentLoggers();

    public static void shutdown() => LogManager.repositorySelector.getLoggerRepository().shutdown();

    public static void resetConfiguration() => LogManager.repositorySelector.getLoggerRepository().resetConfiguration();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [JavaFlags(32778)]
    static LogManager()
    {
      // ISSUE: unable to decompile the method.
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      LogManager logManager = this;
      ObjectImpl.clone((object) logManager);
      return ((object) logManager).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
