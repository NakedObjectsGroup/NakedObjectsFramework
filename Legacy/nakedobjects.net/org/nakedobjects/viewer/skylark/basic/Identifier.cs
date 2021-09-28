// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.Identifier
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class Identifier : AbstractViewDecorator
  {
    private bool identified;

    public Identifier(View wrappedView)
      : base(wrappedView)
    {
    }

    public override void debugDetails(StringBuffer b) => b.append(nameof (Identifier));

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
      Size size = this.getSize();
      canvas.drawSolidRectangle(0, 0, size.getWidth(), size.getHeight(), Style.SECONDARY3);
      this.wrappedView.draw(canvas);
    }

    public override void entered()
    {
      this.getState().setObjectIdentified();
      this.wrappedView.entered();
      this.identified = true;
      this.markDamaged();
    }

    public override void exited()
    {
      this.getState().clearObjectIdentified();
      this.wrappedView.exited();
      this.identified = false;
      this.markDamaged();
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/Identifier [identified=").append(this.identified).append("]").ToString();
  }
}
