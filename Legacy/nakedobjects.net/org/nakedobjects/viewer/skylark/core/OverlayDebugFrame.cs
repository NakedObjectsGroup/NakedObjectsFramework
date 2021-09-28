// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.OverlayDebugFrame
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.core
{
  public class OverlayDebugFrame : DebugFrame
  {
    private readonly Viewer viewer;

    public OverlayDebugFrame(Viewer viewer) => this.viewer = viewer;

    [JavaFlags(4)]
    public override DebugInfo[] getInfo()
    {
      DebugView debugView = new DebugView(this.viewer.getOverlayView() ?? (View) new OverlayDebugFrame.EmptyView(this));
      int length = 1;
      DebugInfo[] debugInfoArray = length >= 0 ? new DebugInfo[length] : throw new NegativeArraySizeException();
      debugInfoArray[0] = (DebugInfo) debugView;
      return debugInfoArray;
    }

    [JavaFlags(32)]
    [Inner]
    public class EmptyView : AbstractView
    {
      [EditorBrowsable(EditorBrowsableState.Never)]
      [JavaFlags(32770)]
      private OverlayDebugFrame this\u00240;

      [JavaFlags(0)]
      public EmptyView(OverlayDebugFrame _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }
    }
  }
}
