// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ExceptionDialogBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaFlags(32)]
  public class ExceptionDialogBorder : AbstractWindowBorder
  {
    public ExceptionDialogBorder(View wrappedView, bool scrollable)
      : base(!scrollable ? wrappedView : (View) new ScrollBorder(wrappedView))
    {
      int length = 1;
      WindowControl[] controls = length >= 0 ? new WindowControl[length] : throw new NegativeArraySizeException();
      controls[0] = (WindowControl) new CloseWindowControl((View) this);
      this.setControls(controls);
    }

    [JavaFlags(4)]
    public override string title() => this.getContent().windowTitle();

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/ExceptionDialogBorder [").append((object) this.getSpecification()).append("]").ToString();
  }
}
