// Decompiled with JetBrains decompiler
// Type: org.nakedobjects.viewer.skylark.table.TableHeader
// Assembly: nakedobjects.net, Version=2.1.5386.20054, Culture=neutral, PublicKeyToken=null
// MVID: 1DE237A2-7EE8-405D-B628-89F8099DF058
// Assembly location: C:\Users\scasc\Documents\sdm\SdmApp-Dlls\bin\nakedobjects.net.dll

using java.lang;
using org.nakedobjects.@object;
using org.nakedobjects.utility;
using org.nakedobjects.viewer.skylark.core;
using org.nakedobjects.viewer.skylark.special;

namespace org.nakedobjects.viewer.skylark.table
{
  public class TableHeader : AbstractView
  {
    private int height;
    private int resizeColumn;

    public TableHeader(Content content, ViewAxis axis)
      : base(content, (ViewSpecification) null, axis)
    {
      this.height = View.VPADDING + Style.LABEL.getTextHeight() + View.VPADDING;
    }

    public override void firstClick(Click click)
    {
      if (click.getLocation().getY() <= this.height)
      {
        TableAxis viewAxis = (TableAxis) this.getViewAxis();
        int column = viewAxis.getColumnAt(click.getLocation().getX()) - 1;
        switch (column)
        {
          case -2:
            base.firstClick(click);
            break;
          case -1:
            ((CollectionContent) this.getContent()).setOrderByElement();
            this.invalidateContent();
            break;
          default:
            NakedObjectField fieldForColumn = viewAxis.getFieldForColumn(column);
            ((CollectionContent) this.getContent()).setOrderByField(fieldForColumn);
            this.invalidateContent();
            break;
        }
      }
      else
        base.firstClick(click);
    }

    public override void invalidateContent() => this.getParent().invalidateContent();

    public override Size getMaximumSize() => new Size(-1, this.height);

    public override Drag dragStart(DragStart drag)
    {
      if (this.isOverColumnBorder(drag.getLocation()))
      {
        TableAxis viewAxis = (TableAxis) this.getViewAxis();
        this.resizeColumn = viewAxis.getColumnBorderAt(drag.getLocation().getX());
        Bounds resizeArea = new Bounds(this.getView().getAbsoluteLocation(), this.getSize());
        resizeArea.translate(this.getView().getPadding().getLeft(), this.getView().getPadding().getTop());
        if (this.resizeColumn == 0)
        {
          resizeArea.setWidth(viewAxis.getHeaderOffset());
        }
        else
        {
          resizeArea.translate(viewAxis.getLeftEdge(this.resizeColumn - 1), 0);
          resizeArea.setWidth(viewAxis.getColumnWidth(this.resizeColumn - 1));
        }
        Size minimumSize = new Size(70, 0);
        return (Drag) new ResizeDrag((View) this, resizeArea, 4, minimumSize, (Size) null);
      }
      return drag.getLocation().getY() <= this.height ? (Drag) null : base.dragStart(drag);
    }

    public override void dragTo(InternalDrag drag)
    {
      int num = drag.getOverlay() != null ? Math.max(70, drag.getOverlay().getSize().getWidth()) : throw new NakedObjectRuntimeException(new StringBuffer().append("No overlay for drag: ").append((object) drag).ToString());
      this.getViewManager().getSpy().addAction(new StringBuffer().append("Resize column to ").append(num).ToString());
      TableAxis viewAxis = (TableAxis) this.getViewAxis();
      if (this.resizeColumn == 0)
        viewAxis.setOffset(num);
      else
        viewAxis.setWidth(this.resizeColumn - 1, num);
      viewAxis.invalidateLayout();
    }

