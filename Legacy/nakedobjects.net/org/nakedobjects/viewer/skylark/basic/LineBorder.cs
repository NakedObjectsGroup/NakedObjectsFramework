// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.basic.LineBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.basic
{
  public class LineBorder : AbstractBorder
  {
    private readonly Color color;

    public LineBorder(View wrappedView)
      : this(1, wrappedView)
    {
    }

    public LineBorder(int size, View wrappedView)
      : this(size, Style.BLACK, wrappedView)
    {
    }

    public LineBorder(Color color, View wrappedView)
      : this(1, color, wrappedView)
    {
    }

    public LineBorder(int size, Color color, View wrappedView)
      : base(wrappedView)
    {
      this.top = size;
      this.left = size;
      this.bottom = size;
      this.right = size;
      this.color = color;
    }

    [JavaFlags(4)]
    public override void debugDetails(StringBuffer b) => b.append(new StringBuffer().append("LineBorder ").append(this.top).append(" pixels\n").ToString());

    public override void draw(Canvas canvas)
    {
      Size size = this.getSize();
      int width = size.getWidth();
      for (int index = 0; index < this.left; ++index)
        canvas.drawRectangle(index, index, width - 2 * index, size.getHeight() - 2 * index, this.color);
      base.draw(canvas);
    }

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/LineBorder").ToString();
  }
}
