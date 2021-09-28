// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.SyslogQuietWriter
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.io;
using java.lang;
using org.apache.log4j.spi;

namespace org.apache.log4j.helpers
{
  public class SyslogQuietWriter : QuietWriter
  {
    [JavaFlags(0)]
    public int syslogFacility;
    [JavaFlags(0)]
    public int level;

    public SyslogQuietWriter(Writer writer, int syslogFacility, ErrorHandler eh)
      : base(writer, eh)
    {
      this.syslogFacility = syslogFacility;
    }

    public virtual void setLevel(int level) => this.level = level;

    public virtual void setSyslogFacility(int syslogFacility) => this.syslogFacility = syslogFacility;

    public override void write(string @string) => base.write(new StringBuffer().append("<").append(this.syslogFacility | this.level).append(">").append(@string).ToString());
  }
}
