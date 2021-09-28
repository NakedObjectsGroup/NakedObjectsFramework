// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugDrawingAbsolute
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/utility/DebugInfo;")]
  public class DebugDrawingAbsolute : DebugInfo
  {
    private readonly View view;

    public DebugDrawingAbsolute(View display) => this.view = display;

    public virtual void debugData(DebugString debug) => this.view.draw((Canvas) new DebugCanvasAbsolute(debug, new Bounds(this.view.getAbsoluteLocation(), this.view.getSize())));

    public virtual string getDebugTitle() => "Drawing (Absolute)";

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      DebugDrawingAbsolute debugDrawingAbsolute = this;
      ObjectImpl.clone((object) debugDrawingAbsolute);
      return ((object) debugDrawingAbsolute).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
