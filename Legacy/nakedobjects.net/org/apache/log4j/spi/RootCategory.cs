// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.RootCategory
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.apache.log4j.helpers;

namespace org.apache.log4j.spi
{
  public sealed class RootCategory : Logger
  {
    public RootCategory(Level level)
      : base("root")
    {
      this.setLevel(level);
    }

    [JavaFlags(17)]
    public Level getChainedLevel() => this.level;

    [JavaFlags(17)]
    public override sealed void setLevel(Level level)
    {
      if (level == null)
        LogLog.error("You have tried to set a null level to root.", new Throwable());
      else
        this.level = level;
    }

    [JavaFlags(17)]
    public void setPriority(Level level) => this.setLevel(level);
  }
}
