// Decompiled with JetBrains decompiler
// Type: org.apache.log4j.helpers.FormattingInfo
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.apache.log4j.helpers
{
  public class FormattingInfo
  {
    [JavaFlags(0)]
    public int min;
    [JavaFlags(0)]
    public int max;
    [JavaFlags(0)]
    public bool leftAlign;

    [JavaFlags(0)]
    public virtual void reset()
    {
      this.min = -1;
      this.max = int.MaxValue;
      this.leftAlign = false;
    }

    [JavaFlags(0)]
    public virtual void dump() => LogLog.debug(new StringBuffer().append("min=").append(this.min).append(", max=").append(this.max).append(", leftAlign=").append(this.leftAlign).ToString());

    public FormattingInfo()
    {
      this.min = -1;
      this.max = int.MaxValue;
      this.leftAlign = false;
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      FormattingInfo formattingInfo = this;
      ObjectImpl.clone((object) formattingInfo);
      return ((object) formattingInfo).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
