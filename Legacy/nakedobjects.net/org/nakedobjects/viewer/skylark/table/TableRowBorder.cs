// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TableRowBorder
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using com.ms.vjsharp.cor;
using java.lang;
using org.nakedobjects.viewer.skylark.basic;
using org.nakedobjects.viewer.skylark.core;

namespace org.nakedobjects.viewer.skylark.table
{
  public class TableRowBorder : AbstractBorder
  {
    private int baseline;
    private IconGraphic icon;
    private TitleText title;

    public TableRowBorder(View wrappedRow)
      : base(wrappedRow)
    {
      this.icon = new IconGraphic((View) this, Style.NORMAL);
      this.title = (TitleText) new ObjectTitleText((View) this, Style.NORMAL);
      this.baseline = this.icon.getBaseline();
      this.left = this.requiredTitleWidth();
      ((TableAxis) wrappedRow.getViewAxis()).ensureOffset(this.left);
    }

    public override void debugDetails(StringBuffer b) => b.append(new StringBuffer().append("RowBorder ").append(this.left).append(" pixels").ToString());

    public override void draw(Canvas canvas)
    {
      int baseline = this.getBaseline();
      TableAxis viewAxis = (TableAxis) this.getViewAxis();
      int headerOffset = viewAxis.getHeaderOffset();
      Canvas subcanvas = canvas.createSubcanvas(0, 0, headerOffset, this.getSize().getHeight());
      int hpadding = View.HPADDING;
      this.icon.draw(subcanvas, hpadding, baseline);
      int x = hpadding + (this.icon.getSize().getWidth() + View.HPADDING + 0 + View.HPADDING);
      this.title.draw(subcanvas, x, baseline, this.getLeft() - x);
      int columnCount = viewAxis.getColumnCount();
      int num1 = viewAxis.getHeaderOffset() - 1;
      canvas.drawLine(num1 - 1, 0, num1 - 1, this.getSize().getHeight() - 1, Style.SECONDARY1);
      canvas.drawLine(num1, 0, num1, this.getSize().getHeight() - 1, Style.SECONDARY1);
      for (int column = 0; column < columnCount; ++column)
      {
        num1 += viewAxis.getColumnWidth(column);
        canvas.drawLine(num1, 0, num1, this.getSize().getHeight() - 1, Style.SECONDARY1);
      }
      int num2 = this.getSize().getHeight() - 1;
      canvas.drawLine(0, num2, this.getSize().getWidth(), num2, Style.SECONDARY2);
      base.draw(canvas);
    }

    public override int getBaseline() => this.baseline;

    [JavaFlags(4)]
    public override int getLeft() => ((TableAxis) this.wrappedView.getViewAxis()).getHeaderOffset();

    [JavaFlags(4)]
    public virtual int requiredTitleWidth() => View.HPADDING + this.icon.getSize().getWidth() + View.HPADDING + this.title.getSize().getWidth() + View.HPADDING;

    public override string ToString() => new StringBuffer().append(this.wrappedView.ToString()).append("/RowBorder").ToString();

    public override ViewAreaType viewAreaType(Location mouseLocation)
    {
      if (mouseLocation.getX() <= this.left)
        return ViewAreaType.CONTENT;
      return mouseLocation.getX() >= this.getSize().getWidth() - this.right ? ViewAreaType.VIEW : base.viewAreaType(mouseLocation);
    }
  }
}
