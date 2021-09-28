// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DebugCanvasAbsolute
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using java.io;
using java.lang;
using java.util;
using org.nakedobjects.utility;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Canvas;")]
  public class DebugCanvasAbsolute : Canvas
  {
    private DebugString buffer;
    private int level;
    private int offsetX;
    private int offsetY;

    public DebugCanvasAbsolute(DebugString buffer, Bounds bounds)
      : this(buffer, 0, bounds.getX(), bounds.getY())
    {
    }

    private DebugCanvasAbsolute(DebugString buffer, int level, int x, int y)
    {
      this.level = level;
      this.buffer = buffer;
      this.offsetX = x;
      this.offsetY = y;
    }

    public virtual void clearBackground(View view, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Clear background of ").append((object) view).append(" to ").append((object) color).append(this.line()).ToString());
    }

    public virtual Canvas createSubcanvas()
    {
      this.buffer.blankLine();
      this.indent();
      this.buffer.appendln("Create subcanvas for same area");
      return (Canvas) new DebugCanvasAbsolute(this.buffer, this.level + 1, this.offsetX, this.offsetY);
    }

    public virtual Canvas createSubcanvas(Bounds bounds) => this.createSubcanvas(bounds.getX(), bounds.getY(), bounds.getWidth(), bounds.getHeight());

    public virtual Canvas createSubcanvas(int x, int y, int width, int height)
    {
      this.indent();
      int x1 = this.offsetX + x;
      int num1 = x1 + width - 1;
      int y1 = this.offsetY + y;
      int num2 = y1 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Canvas ").append(x1).append(",").append(y1).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num1).append(",").append(num2).append(") ").append(this.line()).ToString());
      return (Canvas) new DebugCanvasAbsolute(this.buffer, this.level + 1, x1, y1);
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
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Rectangle (3D) ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append(this.line()).ToString());
    }

    public virtual void drawIcon(Image icon, int x, int y)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + icon.getWidth() - 1;
      int num4 = num2 + icon.getHeight() - 1;
      this.buffer.appendln(new StringBuffer().append("Icon ").append(num1).append(",").append(num2).append(" ").append(icon.getWidth()).append(nameof (x)).append(icon.getHeight()).append(" (").append(num3).append(",").append(num4).append(") ").append(this.line()).ToString());
    }

    public virtual void drawIcon(Image icon, int x, int y, int width, int height)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Icon ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append(this.line()).ToString());
    }

    public virtual void drawLine(int x, int y, int x2, int y2, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = this.offsetX + x2;
      int num4 = this.offsetY + y2;
      this.buffer.appendln(new StringBuffer().append("Line from ").append(num1).append(",").append(num2).append(" to ").append(num3).append(",").append(num4).append(" ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawLine(Location start, int xExtent, int yExtent, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Line from ").append(start.getX()).append(",").append(start.getY()).append(" to ").append(start.getX() + xExtent).append(",").append(start.getY() + yExtent).append(" ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawOval(int x, int y, int width, int height, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      this.buffer.appendln(new StringBuffer().append("Oval ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawRectangle(int x, int y, int width, int height, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Rectangle ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append((object) color).append(this.line()).ToString());
    }

    private string line()
    {
      StringWriter stringWriter;
      ((Throwable) new RuntimeException()).printStackTrace(new PrintWriter((Writer) (stringWriter = new StringWriter())));
      StringTokenizer stringTokenizer = new StringTokenizer(stringWriter.ToString(), "\n\r");
      stringTokenizer.nextElement();
      stringTokenizer.nextElement();
      stringTokenizer.nextElement();
      string str = stringTokenizer.nextToken();
      return StringImpl.substring(str, StringImpl.indexOf(str, 40));
    }

    public virtual void drawRectangleAround(View view, Color color)
    {
      Bounds bounds = view.getBounds();
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Rectangle 0,0 ").append(bounds.getWidth()).append("x").append(bounds.getHeight()).append(" ").append((object) color).append(this.line()).ToString());
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
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Rounded Rectangle ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawShape(Shape shape, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape ").append((object) shape).append(" ").append((object) color).ToString());
    }

    public virtual void drawShape(Shape shape, int x, int y, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      this.buffer.appendln(new StringBuffer().append("Shape ").append((object) shape).append(" at ").append(num1).append(",").append(num2).append(" (left, top)").append(" ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawSolidOval(int x, int y, int width, int height, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Oval (solid) ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawSolidRectangle(int x, int y, int width, int height, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      int num3 = num1 + width - 1;
      int num4 = num2 + height - 1;
      this.buffer.appendln(new StringBuffer().append("Rectangle (solid) ").append(num1).append(",").append(num2).append(" ").append(width).append(nameof (x)).append(height).append(" (").append(num3).append(",").append(num4).append(") ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawSolidShape(Shape shape, Color color)
    {
      this.indent();
      this.buffer.appendln(new StringBuffer().append("Shape (solid) ").append((object) shape).append(" ").append((object) color).ToString());
    }

    public virtual void drawSolidShape(Shape shape, int x, int y, Color color)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      this.buffer.appendln(new StringBuffer().append("Shape (solid)").append((object) shape).append(" at ").append(num1).append(",").append(num2).append(" (left, top)").append(" ").append((object) color).append(this.line()).ToString());
    }

    public virtual void drawText(string text, int x, int y, Color color, Text style)
    {
      this.indent();
      int num1 = this.offsetX + x;
      int num2 = this.offsetY + y;
      this.buffer.appendln(new StringBuffer().append("Text ").append(num1).append(",").append(num2).append(" \"").append(text).append("\" ").append((object) color).append(this.line()).ToString());
    }

    private void indent()
    {
      for (int index = 0; index < this.level; ++index)
        this.buffer.append((object) "   ");
    }

    public virtual void offset(int x, int y)
    {
      this.offsetX += x;
      this.offsetY += y;
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
      DebugCanvasAbsolute debugCanvasAbsolute = this;
      ObjectImpl.clone((object) debugCanvasAbsolute);
      return ((object) debugCanvasAbsolute).MemberwiseClone();
    }
  }
}