    public override void draw(Canvas canvas)
    {
      base.draw(canvas.createSubcanvas());
      int y = View.VPADDING + Style.LABEL.getAscent();
      TableAxis viewAxis = (TableAxis) this.getViewAxis();
      int num1 = viewAxis.getHeaderOffset() - 2;
      if (((CollectionContent) this.getContent()).getOrderByElement())
        this.drawOrderIndicator(canvas, viewAxis, num1 - 10);
      canvas.drawLine(0, this.height - 1, this.getSize().getWidth() - 1, this.height - 1, Style.SECONDARY1);
      canvas.drawLine(num1, 0, num1, this.getSize().getHeight() - 1, Style.SECONDARY1);
      int num2 = num1 + 1;
      int columnCount = viewAxis.getColumnCount();
      NakedObjectField fieldSortOrder = ((CollectionContent) this.getContent()).getFieldSortOrder();
      for (int column = 0; column < columnCount; ++column)
      {
        if (fieldSortOrder == viewAxis.getFieldForColumn(column))
          this.drawOrderIndicator(canvas, viewAxis, num2 + viewAxis.getColumnWidth(column) - 10);
        canvas.drawLine(num2, 0, num2, this.getSize().getHeight() - 1, Style.BLACK);
        canvas.createSubcanvas(num2, 0, viewAxis.getColumnWidth(column) - 1, this.height).drawText(viewAxis.getColumnName(column), View.HPADDING, y, Style.SECONDARY1, Style.LABEL);
        num2 += viewAxis.getColumnWidth(column);
      }
      canvas.drawLine(num2, 0, num2, this.getSize().getHeight() - 1, Style.SECONDARY2);
      canvas.drawRectangle(0, this.height, this.getSize().getWidth() - 1, this.getSize().getHeight() - this.height - 1, Style.SECONDARY2);
    }

    private void drawOrderIndicator(Canvas canvas, TableAxis axis, int x)
    {
      Shape shape = new Shape();
      if (((CollectionContent) this.getContent()).getReverseSortOrder())
      {
        shape.addVertex(0, 7);
        shape.addVertex(3, 0);
        shape.addVertex(6, 7);
      }
      else
      {
        shape.addVertex(0, 0);
        shape.addVertex(6, 0);
        shape.addVertex(3, 7);
      }
      canvas.drawShape(shape, x, 3, Style.SECONDARY2);
    }

    public override View identify(Location location)
    {
      this.getViewManager().getSpy().addTrace(new StringBuffer().append("Identify over column ").append((object) location).ToString());
      if (!this.isOverColumnBorder(location))
        return base.identify(location);
      this.getViewManager().getSpy().addAction("Identified over column ");
      return this.getView();
    }

    private bool isOverColumnBorder(Location at)
    {
      int x = at.getX();
      return ((TableAxis) this.getViewAxis()).getColumnBorderAt(x) >= 0;
    }

    public override void mouseMoved(Location at)
    {
      if (this.isOverColumnBorder(at))
      {
        this.getViewManager().showResizeRightCursor();
      }
      else
      {
        base.mouseMoved(at);
        this.getViewManager().showDefaultCursor();
      }
    }

    public override void secondClick(Click click)
    {
      if (this.isOverColumnBorder(click.getLocation()))
      {
        TableAxis viewAxis = (TableAxis) this.getViewAxis();
        int index1 = viewAxis.getColumnBorderAt(click.getLocation().getX()) - 1;
        if (index1 == -1)
        {
          foreach (View subview in this.getSubviews())
            viewAxis.ensureOffset(((TableRowBorder) subview).requiredTitleWidth());
        }
        else
        {
          View[] subviews = this.getSubviews();
          int width = 0;
          for (int index2 = 0; index2 < subviews.Length; ++index2)
          {
            View subview = subviews[index2].getSubviews()[index1];
            width = Math.max(width, subview.getRequiredSize(new Size()).getWidth());
          }
          viewAxis.setWidth(index1, width);
        }
        viewAxis.invalidateLayout();
      }
      else
        base.secondClick(click);
    }

    public override string ToString() => nameof (TableHeader);

    public override ViewAreaType viewAreaType(Location at)
    {
      int x = at.getX();
      return ((TableAxis) this.getViewAxis()).getColumnBorderAt(x) >= 0 ? ViewAreaType.INTERNAL : base.viewAreaType(at);
    }
  }
}
