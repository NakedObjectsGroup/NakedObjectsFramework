// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.Logger
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j.spi;
using System.ComponentModel;

namespace org.apache.log4j
{
  public class Logger : Category
  {
    private static readonly string FQCN;

    [JavaFlags(4)]
    public Logger(string name)
      : base(name)
    {
    }

    public static Logger getLogger(string name) => LogManager.getLogger(name);

    public static Logger getLogger(Class clazz) => LogManager.getLogger(clazz.getName());

    public static Logger getRootLogger() => LogManager.getRootLogger();

    public static Logger getLogger(string name, LoggerFactory factory) => LogManager.getLogger(name, factory);

    [JavaFlags(32778)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static Logger()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
