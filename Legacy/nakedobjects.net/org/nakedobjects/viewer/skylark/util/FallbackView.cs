// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.FallbackView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.util
{
  [JavaFlags(32)]
  public class FallbackView : AbstractView
  {
    [JavaFlags(4)]
    public FallbackView(Content content, ViewSpecification specification, ViewAxis axis)
      : base(content, specification, axis)
    {
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Size size = this.getSize();
      canvas.drawSolidRectangle(0, 0, size.getWidth() - 1, size.getHeight() - 1, Style.SECONDARY3);
      canvas.drawSolidRectangle(0, 0, 10, size.getHeight() - 1, Style.SECONDARY2);
      canvas.drawLine(10, 0, 10, 50, Style.BLACK);
      canvas.drawRectangle(0, 0, size.getWidth() - 1, size.getHeight() - 1, Style.BLACK);
      canvas.drawText("Fallback View", 14, 20, Style.BLACK, Style.NORMAL);
      canvas.drawText(this.getContent().ToString(), 14, 40, Style.BLACK, Style.NORMAL);
    }

    public override int getBaseline() => 20;

    public override Size getMaximumSize() => new Size(200, 50);

    public override ViewAreaType viewAreaType(Location mouseLocation) => mouseLocation.getX() <= 10 ? ViewAreaType.VIEW : ViewAreaType.CONTENT;

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
    [JavaFlags(41)]
    public class Specification : ViewSpecification
    {
      public virtual bool canDisplay(Content content) => true;

      public virtual View createView(Content content, ViewAxis axis) => (View) new FallbackView(content, (ViewSpecification) this, axis);

      public virtual string getName() => "Fallback";

      public virtual bool isSubView() => false;

      public virtual bool isReplaceable() => false;

      public virtual bool isOpen() => false;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        FallbackView.Specification specification = this;
        ObjectImpl.clone((object) specification);
        return ((object) specification).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
