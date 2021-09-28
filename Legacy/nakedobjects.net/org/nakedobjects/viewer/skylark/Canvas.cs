// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.Canvas
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;

namespace org.nakedobjects.viewer.skylark
{
  [JavaInterface]
  public interface Canvas
  {
    void clearBackground(View view, Color color);

    Canvas createSubcanvas();

    Canvas createSubcanvas(Bounds bounds);

    Canvas createSubcanvas(int x, int y, int width, int height);

    void draw3DRectangle(int x, int y, int width, int height, Color color, bool raised);

    void drawDebugOutline(Bounds bounds, int baseline, Color color);

    void drawIcon(Image icon, int x, int y);

    void drawIcon(Image icon, int x, int y, int width, int height);

    void drawLine(int x, int y, int x2, int y2, Color color);

    void drawLine(Location start, int xExtent, int yExtent, Color color);

    void drawOval(int x, int y, int width, int height, Color color);

    void drawRectangle(int x, int y, int width, int height, Color color);

    void drawRectangleAround(View view, Color color);

    void drawRoundedRectangle(
      int x,
      int y,
      int width,
      int height,
      int arcWidth,
      int arcHeight,
      Color color);

    void drawShape(Shape shape, Color color);

    void drawShape(Shape shape, int x, int y, Color color);

    void drawSolidOval(int x, int y, int width, int height, Color color);

    void drawSolidRectangle(int x, int y, int width, int height, Color color);

    void drawSolidShape(Shape shape, Color color);

    void drawSolidShape(Shape shape, int x, int y, Color color);

    void drawText(string text, int x, int y, Color color, Text style);

    void offset(int x, int y);

    bool overlaps(Bounds bounds);
  }
}
