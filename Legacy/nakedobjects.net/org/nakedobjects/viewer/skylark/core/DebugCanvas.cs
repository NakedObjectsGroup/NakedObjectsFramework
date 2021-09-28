// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugCanvas
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.lang;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Canvas;")]
  public class DebugCanvas : Canvas
  {
    private DebugString buffer;
    private int level;

    public DebugCanvas(DebugString buffer, Bounds bounds)
      : this(buffer, 0)
    {
    }

    private DebugCanvas(DebugString buffer, int level)
    {
      this.level = level;
      this.buffer = buffer;
    }

    public virtual void clearBackground(View view, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Clear background of ").append((object) view).append(" to ").append((object) color).ToString());
    }

    public virtual Canvas createSubcanvas()
    {
      this.buffer.blankLine();
      this.indent();
      this.buffer.appendln("Create subcanvas for same area");
      return (Canvas) new DebugCanvas(this.buffer, this.level + 1);
    }

    public virtual Canvas createSubcanvas(Bounds bounds) => this.createSubcanvas(bounds.getX(), bounds.getY(), bounds.getWidth(), bounds.getHeight());

    public virtual Canvas createSubcanvas(int x, int y, int width, int height)
    {
      this.buffer.blankLine();
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Create subcanvas for area ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).ToString());
      return (Canvas) new DebugCanvas(this.buffer, this.level + 1);
    }

    public virtual void draw3DRectangle(
      int x,
      int y,
      int width,
      int height,
      Color color,
      bool raised)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rectangle (3D) ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).ToString());
    }

    public virtual void drawIcon(Image icon, int x, int y)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Icon ").append(x).append(",").append(y).append(" ").append(icon.getWidth()).append(nameof (x)).append(icon.getHeight()).ToString());
    }

    public virtual void drawIcon(Image icon, int x, int y, int width, int height)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Icon ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).ToString());
    }

    public virtual void drawLine(int x, int y, int x2, int y2, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Line from ").append(x).append(",").append(y).append(" to ").append(x2).append(",").append(y2).append(" ").append((object) color).ToString());
    }

    public virtual void drawLine(Location start, int xExtent, int yExtent, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Line from ").append(start.getX()).append(",").append(start.getY()).append(" to ").append(start.getX() + xExtent).append(",").append(start.getY() + yExtent).append(" ").append((object) color).ToString());
    }

    public virtual void drawOval(int x, int y, int width, int height, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Oval ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).append(" ").append((object) color).ToString());
    }

    public virtual void drawRectangle(int x, int y, int width, int height, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rectangle ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).append(" ").append((object) color).ToString());
    }

    public virtual void drawRectangleAround(View view, Color color)
    {
      Bounds bounds = view.getBounds();
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rectangle 0,0 ").append(bounds.getWidth()).append("x").append(bounds.getHeight()).append(" ").append((object) color).ToString());
    }

    public virtual void drawRoundedRectangle(
      int x,
      int y,
      int width,
      int height,
      int arcWidth,
      int arcHeight,
      Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rounded Rectangle ").append(x).append(",").append(y).append(" ").append(x + width).append(nameof (x)).append(y + height).append(" ").append((object) color).ToString());
    }

    public virtual void drawShape(Shape shape, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape ").append((object) shape).append(" ").append((object) color).ToString());
    }

    public virtual void drawShape(Shape shape, int x, int y, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape ").append((object) shape).append(" at ").append(x).append("/").append(y).append(" (left, top)").append(" ").append((object) color).ToString());
    }

    public virtual void drawSolidOval(int x, int y, int width, int height, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Oval (solid) ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).append(" ").append((object) color).ToString());
    }

    public virtual void drawSolidRectangle(int x, int y, int width, int height, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rectangle (solid) ").append(x).append(",").append(y).append(" ").append(width).append(nameof (x)).append(height).append(" ").append((object) color).ToString());
    }

    public virtual void drawSolidShape(Shape shape, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape (solid) ").append((object) shape).append(" ").append((object) color).ToString());
    }

    public virtual void drawSolidShape(Shape shape, int x, int y, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape (solid)").append((object) shape).append(" at ").append(x).append("/").append(y).append(" (left, top)").append(" ").append((object) color).ToString());
    }

    public virtual void drawText(string text, int x, int y, Color color, Text style)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Text ").append(x).append(",").append(y).append(" \"").append(text).append("\" ").append((object) style).append(" ").append((object) color).ToString());
    }

    private void indent()
    {
      for (int index = 0; index < this.level; ++index)
        this.buffer.append((object) "   ");
    }

    public virtual void offset(int x, int y)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Offset by ").append(x).append("/").append(y).append(" (left, top)").ToString());
    }

    public virtual bool overlaps(Bounds bounds) => true;

    public override string ToString() => "Canvas";

    public virtual void drawDebugOutline(Bounds bounds, int baseline, Color color)
    {
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DebugCanvas debugCanvas = this;
      ObjectImpl.clone((object) debugCanvas);
      return ((object) debugCanvas).MemberwiseClone();
    }
  }
}
