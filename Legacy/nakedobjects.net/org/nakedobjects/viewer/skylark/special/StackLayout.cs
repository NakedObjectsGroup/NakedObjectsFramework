// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.StackLayout
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.metal;

namespace org.nakedobjects.viewer.skylark.special
{
  public class StackLayout : AbstractBuilderDecorator
  {
    private bool fixedWidth;

    public StackLayout(CompositeViewBuilder design)
      : base(design)
    {
      this.fixedWidth = false;
    }

    public StackLayout(CompositeViewBuilder design, bool fixedWidth)
      : base(design)
    {
      this.fixedWidth = fixedWidth;
    }

    public override Size getRequiredSize(View view)
    {
      int height = 0;
      int width = 0;
      foreach (View subview in view.getSubviews())
      {
        Size requiredSize = subview.getRequiredSize(new Size());
        width = Math.max(width, requiredSize.getWidth());
        height += requiredSize.getHeight();
      }
      return new Size(width, height);
    }

    public override bool isOpen() => true;

    public override void layout(View view, Size maximumSize)
    {
      int x = 0;
      int y = 0;
      View[] subviews = view.getSubviews();
      int width = 0;
      for (int index = 0; index < subviews.Length; ++index)
      {
        View view1 = subviews[index];
        view1.layout(new Size(maximumSize));
        Size requiredSize = view1.getRequiredSize(new Size(maximumSize));
        width = Math.max(width, requiredSize.getWidth());
      }
      for (int index = 0; index < subviews.Length; ++index)
      {
        View view2 = subviews[index];
        Size requiredSize = view2.getRequiredSize(new Size());
        if (this.fixedWidth || view2.getSpecification() is TextFieldSpecification)
          requiredSize.ensureWidth(width);
        view2.setSize(requiredSize);
        view2.setLocation(new Location(x, y));
        y += requiredSize.getHeight();
      }
    }
  }
}
