// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.OpenOptionFieldBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.awt.@event;
using org.nakedobjects.viewer.skylark.@event;
using org.nakedobjects.viewer.skylark.core;
using System.ComponentModel;

namespace org.nakedobjects.viewer.skylark.special
{
  public abstract class OpenOptionFieldBorder : AbstractBorder
  {
    private bool over;

    public OpenOptionFieldBorder(View wrappedView)
      : base(wrappedView)
    {
      this.right = 18;
    }

    [JavaFlags(1028)]
    public abstract View createOverlay();

    public override void draw(Canvas canvas)
    {
      Size size = this.getSize();
      int x = size.getWidth() - this.right + 5 - View.HPADDING;
      int y = (size.getHeight() - 6) / 2;
      if (this.isAvailable())
      {
        Shape shape = new Shape(0, 0);
        shape.addVertex(6, 6);
        shape.addVertex(12, 0);
        canvas.drawShape(shape, x, y, Style.SECONDARY2);
        if (this.over)
        {
          Color color = !this.over ? Style.PRIMARY2 : Style.SECONDARY1;
          canvas.drawSolidShape(shape, x, y, color);
        }
      }
      base.draw(canvas);
    }

    public override bool canFocus() => this.isAvailable();

    public override void exited()
    {
      if (this.over)
        this.markDamaged();
      this.over = false;
      base.exited();
    }

    public override void firstClick(Click click)
    {
      if ((double) (click.getLocation().getX() - 2) >= (double) (this.getSize().getWidth() - this.right))
      {
        if (!this.isAvailable())
          return;
        this.open();
      }
      else
        base.firstClick(click);
    }

    public override void mouseWheelMoved(MouseWheelEvent evt)
    {
      if (this.isAvailable())
        this.open();
      ((InputEvent) evt).consume();
    }

    public override Size getRequiredSize(Size maximumSize)
    {
      maximumSize.contractWidth(View.HPADDING);
      Size requiredSize = base.getRequiredSize(maximumSize);
      requiredSize.extendWidth(View.HPADDING);
      return requiredSize;
    }

    [JavaFlags(4)]
    public virtual bool isAvailable() => true;

    public override void keyPressed(KeyboardAction key)
    {
      if (key.getKeyCode() == 40 && this.isAvailable())
      {
        this.open();
        key.consume();
      }
      base.keyPressed(key);
    }

    public override void mouseMoved(Location at)
    {
      if (at.getX() >= this.getSize().getWidth() - this.right)
      {
        this.getViewManager().showArrowCursor();
        if (!this.over)
          this.markDamaged();
        this.over = true;
      }
      else
      {
        if (this.over)
          this.markDamaged();
        this.over = false;
        base.mouseMoved(at);
      }
    }

    private void open() => BackgroundThread.run((View) this, (BackgroundTask) new OpenOptionFieldBorder.\u0031(this));

    [JavaInterfaces("1;org/nakedobjects/viewer/skylark/core/BackgroundTask;")]
    [JavaFlags(32)]
    [Inner]
    public class \u0031 : BackgroundTask
    {
      [JavaFlags(32770)]
      [EditorBrowsable(EditorBrowsableState.Never)]
      private OpenOptionFieldBorder this\u00240;

      public virtual void execute()
      {
        View overlay = this.this\u00240.createOverlay();
        Location absoluteLocation = this.this\u00240.getView().getAbsoluteLocation();
        absoluteLocation.add(this.this\u00240.getView().getPadding().getLeft() - 1, this.this\u00240.getSize().getHeight() + 2);
        overlay.setLocation(absoluteLocation);
        this.this\u00240.getViewManager().setOverlayView(overlay);
      }

      public virtual string getDescription() => "";

      public virtual string getName() => "Opening lookup";

      public \u0031(OpenOptionFieldBorder _param1)
      {
        this.this\u00240 = _param1;
        if (_param1 != null)
          return;
        ObjectImpl.getClass((object) _param1);
      }

      [JavaFlags(4227077)]
      [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
      public new virtual object MemberwiseClone()
      {
        OpenOptionFieldBorder.\u0031 obj = this;
        ObjectImpl.clone((object) obj);
        return ((object) obj).MemberwiseClone();
      }

      [JavaFlags(4227073)]
      public override string ToString() => ObjectImpl.jloToString((object) this);
    }
  }
}
