// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Drag
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark
{
  public abstract class Drag : PointerEvent
  {
    [JavaFlags(4)]
    public Drag()
      : base(0)
    {
    }

    [JavaFlags(1028)]
    public abstract void cancel(Viewer viewer);

    [JavaFlags(1028)]
    public abstract void drag(Viewer viewer, Location location, int mods);

    [JavaFlags(1028)]
    public abstract void end(Viewer viewer);

    [JavaFlags(1028)]
    public abstract void start(Viewer viewer);

    public abstract View getOverlay();
  }
}
