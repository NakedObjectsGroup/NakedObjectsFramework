// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.WindowControl
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark.metal
{
  public abstract class WindowControl : AbstractControlView
  {
    public const int HEIGHT = 13;
    public const int WIDTH = 15;

    [JavaFlags(4)]
    public WindowControl(UserAction action, View target)
      : base(action, target)
    {
    }

    public override Size getMaximumSize() => new Size(15, 13);
  }
}
