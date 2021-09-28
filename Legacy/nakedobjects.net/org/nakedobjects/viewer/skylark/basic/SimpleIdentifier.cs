// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.SimpleIdentifier
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class SimpleIdentifier : AbstractViewDecorator
  {
    public SimpleIdentifier(View wrappedView)
      : base(wrappedView)
    {
    }

    public override void debugDetails(StringBuffer b) => b.append(nameof (SimpleIdentifier));

    public override void dragIn(ContentDrag drag)
    {
      this.wrappedView.dragIn(drag);
      this.markDamaged();
    }

    public override void dragOut(ContentDrag drag)
    {
      this.wrappedView.dragOut(drag);
      this.markDamaged();
    }

    public override void draw(Canvas canvas)
    {
      Color color = (Color) null;
      if (this.getState().canDrop())
        color = Style.VALID;
      else if (this.getState().cantDrop())
        color = Style.INVALID;
      else if (this.getState().isViewIdentified() || this.getState().isObjectIdentified())
        color = Style.PRIMARY1;
      this.wrappedView.draw(canvas.createSubcanvas());
      if (color == null)
        return;
      Size size = this.getSize();
      canvas.drawRectangle(0, 0, size.getWidth() - 1, size.getHeight() - 1, color);
      canvas.drawRectangle(1, 1, size.getWidth() - 3, size.getHeight() - 3, color);
    }

    public override void entered()
    {
      this.getState().setObjectIdentified();
      this.wrappedView.entered();
      this.markDamaged();
    }

    public override void exited()
    {
      this.getState().clearObjectIdentified();
      this.wrappedView.exited();
      this.markDamaged();
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/SimpleIdentifier").ToString();
  }
}
