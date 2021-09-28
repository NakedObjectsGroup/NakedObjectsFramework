// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.SubviewFocusManager
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark
{
  public class SubviewFocusManager : AbstractFocusManager
  {
    private readonly WindowBorder windowBorder;

    public SubviewFocusManager(WindowBorder container)
      : base((View) container)
    {
      this.windowBorder = container;
    }

    public SubviewFocusManager(View container)
      : base(container)
    {
      this.windowBorder = (WindowBorder) null;
    }

    public SubviewFocusManager(View container, View initalFocus)
      : base(container, initalFocus)
    {
      this.windowBorder = (WindowBorder) null;
    }

    [JavaFlags(4)]
    public override View[] getChildViews()
    {
      View[] subviews = this.container.getSubviews();
      View[] viewArray1;
      if (this.windowBorder == null)
      {
        int length = 0;
        viewArray1 = length >= 0 ? new View[length] : throw new NegativeArraySizeException();
      }
      else
        viewArray1 = this.windowBorder.getButtons();
      View[] viewArray2 = viewArray1;
      int length1 = subviews.Length + viewArray2.Length;
      View[] viewArray3 = length1 >= 0 ? new View[length1] : throw new NegativeArraySizeException();
      System.arraycopy((object) subviews, 0, (object) viewArray3, 0, subviews.Length);
      System.arraycopy((object) viewArray2, 0, (object) viewArray3, subviews.Length, viewArray2.Length);
      return viewArray3;
    }
  }
}
