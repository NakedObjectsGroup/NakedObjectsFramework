// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.core.DrawingCanvas
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using com.ms.vjsharp.lang;
using com.ms.vjsharp.util;
using java.awt;
using java.awt.image;
using java.lang;
using System;

namespace org.nakedobjects.viewer.skylark.core
{
  [JavaInterfaces("1;org/nakedobjects/viewer/skylark/Canvas;")]
  public class DrawingCanvas : Canvas
  {
    private Color color;
    private Font font;
    private Graphics graphics;

    private DrawingCanvas(Graphics graphics) => this.graphics = graphics;

    public DrawingCanvas(Graphics bufferGraphic, int x, int y, int width, int height)
    {
      this.graphics = bufferGraphic;
      this.graphics.clipRect(x, y, width, height);
    }

    public virtual void clearBackground(View view, Color color)
    {
      Bounds bounds = view.getBounds();
      this.drawSolidRectangle(0, 0, bounds.getWidth(), bounds.getHeight(), color);
    }

    private Polygon createOval(int x, int y, int width, int height)
    {
      int num1 = 40;
      int length1 = num1;
      int[] numArray1 = length1 >= 0 ? new int[length1] : throw new NegativeArraySizeException();
      int length2 = num1;
      int[] numArray2 = length2 >= 0 ? new int[length2] : throw new NegativeArraySizeException();
      double num2 = 0.0;
      for (int index = 0; index < num1; ++index)
      {
        numArray1[index] = x + Utilities.doubleToInt((double) width / 2.0) + Utilities.doubleToInt((double) width / 2.0 * Math.cos(num2));
        numArray2[index] = y + Utilities.doubleToInt((double) height / 2.0) + Utilities.doubleToInt((double) height / 2.0 * Math.sin(num2));
        num2 += 2.0 * Math.PI / (double) num1;
      }
      return new Polygon(numArray1, numArray2, num1);
    }

    public virtual Canvas createSubcanvas() => (Canvas) new DrawingCanvas(this.graphics.create());

    public virtual Canvas createSubcanvas(Bounds bounds) => this.createSubcanvas(bounds.getX(), bounds.getY(), bounds.getWidth(), bounds.getHeight());

    public virtual Canvas createSubcanvas(int x, int y, int width, int height)
    {
      Graphics bufferGraphic = this.graphics.create();
      bufferGraphic.translate(x, y);
      return (Canvas) new DrawingCanvas(bufferGraphic, 0, 0, width, height);
    }

    public virtual void draw3DRectangle(
      int x,
      int y,
      int width,
      int height,
      Color color,
      bool raised)
    {
      this.useColor(color);
      this.graphics.draw3DRect(x, y, width - 1, height - 1, raised);
    }

    public virtual void drawDebugOutline(Bounds bounds, int baseline, Color color)
    {
      int width = bounds.getWidth();
      int height = bounds.getHeight();
      this.drawRectangle(bounds.getX(), bounds.getY(), width, height, color);
      int num = bounds.getY() + height / 2;
      this.drawLine(bounds.getX(), num, width - 2, num, color);
      if (baseline <= 0)
        return;
      this.drawLine(bounds.getX(), baseline, width - 1, baseline, Color.DEBUG_BASELINE);
    }

    public virtual void drawIcon(Image icon, int x, int y) => this.graphics.drawImage(((AwtImage) icon).getAwtImage(), x, y, (ImageObserver) null);

    public virtual void drawIcon(Image icon, int x, int y, int width, int height) => this.graphics.drawImage(((AwtImage) icon).getAwtImage(), x, y, width - 1, height - 1, (ImageObserver) null);

    public virtual void drawLine(int x, int y, int x2, int y2, Color color)
    {
      this.useColor(color);
      this.graphics.drawLine(x, y, x2, y2);
    }

    public virtual void drawLine(Location start, int xExtent, int yExtent, Color color) => this.drawLine(start.getX(), start.getY(), start.getX() + xExtent, start.getY() + yExtent, color);

    public virtual void drawOval(int x, int y, int width, int height, Color color)
    {
      this.useColor(color);
      this.graphics.drawPolygon(this.createOval(x, y, width - 1, height - 1));
    }

    public virtual void drawRectangle(int x, int y, int width, int height, Color color)
    {
      this.useColor(color);
      this.graphics.drawRect(x, y, width - 1, height - 1);
    }

    public virtual void drawRectangleAround(View view, Color color)
    {
      Bounds bounds = view.getBounds();
      this.drawRectangle(0, 0, bounds.getWidth(), bounds.getHeight(), color);
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
      this.useColor(color);
      this.graphics.drawRoundRect(x, y, width - 1, height - 1, arcWidth, arcHeight);
    }

    public virtual void drawShape(Shape shape, Color color)
    {
      this.useColor(color);
      this.graphics.drawPolygon(shape.getX(), shape.getY(), shape.count());
    }

    public virtual void drawShape(Shape shape, int x, int y, Color color)
    {
      Shape shape1 = new Shape(shape);
      shape1.translate(x, y);
      this.drawShape(shape1, color);
    }

    public virtual void drawSolidOval(int x, int y, int width, int height, Color color)
    {
      this.useColor(color);
      this.graphics.fillPolygon(this.createOval(x, y, width, height));
    }

    public virtual void drawSolidRectangle(int x, int y, int width, int height, Color color)
    {
      this.useColor(color);
      this.graphics.fillRect(x, y, width, height);
    }

    public virtual void drawSolidShape(Shape shape, Color color)
    {
      this.useColor(color);
      this.graphics.fillPolygon(shape.getX(), shape.getY(), shape.count());
    }

    public virtual void drawSolidShape(Shape shape, int x, int y, Color color)
    {
      Shape shape1 = new Shape(shape);
      shape1.translate(x, y);
      this.drawSolidShape(shape1, color);
    }

    public virtual void drawText(string text, int x, int y, Color color, Text style)
    {
      this.useColor(color);
      this.useFont(style);
      this.graphics.drawString(text, x, y);
    }

    public virtual void offset(int x, int y) => this.graphics.translate(x, y);

    public virtual bool overlaps(Bounds bounds)
    {
      Rectangle clipBounds = this.graphics.getClipBounds();
      Bounds bounds1 = new Bounds((int) clipBounds.x, (int) clipBounds.y, (int) clipBounds.width, (int) clipBounds.height);
      return bounds.intersects(bounds1);
    }

    public override string ToString()
    {
      Rectangle clipBounds = this.graphics.getClipBounds();
      return new StringBuffer().append("Canvas [area=").append((int) clipBounds.x).append(",").append((int) clipBounds.y).append(" ").append((int) clipBounds.width).append("x").append((int) clipBounds.height).append(",color=").append((object) this.color).append(",font=").append((object) this.font).append("]").ToString();
    }

    private void useColor(Color color)
    {
      Color awtColor = color.getAwtColor();
      if (this.color == awtColor)
        return;
      this.color = awtColor;
      this.graphics.setColor(awtColor);
    }

    private void useFont(Text style)
    {
      Font awtFont = style.getAwtFont();
      if (this.font == awtFont)
        return;
      this.font = awtFont;
      this.graphics.setFont(awtFont);
    }

    [JavaFlags(4227077)]
    [JavaThrownExceptions("1;java/lang/CloneNotSupportedException;")]
    public new virtual object MemberwiseClone()
    {
      DrawingCanvas drawingCanvas = this;
      ObjectImpl.clone((object) drawingCanvas);
      return ((object) drawingCanvas).MemberwiseClone();
    }
  }
}
