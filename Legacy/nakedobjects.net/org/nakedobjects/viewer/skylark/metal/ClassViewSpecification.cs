// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.metal.ClassViewSpecification
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.@object;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.metal
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
  public class ClassViewSpecification : ViewSpecification
  {
    public virtual bool canDisplay(Content content) => content.isObject() && content.getNaked() != null && content.getNaked().getObject() is NakedClass;

    public virtual View createView(Content content, ViewAxis axis) => (View) new WindowBorder((View) new ClassViewSpecification.ClassView(content, (ViewSpecification) this, axis), false);

    public virtual string getName() => "Class View";

    public virtual bool isSubView() => false;

    public virtual bool isReplaceable() => false;

    public virtual bool isOpen() => true;

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      ClassViewSpecification viewSpecification = this;
      ObjectImpl.clone((object) viewSpecification);
      return ((object) viewSpecification).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);

    [JavaFlags(40)]
    public class ClassView : AbstractView
    {
      [JavaFlags(4)]
      public ClassView(Content content, ViewSpecification specification, ViewAxis axis)
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
    }
  }
}
