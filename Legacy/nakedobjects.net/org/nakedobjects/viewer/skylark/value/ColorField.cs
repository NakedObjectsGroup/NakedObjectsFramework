// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.value.ColorField
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.@object.value;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.value
{
  public class ColorField : AbstractField
  {
    private int color;

    public ColorField(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
    }

    public override void draw(Canvas canvas)
    {
      Color color = !this.hasFocus() ? (!this.getParent().getState().isObjectIdentified() ? (!this.getParent().getState().isRootViewIdentified() ? Style.SECONDARY1 : Style.PRIMARY2) : Style.IDENTIFIED) : Style.PRIMARY1;
      int y1 = 0;
      int x1 = 0;
      Size size = this.getSize();
      int width1 = size.getWidth() - 1;
      int height1 = size.getHeight() - 1;
      canvas.drawRectangle(x1, y1, width1, height1, color);
      int x2 = x1 + 1;
      int y2 = y1 + 1;
      int width2 = width1 - 1;
      int height2 = height1 - 1;
      canvas.drawSolidRectangle(x2, y2, width2, height2, new Color(this.getColor()));
    }

    public override void firstClick(Click click)
    {
      if (!((ValueContent) this.getContent()).isEditable().isAllowed())
        return;
      View view = (View) new DisposeOverlay((View) new ColorFieldOverlay(this));
      Location absoluteLocation = this.getAbsoluteLocation();
      view.setLocation(absoluteLocation);
      this.getViewManager().setOverlayView(view);
    }

    public override int getBaseline() => View.VPADDING + Style.NORMAL.getAscent();

    [JavaFlags(0)]
    public virtual int getColor() => ((ColorValue) ((ValueField) this.getContent()).getObject()).color();

    public override Size getMaximumSize() => new Size(45, 15);

    [JavaFlags(4)]
    public override void save()
    {
      try
      {
        this.parseEntry(new StringBuffer().append("").append(this.color).ToString());
      }
      catch (InvalidEntryException ex)
      {
        throw new NotImplementedException();
      }
    }

    [JavaFlags(0)]
    public virtual void setColor(int color)
    {
      this.color = color;
      this.initiateSave();
    }

    [JavaFlags(41)]
    public class Specification : AbstractFieldSpecification
    {
      public override bool canDisplay(Content content) => content.isValue() && content.getNaked() is ColorValue;

      public override View createView(Content content, ViewAxis axis) => (View) new ColorField(content, (ViewSpecification) this, axis);

      public override string getName() => "Color";
    }
  }
}
