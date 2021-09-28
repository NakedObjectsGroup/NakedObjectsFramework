// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.xml.XMLWatchdog
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using org.apache.log4j.helpers;

namespace org.apache.log4j.xml
{
  [JavaFlags(32)]
  public class XMLWatchdog : FileWatchdog
  {
    [JavaFlags(0)]
    public XMLWatchdog(string filename)
      : base(filename)
    {
    }

    public override void doOnChange() => new DOMConfigurator().doConfigure(this.filename, LogManager.getLoggerRepository());
  }
}
