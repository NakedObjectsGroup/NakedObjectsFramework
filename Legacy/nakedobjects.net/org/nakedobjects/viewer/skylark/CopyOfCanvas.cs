// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.CopyOfCanvas
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;

namespace org.nakedobjects.viewer.skylark
{
  public abstract class CopyOfCanvas
  {
    private Bounds drawingArea;
    private Location origin;

    [JavaFlags(4)]
    public CopyOfCanvas()
    {
      this.origin = new Location();
      this.drawingArea = new Bounds();
    }

    [JavaFlags(4)]
    public virtual void setDrawingArea(Bounds bounds) => this.drawingArea = bounds;

    [JavaFlags(4)]
    public virtual Bounds getDrawingArea() => this.drawingArea;

    [JavaFlags(4)]
    public virtual Location getOrigin() => this.origin;

    public virtual CopyOfCanvas createSubcanvas() => this.createSubcanvas(this.drawingArea.x, this.drawingArea.y, this.drawingArea.width, this.drawingArea.height);

    public virtual CopyOfCanvas createSubcanvas(Bounds bounds) => this.createSubcanvas(bounds.x, bounds.y, bounds.width, bounds.height);

    public virtual CopyOfCanvas createSubcanvas(int x, int y, int width, int height)
    {
      CopyOfCanvas canvas = this.createCanvas(this);
      int num1 = this.drawingArea.getWidth() - x;
      int num2 = this.drawingArea.getHeight() - y;
      canvas.origin.move(x, y);
      canvas.drawingArea.translate(x, y);
      canvas.drawingArea.setWidth(Math.max(0, Math.min(num1, width)));
      canvas.drawingArea.setHeight(Math.max(0, Math.min(num2, height)));
      return canvas;
    }

    [JavaFlags(1028)]
    public abstract CopyOfCanvas createCanvas(CopyOfCanvas source);

    public abstract void draw3DRectangle(int x, int y, int width, int height, bool raised);

    public abstract void drawIcon(Image icon, int x, int y);

    public abstract void drawIcon(Image icon, int x, int y, int width, int height);

    public abstract void drawLine(int x, int y, int x2, int y2, Color color);

    public abstract void drawLine(Location start, int xExtent, int yExtent, Color color);

    public abstract void drawRectangle(int x, int y, int width, int height, Color color);

    public abstract void drawRectangle(Location at, Size size, Color color);

    public abstract void drawRectangle(Size size, Color color);

    public abstract void drawRoundedRectangle(
      int x,
      int y,
      int width,
      int height,
      int arcWidth,
      int arcHeight,
      Color color);

    public abstract void drawSolidOval(int x, int y, int width, int height, Color color);

    public abstract void drawSolidRectangle(int x, int y, int width, int height, Color color);

    public abstract void drawSolidRectangle(Location at, Size size, Color color);

    public abstract void drawSolidShape(Shape shape, int x, int y, Color color);

    public abstract void drawText(string text, int x, int y, Color color, Text style);

    public virtual bool intersects(Bounds view) => this.drawingArea.intersects(view);

    public virtual void reduce(int left, int top, int right, int bottom)
    {
      this.origin.move(left, top);
      this.drawingArea.translate(left, top);
      this.drawingArea.contractHeight(top + bottom);
      this.drawingArea.contractWidth(left + right);
    }

    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    [JavaFlags(4227077)]
    public new virtual object MemberwiseClone()
    {
      CopyOfCanvas copyOfCanvas = this;
      ObjectImpl.clone((object) copyOfCanvas);
      return ((object) copyOfCanvas).MemberwiseClone();
    }

    [JavaFlags(4227073)]
    public override string ToString() => ObjectImpl.jloToString((object) this);
  }
}
