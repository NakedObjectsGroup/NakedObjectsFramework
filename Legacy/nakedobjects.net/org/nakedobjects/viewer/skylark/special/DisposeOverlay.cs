// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.DisposeOverlay
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.special
{
  public class DisposeOverlay : AbstractViewDecorator
  {
    public DisposeOverlay(View wrappedView)
      : base(wrappedView)
    {
    }

    public override void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() == 27)
        this.dispose();
      base.keyPressed(key);
    }
  }
}
