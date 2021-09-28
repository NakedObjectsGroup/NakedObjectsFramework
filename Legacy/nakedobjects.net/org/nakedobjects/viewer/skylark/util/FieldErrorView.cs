// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.util.FieldErrorView
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.util
{
  public class FieldErrorView : AbstractView
  {
    private string error;

    public FieldErrorView(string errorMessage)
      : base((Content) null, (ViewSpecification) null, (ViewAxis) null)
    {
      this.error = errorMessage;
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas);
      Size size = this.getSize();
      canvas.drawSolidRectangle(0, 0, size.getWidth() - 1, size.getHeight() - 1, Style.WHITE);
      canvas.drawRectangle(0, 0, size.getWidth() - 1, size.getHeight() - 1, Style.BLACK);
      canvas.drawText(this.error, 14, 20, Style.INVALID, Style.NORMAL);
    }

    public override int getBaseline() => 20;

    public override Size getMaximumSize() => new Size(250, 30);

    public override ViewAreaType viewAreaType(Location mouseLocation) => mouseLocation.getX() <= 10 ? ViewAreaType.VIEW : ViewAreaType.CONTENT;

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/ViewSpecification;")]
    [JavaFlags(41)]
    public class Specification : ViewSpecification
    {
      public virtual bool canDisplay(Content content) => true;

      public virtual View createView(Content content, ViewAxis axis) => throw new NotImplementedException();

      public virtual string getName() => "Field Error";

      public virtual bool isSubView() => false;

      public virtual bool isReplaceable() => false;

      public virtual bool isOpen() => false;

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        FieldErrorView.Specification specification = this;
        ObjectImpl.clone((object) specification);
        return ((object) specification).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
