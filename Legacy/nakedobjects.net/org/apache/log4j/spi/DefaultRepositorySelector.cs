// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.spi.DefaultRepositorySelector
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;

namespace org.apache.log4j.spi
{
  [JavaInterfaces("1;org/apache/log4j/spi/RepositorySelector;")]
  public class DefaultRepositorySelector : RepositorySelector
  {
    [JavaFlags(16)]
    public readonly LoggerRepository repository;

    public DefaultRepositorySelector(LoggerRepository repository) => this.repository = repository;

    public virtual LoggerRepository getLoggerRepository() => this.repository;

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DefaultRepositorySelector repositorySelector = this;
      ObjectImpl.clone((object) repositorySelector);
      return ((object) repositorySelector).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
