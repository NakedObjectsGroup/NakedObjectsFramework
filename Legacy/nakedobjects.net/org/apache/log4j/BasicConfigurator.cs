// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.BasicConfigurator
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.log4j
{
  public class BasicConfigurator
  {
    [JavaFlags(4)]
    public BasicConfigurator()
    {
    }

    public static void configure() => Logger.getRootLogger().addAppender((Appender) new ConsoleAppender((Layout) new PatternLayout("%r [%t] %p %c %x - %m%n")));

    public static void configure(Appender appender) => Logger.getRootLogger().addAppender(appender);

    public static void resetConfiguration() => LogManager.resetConfiguration();

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      BasicConfigurator basicConfigurator = this;
      ObjectImpl.clone((object) basicConfigurator);
      return ((object) basicConfigurator).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
