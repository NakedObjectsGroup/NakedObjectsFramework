// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.ActionDialogFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class ActionDialogFocusManager : AbstractFocusManager
  {
    private readonly ButtonBorder buttonBorder;

    public ActionDialogFocusManager(ButtonBorder buttonBorder)
      : base(buttonBorder.getView())
    {
      this.buttonBorder = buttonBorder;
    }

    [JavaFlags(4)]
    public override View[] getChildViews()
    {
      View[] subviews = this.container.getSubviews();
      View[] buttons = (View[]) this.buttonBorder.getButtons();
      int length = subviews.Length + buttons.Length;
      View[] viewArray = length >= 0 ? new View[length] : throw new NegativeArraySizeException();
      System.arraycopy((object) subviews, 0, (object) viewArray, 0, subviews.Length);
      System.arraycopy((object) buttons, 0, (object) viewArray, subviews.Length, buttons.Length);
      return viewArray;
    }
  }
}
