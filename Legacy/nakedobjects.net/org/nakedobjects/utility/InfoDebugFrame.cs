// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.utility.InfoDebugFrame
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;

namespace org.nakedobjects.utility
{
  public class InfoDebugFrame : DebugFrame
  {
    private const long serialVersionUID = 1;
    private DebugInfo[] info;

    [JavaFlags(4)]
    public override DebugInfo[] getInfo() => this.info;

    public virtual void setInfo(DebugInfo info)
    {
      int length = 1;
      DebugInfo[] debugInfoArray = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
      debugInfoArray[0] = info;
      this.info = debugInfoArray;
    }

    public virtual void setInfo(DebugInfo[] info) => this.info = info;
  }
}
