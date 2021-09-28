// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.special.ResizeDrag
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark.special
{
  public class ResizeDrag : InternalDrag
  {
    public const int BOTTOM = 2;
    public const int BOTTOM_LEFT = 7;
    public const int BOTTOM_RIGHT = 8;
    public const int LEFT = 3;
    public const int RIGHT = 4;
    public const int TOP = 1;
    public const int TOP_LEFT = 5;
    public const int TOP_RIGHT = 6;
    private readonly Location anchor;
    private readonly int direction;
    private readonly ViewResizeOutline overlay;
    private readonly View view;
    private readonly Size minimumSize;
    private readonly Size maximumSize;

    public ResizeDrag(View view, Bounds resizeArea, int direction)
      : this(view, resizeArea, direction, (Size) null, (Size) null)
    {
    }

    public ResizeDrag(
      View view,
      Bounds resizeArea,
      int direction,
      Size minimumSize,
      Size maximumSize)
    {
      this.view = view;
      this.direction = direction;
      this.anchor = resizeArea.getLocation();
      this.minimumSize = minimumSize;
      this.maximumSize = maximumSize;
      this.overlay = new ViewResizeOutline(resizeArea);
      this.overlay.setLocation(resizeArea.getLocation());
    }

    [JavaFlags(4)]
    public override void cancel(Viewer viewer) => this.view.dragCancel((InternalDrag) this);

    [JavaFlags(4)]
    public override void drag(Viewer viewer, Location location, int mods)
    {
      switch (this.direction)
      {
        case 1:
          this.extendUpward(location);
          break;
        case 2:
          this.extendDownward(location);
          break;
        case 3:
          this.extendLeft(location);
          break;
        case 4:
          this.extendRight(location);
          break;
        case 5:
          this.extendLeft(location);
          this.extendUpward(location);
          break;
        case 6:
          this.extendRight(location);
          this.extendUpward(location);
          break;
        case 7:
          this.extendLeft(location);
          this.extendDownward(location);
          break;
        case 8:
          this.extendRight(location);
          this.extendDownward(location);
          break;
      }
    }

    [JavaFlags(4)]
    public override void end(Viewer viewer)
    {
      this.view.dragTo((InternalDrag) this);
      this.view.getViewManager().clearOverlayView(this.view);
    }

    private void extendDownward(Location location)
    {
      this.overlay.markDamaged();
      int height = location.getY() - this.anchor.getY();
      this.overlay.setSize(new Size(this.overlay.getSize().getWidth(), height));
      this.overlay.markDamaged();
    }

    private void extendLeft(Location location)
    {
      this.overlay.markDamaged();
      int height = this.overlay.getSize().getHeight();
      int width = this.anchor.getX() - location.getX();
      this.overlay.setSize(new Size(width, height));
      this.overlay.setBounds(new Bounds(this.anchor.getX() - width, this.anchor.getY(), width, height));
      this.overlay.markDamaged();
    }

    private void extendRight(Location location)
    {
      this.overlay.markDamaged();
      int height = this.overlay.getSize().getHeight();
      int width = location.getX() - this.anchor.getX();
      if (this.maximumSize != null && width > this.maximumSize.getWidth())
        width = this.maximumSize.getWidth();
      if (this.minimumSize != null && width < this.minimumSize.getWidth())
        width = this.minimumSize.getWidth();
      this.overlay.setSize(new Size(width, height));
      this.overlay.markDamaged();
    }

    private void extendUpward(Location location)
    {
      this.overlay.markDamaged();
      int height = this.anchor.getY() - location.getY();
      int width = this.overlay.getSize().getWidth();
      this.overlay.setSize(new Size(width, height));
      this.overlay.setBounds(new Bounds(this.anchor.getX(), this.anchor.getY() - height, width, height));
      this.overlay.markDamaged();
    }

    public virtual int getDirection() => this.direction;

    public override Location getLocation()
    {
      Size size = this.overlay.getSize();
      return new Location(size.getWidth(), size.getHeight());
    }

    public override View getOverlay() => (View) this.overlay;

    [JavaFlags(4)]
    public override void start(Viewer viewer)
    {
    }
  }
}
